################################################################################
# variables.tf
# All inputs documented. Sensible defaults let you run `terraform apply`
# with minimal changes. Override in terraform.tfvars.
#
# Model versions confirmed available in East US as of June 2026:
#   gpt-4.1        2025-04-14   Deprecating  (safe for workshops)
#   gpt-4.1-mini   2025-04-14   Deprecating  (safe for workshops)
#   gpt-5.1        2025-11-13   GA ✅        (upgrade path)
#   gpt-5-mini     2025-08-07   GA ✅        (upgrade path)
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
  description = "Azure region. East US recommended for best model availability."
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

# ── Primary model (gpt-4.1 by default, upgrade to gpt-5.1 in tfvars) ──────────

variable "openai_primary_deployment_name" {
  type        = string
  description = "Name of the primary model deployment. Referenced in AI Agent app settings as AzureOpenAI__DeploymentName."
  default     = "primary-model"
}

variable "openai_primary_model_name" {
  type        = string
  description = "Primary model name. Options: gpt-4.1 (safe) or gpt-5.1 (latest GA)."
  default     = "gpt-4.1"
}

variable "openai_primary_model_version" {
  type        = string
  description = "Primary model version. gpt-4.1=2025-04-14, gpt-5.1=2025-11-13"
  default     = "2025-04-14"
}

variable "openai_primary_capacity" {
  type        = number
  description = "Primary model throughput in thousands of tokens per minute. 3 = 3K TPM (max for Standard in East US)."
  default     = 3
}

# ── Mini/cheap model (gpt-4.1-mini by default, upgrade to gpt-5-mini) ─────────

variable "openai_mini_deployment_name" {
  type        = string
  description = "Name of the cheaper/faster model deployment. Used in the bonus cost-comparison demo."
  default     = "mini-model"
}

variable "openai_mini_model_name" {
  type        = string
  description = "Mini model name. Options: gpt-4.1-mini (safe) or gpt-5-mini (latest GA)."
  default     = "gpt-4.1-mini"
}

variable "openai_mini_model_version" {
  type        = string
  description = "Mini model version. gpt-4.1-mini=2025-04-14, gpt-5-mini=2025-08-07"
  default     = "2025-04-14"
}

variable "openai_mini_capacity" {
  type        = number
  description = "Mini model throughput in thousands of tokens per minute."
  default     = 3
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
