################################################################################
# terraform.tfvars.example
#
# Copy this file to terraform.tfvars and fill in your values.
#
#   cp terraform.tfvars.example terraform.tfvars
#
# terraform.tfvars is in .gitignore — never commit it (contains sensitive values).
################################################################################

# ── Your identity ─────────────────────────────────────────────────────────────
# Used in APIM publisher info and resource tags.
apim_publisher_name  = "APPMATTIC"
apim_publisher_email = "hardik@appmattic.com"   # <-- change this

# ── Project naming ─────────────────────────────────────────────────────────────
# These control what every resource is named.
# Convention: rg-aiagent-dev-eus-xxxx, app-aiagent-dev-eus-xxxx, etc.
project_name   = "amaagent"    # max 8 chars, lowercase
environment    = "dev"        # dev | test | staging | prod
location       = "eastus"     # East US recommended for GPT-4o availability
location_short = "eus"        # short code used in names (eus, weu, uks, etc.)

# ── App Service ───────────────────────────────────────────────────────────────
# B1 = ~$13/month. F1 = free but has cold starts and no always-on.
app_service_sku = "B1"

# ── Azure OpenAI ──────────────────────────────────────────────────────────────
# capacity = TPM ÷ 1000. 10 = 10K tokens per minute (enough for demo).
openai_primary_deployment_name = "gpt-5.1"
openai_primary_model_name      = "gpt-5.1"
openai_primary_model_version   = "2025-11-13"
openai_primary_capacity        = 3

openai_mini_deployment_name    = "gpt-5-mini"
openai_mini_model_name         = "gpt-5-mini"
openai_mini_model_version      = "2025-08-07"
openai_mini_capacity           = 3

# ── API Management ────────────────────────────────────────────────────────────
# Developer_1 = ~$50/month, no SLA. Use Basic_1 for production.
# NOTE: APIM takes 30-45 minutes to provision.
apim_sku = "Developer_1"

# Rate limiting: 20 calls per 60 seconds per tenant
apim_rate_limit_calls          = 20
apim_rate_limit_period_seconds = 60
apim_cache_duration_seconds    = 300   # 5 minutes

# ── Tenant subscriptions ──────────────────────────────────────────────────────
# Add as many tenants as you need. Each gets its own subscription key
# and independent rate limit counter.
apim_tenant_subscriptions = [
  { name = "tenant-a", display_name = "Tenant A — Workshop Member" },
  { name = "tenant-b", display_name = "Tenant B — Workshop Member" },
  # { name = "tenant-c", display_name = "Tenant C — AMA AHMEDABAD" },
]

# ── Tags ──────────────────────────────────────────────────────────────────────
tags = {
  owner = "APPMATTIC"          # <-- change this
  event = "DOTNET DAYS AHMEDABAD"
}
