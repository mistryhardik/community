#!/usr/bin/env bash
# =============================================================================
# setup-terraform.sh
# HackerSpace Mumbai · Global Azure 2026
#
# Run this from your repo root:
#   cd /Users/hardik/source/community-1/gab/mumbai\ 2026
#   bash setup-terraform.sh
#
# Creates:
#   terraform/
#   ├── .gitignore
#   ├── README.md
#   ├── main.tf
#   ├── variables.tf
#   ├── outputs.tf
#   ├── monitoring.tf
#   ├── openai.tf
#   ├── app_service.tf
#   ├── apim.tf
#   └── terraform.tfvars.example
# =============================================================================

set -e

TERRAFORM_DIR="$(pwd)/terraform"

echo ""
echo "🚀 Setting up Terraform folder at: $TERRAFORM_DIR"
echo ""

mkdir -p "$TERRAFORM_DIR"

# =============================================================================
# .gitignore
# =============================================================================
cat > "$TERRAFORM_DIR/.gitignore" << 'GITIGNORE'
# Terraform state — never commit these
.terraform/
.terraform.lock.hcl
terraform.tfstate
terraform.tfstate.backup
*.tfstate
*.tfstate.*

# Variable files contain sensitive values (API keys, emails)
terraform.tfvars
*.auto.tfvars

# Crash log files
crash.log
crash.*.log

# Override files
override.tf
override.tf.json
*_override.tf
*_override.tf.json

# Plan output files
*.tfplan
plan.out
GITIGNORE

echo "  ✅  .gitignore"

# =============================================================================
# main.tf
# =============================================================================
cat > "$TERRAFORM_DIR/main.tf" << 'MAINTF'
################################################################################
# Azure AI Agent Workshop — Terraform
# HackerSpace Mumbai · Global Azure 2026
# Author: Hardik Mistry, Principal Architect
#
# Resources provisioned:
#   - Resource Group
#   - Log Analytics Workspace
#   - Application Insights
#   - Azure OpenAI (gpt-4o + gpt-4o-mini deployments)
#   - App Service Plan
#   - App Service: MCP Server  (.NET 8)
#   - App Service: AI Agent    (.NET 8)
#   - API Management           (Developer tier by default)
#   - APIM Logger              (linked to App Insights)
#
# Usage:
#   cp terraform.tfvars.example terraform.tfvars
#   edit terraform.tfvars
#   terraform init
#   terraform plan
#   terraform apply
#
# NOTE: APIM takes 30-45 minutes to provision. This is expected.
################################################################################

terraform {
  required_version = ">= 1.5.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.110.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.6.0"
    }
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
    cognitive_account {
      purge_soft_delete_on_destroy = true
    }
  }
}

# ── Random suffix ─────────────────────────────────────────────────────────────
# Ensures globally unique names for App Service, APIM, etc.
# Do not taint this unless you want entirely new resource names.
resource "random_string" "suffix" {
  length  = 4
  special = false
  upper   = false
  numeric = true
}

# ── Locals ────────────────────────────────────────────────────────────────────
locals {
  # Example result: aiagent-dev-eus-a3b1
  name_suffix = "${var.project_name}-${var.environment}-${var.location_short}-${random_string.suffix.result}"

  common_tags = merge(var.tags, {
    project         = var.project_name
    environment     = var.environment
    managed-by      = "terraform"
    community-event = "DOTNET DAYS AHMEDABAD"
    repository      = "github.com/mistryhardik/community"
  })
}

# ── Resource Group ─────────────────────────────────────────────────────────────
# Naming convention: rg-<workload>-<env>-<region>-<suffix>
resource "azurerm_resource_group" "main" {
  name     = "rg-${local.name_suffix}"
  location = var.location
  tags     = local.common_tags
}
MAINTF

echo "  ✅  main.tf"

# =============================================================================
# variables.tf
# =============================================================================
cat > "$TERRAFORM_DIR/variables.tf" << 'VARIABLESTF'
################################################################################
# variables.tf
# All inputs documented. Sensible defaults let you run `terraform apply`
# with minimal changes. Override in terraform.tfvars.
################################################################################

# ── Project identity ──────────────────────────────────────────────────────────

