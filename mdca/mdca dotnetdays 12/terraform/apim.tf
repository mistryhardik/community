################################################################################
# apim.tf
# Azure API Management — full setup
#
# Naming convention:
#   API Management: apim-<suffix>
#
# What this provisions:
#   1. APIM service instance
#   2. Application Insights logger (linked to appi-)
#   3. AI Agent API definition (backend = AI Agent App Service)
#   4. POST /api/chat operation
#   5. Standard product (requires subscription)
#   6. Inbound + Outbound policy:
#        - Subscription key check
#        - Per-tenant rate limiting
#        - Response caching (keyed on request body hash)
#        - System prompt enforcement at gateway
#        - Token usage trace to Application Insights
#   7. Tenant subscriptions (one per entry in var.apim_tenant_subscriptions)
#
# NOTE: APIM takes 30-45 minutes to provision. `terraform apply` will wait.
#       This is expected — do not interrupt.
################################################################################

# ── APIM Service ──────────────────────────────────────────────────────────────
resource "azurerm_api_management" "main" {
  name                = "apim-${local.name_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  publisher_name      = var.apim_publisher_name
  publisher_email     = var.apim_publisher_email
  sku_name            = var.apim_sku

  identity {
    type = "SystemAssigned"
  }

  tags = local.common_tags

  # APIM is slow to provision — timeouts are extended
  timeouts {
    create = "90m"
    update = "90m"
    delete = "90m"
  }
}

# ── APIM Logger (Application Insights) ───────────────────────────────────────
resource "azurerm_api_management_logger" "appinsights" {
  name                = "appinsights-logger"
  api_management_name = azurerm_api_management.main.name
  resource_group_name = azurerm_resource_group.main.name
  resource_id         = azurerm_application_insights.main.id

  application_insights {
    instrumentation_key = azurerm_application_insights.main.instrumentation_key
  }
}

# ── APIM Diagnostic (link APIM telemetry to App Insights) ────────────────────
resource "azurerm_api_management_diagnostic" "appinsights" {
  identifier               = "applicationinsights"
  resource_group_name      = azurerm_resource_group.main.name
  api_management_name      = azurerm_api_management.main.name
  api_management_logger_id = azurerm_api_management_logger.appinsights.id

  sampling_percentage       = 100.0
  always_log_errors         = true
  log_client_ip             = true
  verbosity                 = "information"
  http_correlation_protocol = "W3C"

  frontend_request {
    body_bytes = 1024
    headers_to_log = ["Content-Type", "Ocp-Apim-Subscription-Key"]
  }

  frontend_response {
    body_bytes = 1024
    headers_to_log = ["Content-Type", "X-Cache", "X-RateLimit-Remaining"]
  }

  backend_request {
    body_bytes = 1024
  }

  backend_response {
    body_bytes = 1024
  }
}

# ── Backend (AI Agent App Service) ───────────────────────────────────────────
resource "azurerm_api_management_backend" "ai_agent" {
  name                = "ai-agent-backend"
  resource_group_name = azurerm_resource_group.main.name
  api_management_name = azurerm_api_management.main.name
  protocol            = "http"
  url                 = "https://${azurerm_windows_web_app.ai_agent.default_hostname}"

  tls {
    validate_certificate_chain = true
    validate_certificate_name  = true
  }
}

# ── API Definition ────────────────────────────────────────────────────────────
resource "azurerm_api_management_api" "ai_agent" {
  name                  = "mcp-ai-agent-api"
  resource_group_name   = azurerm_resource_group.main.name
  api_management_name   = azurerm_api_management.main.name
  revision              = "1"
  display_name          = "MCP AI Agent API"
  path                  = "agent"
  protocols             = ["https"]
  subscription_required = true
  service_url           = "https://${azurerm_windows_web_app.ai_agent.default_hostname}"

  subscription_key_parameter_names {
    header = "Ocp-Apim-Subscription-Key"
    query  = "subscription-key"
  }
}

# ── POST /api/chat Operation ──────────────────────────────────────────────────
resource "azurerm_api_management_api_operation" "chat" {
  operation_id        = "post-chat"
  api_name            = azurerm_api_management_api.ai_agent.name
  api_management_name = azurerm_api_management.main.name
  resource_group_name = azurerm_resource_group.main.name
  display_name        = "Chat"
  method              = "POST"
  url_template        = "/api/chat"
  description         = "Send a message to the MCP-powered AI agent and receive a response."

  request {
    description = "Chat request body"
    representation {
      content_type = "application/json"
      example {
        name  = "default"
        value = jsonencode({
          message = "What is Azure API Management?"
          model   = "gpt-4o-demo"
        })
      }
    }
  }

  response {
    status_code = 200
    description = "Successful AI agent response with token usage"
    representation {
      content_type = "application/json"
      example {
        name  = "default"
        value = jsonencode({
          reply            = "Azure API Management is a fully managed gateway service..."
          model            = "gpt-4o-demo"
          toolsCalled      = []
          promptTokens     = 28
          completionTokens = 56
          totalTokens      = 84
        })
      }
    }
  }

  response {
    status_code = 429
    description = "Rate limit exceeded for this tenant"
  }

  response {
    status_code = 401
    description = "Missing or invalid subscription key"
  }
}

