################################################################################
# monitoring.tf
# Log Analytics Workspace + Application Insights
#
# Naming conventions:
#   Log Analytics:       log-<suffix>
#   Application Insights: appi-<suffix>
################################################################################

# ── Log Analytics Workspace ───────────────────────────────────────────────────
resource "azurerm_log_analytics_workspace" "main" {
  name                = "log-${local.name_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = var.log_analytics_sku
  retention_in_days   = var.log_analytics_retention_days
  tags                = local.common_tags
}

# ── Application Insights ──────────────────────────────────────────────────────
resource "azurerm_application_insights" "main" {
  name                = "appi-${local.name_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
  tags                = local.common_tags
}