variable "project_name" {
  type        = string
  description = "Short workload name used in every resource name. Lowercase letters and hyphens only. Max 8 chars."
  default     = "aiagent"

  validation {
    condition     = can(regex("^[a-z][a-z0-9-]{1,7}$", var.project_name))
    error_message = "project_name must be 2-8 lowercase alphanumeric characters or hyphens, starting with a letter."
  }
}

variable "environment" {
  type        = string
  description = "Deployment environment. Used in resource names and tags."
  default     = "dev"

  validation {
    condition     = contains(["dev", "test", "staging", "prod"], var.environment)
    error_message = "environment must be one of: dev, test, staging, prod."
  }
}

variable "location" {
  type        = string
  description = "Azure region. East US recommended for best GPT-4o availability."
  default     = "eastus"
}

variable "location_short" {
  type        = string
  description = "Short region code used in resource names (e.g. eus for eastus). Keep to 3-4 chars."
  default     = "eus"
}

# ── Tags ──────────────────────────────────────────────────────────────────────

variable "tags" {
  type        = map(string)
  description = "Additional tags merged with common tags on every resource."
  default     = {}
}

# ── Azure OpenAI ──────────────────────────────────────────────────────────────

variable "openai_sku" {
  type        = string
  description = "Azure OpenAI pricing tier."
  default     = "S0"
}

variable "openai_gpt4o_deployment_name" {
  type        = string
  description = "GPT-4o deployment name. Referenced in the AI Agent app settings."
  default     = "gpt-4o-demo"
}

variable "openai_gpt4o_version" {
  type        = string
  description = "GPT-4o model version. Check regional availability before changing."
  default     = "2024-11-20"
}

variable "openai_gpt4o_capacity" {
  type        = number
  description = "GPT-4o throughput in thousands of tokens per minute. 10 = 10K TPM."
  default     = 10
}

variable "openai_gpt4o_mini_deployment_name" {
  type        = string
  description = "GPT-4o-mini deployment name."
  default     = "gpt-4o-mini-demo"
}

variable "openai_gpt4o_mini_version" {
  type        = string
  description = "GPT-4o-mini model version."
  default     = "2025-04-14"
}

variable "openai_gpt4o_mini_capacity" {
  type        = number
  description = "GPT-4o-mini throughput in thousands of tokens per minute."
  default     = 10
}

# ── App Service ───────────────────────────────────────────────────────────────

variable "app_service_sku" {
  type        = string
  description = "App Service Plan SKU. B1 minimum for always-on. F1 is free but has cold-start limitations."
  default     = "B1"

  validation {
    condition     = contains(["F1", "B1", "B2", "B3", "S1", "S2", "P1v3", "P2v3"], var.app_service_sku)
    error_message = "app_service_sku must be a valid App Service SKU."
  }
}

variable "dotnet_version" {
  type        = string
  description = ".NET framework version for both App Services."
  default     = "v8.0"
}

# ── API Management ────────────────────────────────────────────────────────────

variable "apim_sku" {
  type        = string
  description = "APIM pricing tier. Developer has no SLA. Use Basic or Standard for production. Takes 30-45 min to provision."
  default     = "Developer_1"

  validation {
    condition     = can(regex("^(Developer|Basic|Standard|Premium)_[0-9]+$", var.apim_sku))
    error_message = "apim_sku must be in the format Tier_Capacity (e.g. Developer_1, Basic_1)."
  }
}

variable "apim_publisher_name" {
  type        = string
  description = "Publisher name shown in the APIM Developer Portal."
  default     = "HackerSpace Mumbai"
}

variable "apim_publisher_email" {
  type        = string
  description = "Publisher email. Required by APIM for notifications."
  default     = "admin@example.com"
}

variable "apim_rate_limit_calls" {
  type        = number
  description = "API calls allowed per tenant per rate limit window."
  default     = 20
}

variable "apim_rate_limit_period_seconds" {
  type        = number
  description = "Rate limit window in seconds."
  default     = 60
}

variable "apim_cache_duration_seconds" {
  type        = number
  description = "Response cache TTL in seconds. 300 = 5 minutes."
  default     = 300
}

# ── Tenant subscriptions ──────────────────────────────────────────────────────

variable "apim_tenant_subscriptions" {
  type = list(object({
    name         = string
    display_name = string
  }))
  description = "Tenant subscriptions to create. Each gets its own API key and rate limit counter."
  default = [
    { name = "tenant-a", display_name = "Tenant A — Workshop Member" },
    { name = "tenant-b", display_name = "Tenant B — Workshop Member" },
  ]
}