# ── API-level policy (all operations) ────────────────────────────────────────
# Applied to ALL operations on this API. Covers:
#   1. Subscription key validation
#   2. Per-tenant rate limiting (counter keyed on subscription ID)
#   3. Response cache lookup (keyed on request body hash)
#   4. System prompt enforcement at the gateway
#   5. Cache store on outbound
#   6. Token usage trace to Application Insights
resource "azurerm_api_management_api_policy" "ai_agent" {
  api_name            = azurerm_api_management_api.ai_agent.name
  api_management_name = azurerm_api_management.main.name
  resource_group_name = azurerm_resource_group.main.name

  xml_content = <<XML
<policies>
    <inbound>
        <base />
        <!-- 1. Validate subscription key is present -->
        <check-header name="Ocp-Apim-Subscription-Key" failed-check-httpcode="401" failed-check-error-message="Missing subscription key" ignore-case="false" />
        <!-- 2. Per-tenant rate limiting: 20 calls per 60 seconds -->
        <rate-limit-by-key calls="20" renewal-period="60" counter-key="@(context.Subscription.Id)" remaining-calls-variable-name="remainingCalls" retry-after-header-name="Retry-After" remaining-calls-header-name="X-RateLimit-Remaining" />
        <!-- Bonus: Enforce model and system prompt at gateway — tenants cannot override -->
        <!--<set-body>@{
            var body = context.Request.Body.As<Newtonsoft.Json.Linq.JObject>(true);
            body["model"] = "gpt-4o-mini";
            body["systemPrompt"] = "You are a dotnet principal engineer. Answer in 2-3 plain sentences maximum. No bullet points, no headers, no examples unless asked.";
            return body.ToString();
        }</set-body>-->
        <!-- 3. Response caching: cache by message body for 5 minutes -->
        <cache-lookup-value key="@("chat-" + context.Request.Body.As<string>(true).GetHashCode())" variable-name="cachedResponse" />
        <choose>
            <when condition="@(context.Variables.ContainsKey("cachedResponse"))">
                <return-response>
                    <set-status code="200" reason="OK" />
                    <set-header name="Content-Type" exists-action="override">
                        <value>application/json</value>
                    </set-header>
                    <set-header name="X-Cache" exists-action="override">
                        <value>HIT</value>
                    </set-header>
                    <set-body>@((string)context.Variables["cachedResponse"])</set-body>
                </return-response>
            </when>
        </choose>
        <!-- 4. Add cache MISS header for non-cached responses -->
        <set-header name="X-Cache" exists-action="override">
            <value>MISS</value>
        </set-header>
        <!-- 5. Forward to your App Service backend -->
        <set-backend-service backend-id="ai-agent-backend" />
    </inbound>
    <backend>
        <base />
    </backend>
    <outbound>
        <base />
        <!-- 6. Store response in cache for 5 minutes (300 seconds) -->
        <cache-store-value key="@("chat-" + context.Request.Body.As<string>(true).GetHashCode())" value="@(context.Response.Body.As<string>(true))" duration="300" />
        <!-- 7. Log token usage per tenant to Application Insights -->
        <trace source="token-usage" severity="information">
            <message>@{
        var body = context.Response.Body.As<Newtonsoft.Json.Linq.JObject>(true);
        return new Newtonsoft.Json.Linq.JObject(
            new Newtonsoft.Json.Linq.JProperty("tenantId",         context.Subscription.Id),
            new Newtonsoft.Json.Linq.JProperty("tenantName",       context.Subscription.Name),
            new Newtonsoft.Json.Linq.JProperty("promptTokens",     body?["promptTokens"]),
            new Newtonsoft.Json.Linq.JProperty("completionTokens", body?["completionTokens"]),
            new Newtonsoft.Json.Linq.JProperty("totalTokens",      body?["totalTokens"]),
            new Newtonsoft.Json.Linq.JProperty("cacheHit",         context.Response.Headers.GetValueOrDefault("X-Cache", "MISS")),
            new Newtonsoft.Json.Linq.JProperty("timestamp",        System.DateTime.UtcNow.ToString("o"))
        ).ToString();
        }</message>
            <metadata name="tenant-id" value="@(context.Subscription.Id)" />
            <metadata name="cache-hit" value="@(context.Response.Headers.GetValueOrDefault("X-Cache", "MISS"))" />
        </trace>
    </outbound>
    <on-error>
        <base />
        <set-header name="X-Error-Source" exists-action="override">
            <value>APIM</value>
        </set-header>
    </on-error>
</policies>
XML

  depends_on = [azurerm_api_management_logger.appinsights]
}

# ── Product (Standard tier) ───────────────────────────────────────────────────
resource "azurerm_api_management_product" "standard" {
  product_id            = "standard"
  api_management_name   = azurerm_api_management.main.name
  resource_group_name   = azurerm_resource_group.main.name
  display_name          = "Standard Tier"
  description           = "Standard access to the MCP AI Agent API. Rate limited to ${var.apim_rate_limit_calls} calls per ${var.apim_rate_limit_period_seconds} seconds per subscription."
  subscription_required = true
  approval_required     = false
  published             = true
  subscriptions_limit   = 100
}

# ── Link API → Product ────────────────────────────────────────────────────────
resource "azurerm_api_management_product_api" "standard" {
  api_name            = azurerm_api_management_api.ai_agent.name
  product_id          = azurerm_api_management_product.standard.product_id
  api_management_name = azurerm_api_management.main.name
  resource_group_name = azurerm_resource_group.main.name
}

# ── Tenant Subscriptions ──────────────────────────────────────────────────────
# Creates one subscription per entry in var.apim_tenant_subscriptions.
# Each gets a unique subscription key and its own rate limit counter.
resource "azurerm_api_management_subscription" "tenants" {
  for_each = { for t in var.apim_tenant_subscriptions : t.name => t }

  api_management_name = azurerm_api_management.main.name
  resource_group_name = azurerm_resource_group.main.name
  product_id          = azurerm_api_management_product.standard.id
  display_name        = each.value.display_name
  state               = "active"
  allow_tracing       = true

  depends_on = [azurerm_api_management_product_api.standard]
}
