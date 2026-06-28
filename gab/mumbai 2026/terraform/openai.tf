################################################################################
# openai.tf
# Azure OpenAI Cognitive Account + two model deployments
#
# Naming convention: cog-<suffix>
#
# Confirmed available in East US as of June 2026 (from az cognitiveservices
# account list-models). MaxCapacity = 3 for Standard SKU in this region.
#
# DEFAULT SETUP (gpt-4.1 family):
#   Primary : gpt-4.1       2025-04-14  Deprecating
#   Mini    : gpt-4.1-mini  2025-04-14  Deprecating
#
# TO UPGRADE TO GPT-5.x — change these two blocks in terraform.tfvars:
#   openai_primary_model_name    = "gpt-5.1"
#   openai_primary_model_version = "2025-11-13"
#   openai_mini_model_name       = "gpt-5-mini"
#   openai_mini_model_version    = "2025-08-07"
#
# NOTE: gpt-5.x models may require a higher subscription quota tier.
#       If you get a quota error, use gpt-4.1 / gpt-4.1-mini instead.
################################################################################

resource "azurerm_cognitive_account" "openai" {
  name                  = "cog-${local.name_suffix}"
  location              = azurerm_resource_group.main.location
  resource_group_name   = azurerm_resource_group.main.name
  kind                  = "OpenAI"
  sku_name              = var.openai_sku
  custom_subdomain_name = "cog-${local.name_suffix}"

  # Public access required for App Service → OpenAI in this lab.
  # Use private endpoints + Managed Identity in production.
  public_network_access_enabled = true

  tags = local.common_tags
}

# ── Primary model deployment ──────────────────────────────────────────────────
# Default: gpt-4.1 (2025-04-14)
# Switch to gpt-5.1 (2025-11-13) in terraform.tfvars for the latest GA model.
resource "azurerm_cognitive_deployment" "primary" {
  name                 = var.openai_primary_deployment_name
  cognitive_account_id = azurerm_cognitive_account.openai.id

  model {
    format  = "OpenAI"
    name    = var.openai_primary_model_name
    version = var.openai_primary_model_version
  }

  scale {
    type     = "Standard"
    capacity = var.openai_primary_capacity
  }
}

# ── Mini / cheaper model deployment ──────────────────────────────────────────
# Default: gpt-4.1-mini (2025-04-14)
# Switch to gpt-5-mini (2025-08-07) in terraform.tfvars for latest GA mini.
# This deployment powers the cost-comparison bonus demo.
resource "azurerm_cognitive_deployment" "mini" {
  name                 = var.openai_mini_deployment_name
  cognitive_account_id = azurerm_cognitive_account.openai.id

  model {
    format  = "OpenAI"
    name    = var.openai_mini_model_name
    version = var.openai_mini_model_version
  }

  scale {
    type     = "GlobalStandard"
    capacity = var.openai_mini_capacity
  }

  # Deploy mini after primary to avoid simultaneous capacity allocation conflicts
  depends_on = [azurerm_cognitive_deployment.primary]
}
