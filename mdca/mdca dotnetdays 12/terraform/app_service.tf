################################################################################
# app_service.tf
# App Service Plan + MCP Server Web App + AI Agent Web App
#
# Naming conventions:
#   App Service Plan: asp-<suffix>
#   Web App:          app-<role>-<suffix>
#
# All app settings injected here — no Portal config needed after `terraform apply`.
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
    # OpenAI connection — primary model used by default
    "AzureOpenAI__Endpoint"                      = azurerm_cognitive_account.openai.endpoint
    "AzureOpenAI__Key"                           = azurerm_cognitive_account.openai.primary_access_key
    "AzureOpenAI__DeploymentName"                = var.openai_primary_deployment_name
    "AzureOpenAI__MiniDeploymentName"            = var.openai_mini_deployment_name

    # MCP Server URL
    "McpServer__Url"                             = "https://${azurerm_windows_web_app.mcp_server.default_hostname}"

    # Application Insights
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
    azurerm_cognitive_deployment.primary,
  ]
}
