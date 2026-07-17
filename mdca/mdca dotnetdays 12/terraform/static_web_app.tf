################################################################################
# static_web_app.tf
# Static Web App for the workshop landing page.
#
# We provision the Azure resource in Terraform, but we do not bind the GitHub
# repository here. Deployment uses the SWA API token from GitHub Actions so we
# avoid storing GitHub PATs in Terraform state.
################################################################################

resource "azurerm_static_web_app" "frontend" {
  name                = "swa-${local.name_suffix}"
  resource_group_name = azurerm_resource_group.main.name
  location            = lower(var.static_web_app_location)
  sku_tier            = "Free"
  sku_size            = "Free"
  tags                = local.common_tags
}