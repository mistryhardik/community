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
#   edit terraform.tfvars with your values
#   terraform init
#   terraform plan
#   terraform apply
#
# NOTE: APIM takes 30-45 minutes to provision. This is normal.
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
      # Prevent accidental deletion of non-empty resource groups
      prevent_deletion_if_contains_resources = false
    }
    cognitive_account {
      purge_soft_delete_on_destroy = true
    }
  }
}

# ── Random suffix ─────────────────────────────────────────────────────────────
# Ensures globally unique names for resources like App Service and APIM.
# Keep this in state — do not taint unless you want new names for everything.
resource "random_string" "suffix" {
  length  = 4
  special = false
  upper   = false
  numeric = true
}

# ── Local values ──────────────────────────────────────────────────────────────
locals {
  # Short suffix used in every resource name: <project>-<env>-<region_short>-<random>
  # Example: aiagent-dev-eus-a3b1
  name_suffix = "${var.project_name}-${var.environment}-${var.location_short}-${random_string.suffix.result}"

  # Merged tags applied to every resource
  common_tags = merge(var.tags, {
    project        = var.project_name
    environment    = var.environment
    managed-by     = "terraform"
    community-event = "DOTNET DAYS AHMEDABAD"
    repository     = "github.com/mistryhardik/community"
  })
}

# ── Resource Group ─────────────────────────────────────────────────────────────
# Naming convention: rg-<workload>-<env>-<region>
resource "azurerm_resource_group" "main" {
  name     = "rg-${local.name_suffix}"
  location = var.location
  tags     = local.common_tags
}