# ── Monitoring ────────────────────────────────────────────────────────────────

variable "log_analytics_sku" {
  type        = string
  description = "Log Analytics Workspace SKU."
  default     = "PerGB2018"
}

variable "log_analytics_retention_days" {
  type        = number
  description = "Log retention in days."
  default     = 30
}
VARIABLESTF

echo "  ✅  variables.tf"

# =============================================================================
# monitoring.tf
# =============================================================================
cat > "$TERRAFORM_DIR/monitoring.tf" << 'MONITORINGTF'
################################################################################
# monitoring.tf
# Log Analytics Workspace + Application Insights
#
# Naming conventions:
#   Log Analytics:        log-<suffix>
#   Application Insights: appi-<suffix>
################################################################################

resource "azurerm_log_analytics_workspace" "main" {
  name                = "log-${local.name_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = var.log_analytics_sku
  retention_in_days   = var.log_analytics_retention_days
  tags                = local.common_tags
}

resource "azurerm_application_insights" "main" {
  name                = "appi-${local.name_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
  tags                = local.common_tags
}
MONITORINGTF

echo "  ✅  monitoring.tf"

# =============================================================================
# openai.tf
# =============================================================================
cat > "$TERRAFORM_DIR/openai.tf" << 'OPENAITF'
################################################################################
# openai.tf
# Azure OpenAI Cognitive Account + GPT-4o and GPT-4o-mini deployments
#
# Naming convention: cog-<suffix>
#
# Notes:
#   - eastus has the best model availability.
#   - capacity = TPM ÷ 1000. capacity=10 means 10K TPM.
#   - purge_soft_delete_on_destroy = true (set in provider) handles clean teardown.
################################################################################

resource "azurerm_cognitive_account" "openai" {
  name                  = "cog-${local.name_suffix}"
  location              = azurerm_resource_group.main.location
  resource_group_name   = azurerm_resource_group.main.name
  kind                  = "OpenAI"
  sku_name              = var.openai_sku
  custom_subdomain_name = "cog-${local.name_suffix}"

  # Public access required for App Service → OpenAI calls in this lab.
  # Use private endpoints in production.
  public_network_access_enabled = true

  tags = local.common_tags
}

resource "azurerm_cognitive_deployment" "gpt4o" {
  name                 = var.openai_gpt4o_deployment_name
  cognitive_account_id = azurerm_cognitive_account.openai.id

  model {
    format  = "OpenAI"
    name    = "gpt-4o"
    version = var.openai_gpt4o_version
  }

  scale {
    type     = "Standard"
    capacity = var.openai_gpt4o_capacity
  }
}

resource "azurerm_cognitive_deployment" "gpt4o_mini" {
  name                 = var.openai_gpt4o_mini_deployment_name
  cognitive_account_id = azurerm_cognitive_account.openai.id

  model {
    format  = "OpenAI"
    name    = "gpt-4o-mini"
    version = var.openai_gpt4o_mini_version
  }

  scale {
    type     = "Standard"
    capacity = var.openai_gpt4o_mini_capacity
  }

  # Deploy mini after gpt-4o to avoid capacity conflicts
  depends_on = [azurerm_cognitive_deployment.gpt4o]
}
OPENAITF

echo "  ✅  openai.tf"

# =============================================================================
# app_service.tf
# =============================================================================
cat > "$TERRAFORM_DIR/app_service.tf" << 'APPSERVICETF'
################################################################################
# app_service.tf
# App Service Plan + MCP Server Web App + AI Agent Web App
#
# Naming conventions:
#   App Service Plan: asp-<suffix>
#   Web App:          app-<role>-<suffix>
#
# App settings (env vars) are injected here so members never need
# to open the Portal after `terraform apply`.
################################################################################

resource "azurerm_service_plan" "main" {
  name                = "asp-${local.name_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  os_type             = "Windows"
  sku_name            = var.app_service_sku
  tags                = local.common_tags
}

# ── MCP Server ────────────────────────────────────────────────────────────────
resource "azurerm_windows_web_app" "mcp_server" {
  name                = "app-mcpsrv-${local.name_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true
  tags                = local.common_tags

  site_config {
    always_on = var.app_service_sku != "F1"

    application_stack {
      current_stack  = "dotnet"
      dotnet_version = var.dotnet_version
    }

    cors {
      allowed_origins = ["*"]
    }
  }

  app_settings = {
    "APPLICATIONINSIGHTS_CONNECTION_STRING"      = azurerm_application_insights.main.connection_string
    "ApplicationInsightsAgent_EXTENSION_VERSION" = "~3"
    "ASPNETCORE_ENVIRONMENT"                     = var.environment == "prod" ? "Production" : "Development"
    "APP_ROLE"                                   = "McpServer"
  }

  logs {
    application_logs {
      file_system_level = "Information"
    }
    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 35
      }
    }
  }

  identity {
    type = "SystemAssigned"
  }
}

# ── AI Agent ──────────────────────────────────────────────────────────────────
resource "azurerm_windows_web_app" "ai_agent" {
  name                = "app-aiagent-${local.name_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true
  tags                = local.common_tags

  site_config {
    always_on = var.app_service_sku != "F1"

    application_stack {
      current_stack  = "dotnet"
      dotnet_version = var.dotnet_version
    }
  }

  app_settings = {
    "AzureOpenAI__Endpoint"                      = azurerm_cognitive_account.openai.endpoint
    "AzureOpenAI__Key"                           = azurerm_cognitive_account.openai.primary_access_key
    "AzureOpenAI__DeploymentName"                = var.openai_gpt4o_deployment_name
    "McpServer__Url"                             = "https://${azurerm_windows_web_app.mcp_server.default_hostname}"
    "APPLICATIONINSIGHTS_CONNECTION_STRING"      = azurerm_application_insights.main.connection_string
    "ApplicationInsightsAgent_EXTENSION_VERSION" = "~3"
    "ASPNETCORE_ENVIRONMENT"                     = var.environment == "prod" ? "Production" : "Development"
    "APP_ROLE"                                   = "AiAgent"
  }

  logs {
    application_logs {
      file_system_level = "Information"
    }
    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 35
      }
    }
  }

  identity {
    type = "SystemAssigned"
  }

  depends_on = [
    azurerm_windows_web_app.mcp_server,
    azurerm_cognitive_account.openai,
    azurerm_cognitive_deployment.gpt4o,
  ]
}
APPSERVICETF

echo "  ✅  app_service.tf"

# =============================================================================
# apim.tf
# =============================================================================
cat > "$TERRAFORM_DIR/apim.tf" << 'APIMTF'
################################################################################
# apim.tf
# Azure API Management — full setup
#
# Naming convention: apim-<suffix>
#
# Provisions:
#   1. APIM service instance
#   2. Application Insights logger
#   3. Diagnostic settings (100% sampling)
#   4. Backend (AI Agent App Service)
#   5. API definition + POST /api/chat operation
#   6. Inbound + Outbound policy:
#        - Subscription key check
#        - Per-tenant rate limiting
#        - Response caching (keyed on request body hash)
#        - System prompt enforcement at gateway
#        - Token usage trace to Application Insights
#   7. Standard product
#   8. Tenant subscriptions (one per var.apim_tenant_subscriptions entry)
#
# NOTE: APIM takes 30-45 minutes to provision. terraform apply will wait.
################################################################################

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

  timeouts {
    create = "90m"
    update = "90m"
    delete = "90m"
  }
}

# ── Logger ────────────────────────────────────────────────────────────────────
resource "azurerm_api_management_logger" "appinsights" {
  name                = "appinsights-logger"
  api_management_name = azurerm_api_management.main.name
  resource_group_name = azurerm_resource_group.main.name
  resource_id         = azurerm_application_insights.main.id

  application_insights {
    instrumentation_key = azurerm_application_insights.main.instrumentation_key
  }
}

# ── Diagnostics ───────────────────────────────────────────────────────────────
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
    body_bytes     = 1024
    headers_to_log = ["Content-Type", "Ocp-Apim-Subscription-Key"]
  }

  frontend_response {
    body_bytes     = 1024
    headers_to_log = ["Content-Type", "X-Cache", "X-RateLimit-Remaining"]
  }

  backend_request  { body_bytes = 1024 }
  backend_response { body_bytes = 1024 }
}

# ── Backend ───────────────────────────────────────────────────────────────────
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

# ── API ───────────────────────────────────────────────────────────────────────
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

# ── Operation: POST /api/chat ─────────────────────────────────────────────────
resource "azurerm_api_management_api_operation" "chat" {
  operation_id        = "post-chat"
  api_name            = azurerm_api_management_api.ai_agent.name
  api_management_name = azurerm_api_management.main.name
  resource_group_name = azurerm_resource_group.main.name
  display_name        = "Chat"
  method              = "POST"
  url_template        = "/api/chat"
  description         = "Send a message to the MCP-powered AI agent."

  request {
    description = "Chat request"
    representation {
      content_type = "application/json"
      example {
        name  = "default"
        value = jsonencode({ message = "What is Azure API Management?", model = "gpt-4o-demo" })
      }
    }
  }

  response {
    status_code = 200
    description = "Successful response with token usage"
    representation {
      content_type = "application/json"
      example {
        name  = "default"
        value = jsonencode({
          reply = "Azure API Management is...", model = "gpt-4o-demo",
          toolsCalled = [], promptTokens = 28, completionTokens = 56, totalTokens = 84
        })
      }
    }
  }

  response { status_code = 429; description = "Rate limit exceeded" }
  response { status_code = 401; description = "Missing or invalid subscription key" }
}

# ── Policy ────────────────────────────────────────────────────────────────────
resource "azurerm_api_management_api_policy" "ai_agent" {
  api_name            = azurerm_api_management_api.ai_agent.name
  api_management_name = azurerm_api_management.main.name
  resource_group_name = azurerm_resource_group.main.name

  xml_content = <<-XML
    <policies>
      <inbound>
        <base />

        <!-- 1. Validate subscription key -->
        <check-header
          name="Ocp-Apim-Subscription-Key"
          failed-check-httpcode="401"
          failed-check-error-message="Missing or invalid subscription key."
          ignore-case="false" />

        <!-- 2. Per-tenant rate limiting -->
        <rate-limit-by-key
          calls="${var.apim_rate_limit_calls}"
          renewal-period="${var.apim_rate_limit_period_seconds}"
          counter-key="@(context.Subscription.Id)"
          remaining-calls-header-name="X-RateLimit-Remaining"
          remaining-calls-variable-name="remainingCalls"
          retry-after-header-name="Retry-After" />

        <!-- 3. Cache lookup keyed on request body hash -->
        <cache-lookup-value
          key="@(&quot;mcp-&quot; + context.Request.Body.As&lt;string&gt;(true).GetHashCode())"
          variable-name="cachedResponse" />
        <choose>
          <when condition="@(context.Variables.ContainsKey(&quot;cachedResponse&quot;))">
            <return-response>
              <set-status code="200" reason="OK" />
              <set-header name="Content-Type" exists-action="override">
                <value>application/json</value>
              </set-header>
              <set-header name="X-Cache" exists-action="override">
                <value>HIT</value>
              </set-header>
              <set-body>@((string)context.Variables[&quot;cachedResponse&quot;])</set-body>
            </return-response>
          </when>
        </choose>

        <!-- 4. Mark cache miss -->
        <set-header name="X-Cache" exists-action="override">
          <value>MISS</value>
        </set-header>

        <!-- 5. Enforce system prompt at gateway if not provided by caller -->
        <set-body>@{
          var body = context.Request.Body.As&lt;Newtonsoft.Json.Linq.JObject&gt;(true);
          if (body["systemPrompt"] == null)
            body["systemPrompt"] = "You are a concise Azure technical assistant. "
              + "Use available tools when asked about costs, quotas, or service health. "
              + "Answer in 2-3 plain sentences. No bullet points unless asked.";
          return body.ToString();
        }</set-body>

        <set-backend-service backend-id="ai-agent-backend" />
      </inbound>

      <backend><base /></backend>

      <outbound>
        <base />

        <!-- 6. Cache response -->
        <cache-store-value
          key="@(&quot;mcp-&quot; + context.Request.Body.As&lt;string&gt;(true).GetHashCode())"
          value="@(context.Response.Body.As&lt;string&gt;(true))"
          duration="${var.apim_cache_duration_seconds}" />

        <!-- 7. Emit per-tenant token usage to Application Insights -->
        <trace source="mcp-token-usage" severity="information">
          <message>@{
            var body = context.Response.Body.As&lt;Newtonsoft.Json.Linq.JObject&gt;(true);
            return new Newtonsoft.Json.Linq.JObject(
              new Newtonsoft.Json.Linq.JProperty("tenantId",         context.Subscription.Id),
              new Newtonsoft.Json.Linq.JProperty("tenantName",       context.Subscription.Name),
              new Newtonsoft.Json.Linq.JProperty("totalTokens",      body?["totalTokens"]),
              new Newtonsoft.Json.Linq.JProperty("promptTokens",     body?["promptTokens"]),
              new Newtonsoft.Json.Linq.JProperty("completionTokens", body?["completionTokens"]),
              new Newtonsoft.Json.Linq.JProperty("toolsCalled",      body?["toolsCalled"]),
              new Newtonsoft.Json.Linq.JProperty("cacheHit",         context.Response.Headers.GetValueOrDefault("X-Cache", "MISS")),
              new Newtonsoft.Json.Linq.JProperty("model",            body?["model"]),
              new Newtonsoft.Json.Linq.JProperty("timestamp",        System.DateTime.UtcNow.ToString("o"))
            ).ToString();
          }</message>
          <metadata name="tenant" value="@(context.Subscription.Name)" />
          <metadata name="cache"  value="@(context.Response.Headers.GetValueOrDefault(&quot;X-Cache&quot;,&quot;MISS&quot;))" />
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

# ── Product ───────────────────────────────────────────────────────────────────
resource "azurerm_api_management_product" "standard" {
  product_id            = "standard"
  api_management_name   = azurerm_api_management.main.name
  resource_group_name   = azurerm_resource_group.main.name
  display_name          = "Standard Tier"
  description           = "Standard access. Rate limited to ${var.apim_rate_limit_calls} calls per ${var.apim_rate_limit_period_seconds}s per subscription."
  subscription_required = true
  approval_required     = false
  published             = true
  subscriptions_limit   = 100
}

resource "azurerm_api_management_product_api" "standard" {
  api_name            = azurerm_api_management_api.ai_agent.name
  product_id          = azurerm_api_management_product.standard.product_id
  api_management_name = azurerm_api_management.main.name
  resource_group_name = azurerm_resource_group.main.name
}

# ── Tenant Subscriptions (one per entry in var.apim_tenant_subscriptions) ─────
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
APIMTF

echo "  ✅  apim.tf"

# =============================================================================
# outputs.tf
# =============================================================================
cat > "$TERRAFORM_DIR/outputs.tf" << 'OUTPUTSTF'
################################################################################
# outputs.tf
# Everything printed after `terraform apply` — all the URLs and keys needed
# to test the demo without opening the Portal.
################################################################################

output "resource_group_name" {
  description = "Resource group name. Use in the cleanup command."
  value       = azurerm_resource_group.main.name
}

output "openai_endpoint" {
  description = "Azure OpenAI endpoint URL."
  value       = azurerm_cognitive_account.openai.endpoint
}

output "openai_key" {
  description = "Azure OpenAI primary access key."
  value       = azurerm_cognitive_account.openai.primary_access_key
  sensitive   = true
}

output "openai_gpt4o_deployment" {
  description = "GPT-4o deployment name."
  value       = var.openai_gpt4o_deployment_name
}

output "openai_gpt4o_mini_deployment" {
  description = "GPT-4o-mini deployment name."
  value       = var.openai_gpt4o_mini_deployment_name
}

output "mcp_server_url" {
  description = "MCP Server URL. Health check: <url>/health"
  value       = "https://${azurerm_windows_web_app.mcp_server.default_hostname}"
}

output "ai_agent_url" {
  description = "AI Agent URL. Chat endpoint: <url>/api/chat"
  value       = "https://${azurerm_windows_web_app.ai_agent.default_hostname}"
}

output "ai_agent_swagger_url" {
  description = "Swagger UI for the AI Agent."
  value       = "https://${azurerm_windows_web_app.ai_agent.default_hostname}/swagger"
}

output "app_insights_connection_string" {
  description = "Application Insights connection string. Already injected into both App Services."
  value       = azurerm_application_insights.main.connection_string
  sensitive   = true
}

output "apim_gateway_url" {
  description = "APIM Gateway URL. All API calls go through here."
  value       = azurerm_api_management.main.gateway_url
}

output "apim_developer_portal_url" {
  description = "APIM Developer Portal."
  value       = azurerm_api_management.main.developer_portal_url
}

output "apim_chat_endpoint" {
  description = "Full chat endpoint URL through APIM."
  value       = "${azurerm_api_management.main.gateway_url}/agent/api/chat"
}

output "tenant_subscription_keys" {
  description = "Primary keys per tenant. Pass in Ocp-Apim-Subscription-Key header."
  sensitive   = true
  value = {
    for name, sub in azurerm_api_management_subscription.tenants :
    name => sub.primary_key
  }
}

output "quickstart_commands" {
  description = "Ready-to-run curl commands to verify the deployment."
  value       = <<-EOT

    ═══════════════════════════════════════════════════════════
      QUICK START — copy-paste these to verify your deployment
    ═══════════════════════════════════════════════════════════

    1. Get tenant keys:
       terraform output -json tenant_subscription_keys

    2. Health checks:
       curl https://${azurerm_windows_web_app.mcp_server.default_hostname}/health
       curl https://${azurerm_windows_web_app.ai_agent.default_hostname}/health

    3. Chat through APIM:
       curl -X POST ${azurerm_api_management.main.gateway_url}/agent/api/chat \
         -H "Content-Type: application/json" \
         -H "Ocp-Apim-Subscription-Key: <TENANT_A_KEY>" \
         -d '{"message": "What is Azure APIM?"}'

    4. Tool-calling test:
       curl -X POST ${azurerm_api_management.main.gateway_url}/agent/api/chat \
         -H "Content-Type: application/json" \
         -H "Ocp-Apim-Subscription-Key: <TENANT_A_KEY>" \
         -d '{"message": "I used 500 prompt tokens and 300 completion tokens with gpt-4o. What did that cost?"}'

    5. Rate limit burst test (expects 429 after call 20):
       for i in $(seq 1 25); do
         curl -s -o /dev/null -w "Call $i: HTTP %{http_code}\n" \
           -X POST ${azurerm_api_management.main.gateway_url}/agent/api/chat \
           -H "Content-Type: application/json" \
           -H "Ocp-Apim-Subscription-Key: <TENANT_A_KEY>" \
           -d '{"message":"Hello"}'
       done

    6. Cleanup:
       az group delete --name ${azurerm_resource_group.main.name} --yes --no-wait

    ═══════════════════════════════════════════════════════════
  EOT
}
OUTPUTSTF

echo "  ✅  outputs.tf"

# =============================================================================
# terraform.tfvars.example
# =============================================================================
cat > "$TERRAFORM_DIR/terraform.tfvars.example" << 'TFVARSEXAMPLE'
################################################################################
# terraform.tfvars.example
#
# Copy to terraform.tfvars and fill in your values:
#   cp terraform.tfvars.example terraform.tfvars
#
# terraform.tfvars is in .gitignore — never commit it.
################################################################################

# ── Your identity ─────────────────────────────────────────────────────────────
apim_publisher_name  = "HackerSpace Mumbai"
apim_publisher_email = "your-email@example.com"    # <-- change this

# ── Project naming ─────────────────────────────────────────────────────────────
# Controls how every resource is named.
# Result: rg-aiagent-dev-eus-xxxx, app-aiagent-dev-eus-xxxx, etc.
project_name   = "aiagent"    # max 8 chars, lowercase
environment    = "dev"        # dev | test | staging | prod
location       = "eastus"     # East US has best GPT-4o availability
location_short = "eus"        # 3-4 char region code used in names

# ── App Service ───────────────────────────────────────────────────────────────
# B1 = ~$13/month. F1 = free but cold starts and no always-on.
app_service_sku = "B1"

# ── Azure OpenAI ──────────────────────────────────────────────────────────────
# capacity = TPM ÷ 1000. 10 = 10K tokens per minute.
openai_gpt4o_capacity      = 10
openai_gpt4o_mini_capacity = 10

# ── API Management ────────────────────────────────────────────────────────────
# Developer_1 = ~$50/month, no SLA. Use Basic_1 for production.
# NOTE: APIM takes 30-45 minutes to provision.
apim_sku = "Developer_1"

# Rate limiting and caching
apim_rate_limit_calls          = 20
apim_rate_limit_period_seconds = 60
apim_cache_duration_seconds    = 300

# ── Tenant subscriptions ──────────────────────────────────────────────────────
# Each tenant gets an independent API key and rate limit counter.
# Add more entries to create more tenant keys.
apim_tenant_subscriptions = [
  { name = "tenant-a", display_name = "Tenant A — Workshop Member" },
  { name = "tenant-b", display_name = "Tenant B — Workshop Member" },
  # { name = "tenant-c", display_name = "Your Name" },
]

# ── Tags ──────────────────────────────────────────────────────────────────────
tags = {
  owner = "your-name"           # <-- change this
  event = "Global Azure 2026"
}
TFVARSEXAMPLE

echo "  ✅  terraform.tfvars.example"

# =============================================================================
# README.md
# =============================================================================
cat > "$TERRAFORM_DIR/README.md" << 'READMETF'
# Terraform — Azure AI Agent Workshop Infrastructure

Provisions every Azure resource for the **MCP + Azure APIM** workshop in one command.

## Resources created

| Resource | Name pattern | Purpose |
|---|---|---|
| Resource Group | `rg-<project>-<env>-<region>-<random>` | Container for all resources |
| Log Analytics | `log-<suffix>` | Log storage |
| Application Insights | `appi-<suffix>` | Per-tenant token telemetry |
| Azure OpenAI | `cog-<suffix>` | GPT-4o + GPT-4o-mini |
| App Service Plan | `asp-<suffix>` | Shared compute |
| MCP Server App | `app-mcpsrv-<suffix>` | MCP tool server |
| AI Agent App | `app-aiagent-<suffix>` | AI agent with MCP client |
| API Management | `apim-<suffix>` | Rate limiting, caching, auth |
| APIM Subscriptions | one per tenant | Individual API keys |

## Quick start

```bash
# 1. Copy and edit variables
cp terraform.tfvars.example terraform.tfvars
# Set your apim_publisher_email and tags.owner at minimum

# 2. Log in to Azure
az login

# 3. Initialise and apply
terraform init
terraform plan
terraform apply        # APIM takes 30-45 min — this is normal

# 4. Get tenant keys
terraform output -json tenant_subscription_keys

# 5. Cleanup when done
terraform destroy
```

## Deploy your .NET apps after apply

```bash
# MCP Server
cd ../aiagentapi/McpAzureServer
dotnet publish -c Release -o ./publish
cd publish && zip -r ../deploy.zip . && cd ..
az webapp deploy \
  --name $(cd ../../terraform && terraform output -raw mcp_server_url | sed 's|https://||') \
  --resource-group $(cd ../../terraform && terraform output -raw resource_group_name) \
  --src-path deploy.zip

# AI Agent
cd ../McpAiAgent
dotnet publish -c Release -o ./publish
cd publish && zip -r ../deploy.zip . && cd ..
az webapp deploy \
  --name $(cd ../../terraform && terraform output -raw ai_agent_url | sed 's|https://||') \
  --resource-group $(cd ../../terraform && terraform output -raw resource_group_name) \
  --src-path deploy.zip
```

## Estimated cost

| Resource | ~Monthly cost |
|---|---|
| APIM Developer tier | $50 |
| App Service Plan B1 | $13 |
| Azure OpenAI | Pay-per-token |
| App Insights + Log Analytics | Free (first 5 GB) |
| **Total** | **~$63/month** |

> Use `F1` App Service SKU and skip APIM for a fully free environment (with limitations).

---

*HackerSpace Mumbai · Global Azure 2026 · [github.com/mistryhardik/community](https://github.com/mistryhardik/community)*
READMETF

echo "  ✅  README.md"

# =============================================================================
# Done
# =============================================================================
echo ""
echo "═══════════════════════════════════════════════════════════"
echo "  ✅  Done! Terraform folder created at:"
echo "      $TERRAFORM_DIR"
echo ""
echo "  Files created:"
find "$TERRAFORM_DIR" -type f | sort | sed 's|.*/terraform/|      terraform/|'
echo ""
echo "  Next steps:"
echo "    cd terraform"
echo "    cp terraform.tfvars.example terraform.tfvars"
echo "    # Edit terraform.tfvars — set your email + owner tag"
echo "    terraform init"
echo "    terraform plan"
echo "    terraform apply"
echo "═══════════════════════════════════════════════════════════"
echo ""