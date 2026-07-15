# Terraform — Azure AI Agent Workshop Infrastructure

This folder contains Terraform configuration to provision every Azure resource needed for the **MCP + Azure APIM** workshop demo. Run `terraform apply` once and get a fully wired environment — no Portal clicks required.

## What gets created

| Resource | Azure Name Pattern | Purpose |
|---|---|---|
| Resource Group | `rg-<project>-<env>-<region>-<suffix>` | Container for all resources |
| Log Analytics Workspace | `log-<suffix>` | Log storage for App Insights |
| Application Insights | `appi-<suffix>` | Telemetry, per-tenant token usage |
| Azure OpenAI | `cog-<suffix>` | GPT-4o + GPT-4o-mini deployments |
| App Service Plan | `asp-<suffix>` | Shared compute for both apps |
| App Service — MCP Server | `app-mcpsrv-<suffix>` | Hosts your MCP tool server |
| App Service — AI Agent | `app-aiagent-<suffix>` | Hosts your .NET AI agent |
| API Management | `apim-<suffix>` | Rate limiting, caching, auth, tracing |
| APIM Product | `standard` | Access tier that subscriptions are linked to |
| APIM Subscriptions | one per tenant | Individual API keys per tenant |

All names follow [Microsoft's recommended Azure naming conventions](https://learn.microsoft.com/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming).

## File structure

```
terraform/
├── main.tf                   # Provider config, resource group, random suffix, locals
├── variables.tf              # All input variables with descriptions and defaults
├── outputs.tf                # All useful values + quickstart curl commands
├── monitoring.tf             # Log Analytics Workspace + Application Insights
├── openai.tf                 # Azure OpenAI + GPT-4o + GPT-4o-mini deployments
├── app_service.tf            # App Service Plan + MCP Server + AI Agent web apps
├── apim.tf                   # Full APIM: service, logger, API, policy, product, subscriptions
├── terraform.tfvars.example  # Copy this to terraform.tfvars and fill in your values
└── README.md                 # This file
```

## Prerequisites

- [Terraform >= 1.5.0](https://developer.hashicorp.com/terraform/downloads)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) — logged in (`az login`)
- An Azure subscription (see [Step 0 in the main lab guide](../README.md#-step-0--claim-your-azure-credits))

## Quick start

```bash
# 1. Clone the repo and navigate here
cd gab/mumbai\ 2026/terraform

# 2. Copy and edit the variables file
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars — at minimum set apim_publisher_email and tags.owner

# 3. Log in to Azure
az login
az account show  # verify correct subscription

# 4. Initialise Terraform
terraform init

# 5. Preview what will be created (no changes made)
terraform plan

# 6. Apply — grab a coffee, APIM takes 30-45 minutes
terraform apply
# Type 'yes' when prompted

# 7. After apply — get your tenant subscription keys
terraform output -json tenant_subscription_keys

# 8. Test with the quickstart commands printed by terraform apply
```

## After `terraform apply`

The `quickstart_commands` output prints ready-to-run curl commands including:
- Health checks for both App Services
- Basic chat call through APIM
- Tool-calling test (invokes the MCP Server's `CalculateTokenCost` tool)
- Rate limiting burst test (25 calls, expect 429 after call 20)

```bash
# View all outputs
terraform output

# View sensitive outputs
terraform output openai_key
terraform output -json tenant_subscription_keys
terraform output app_insights_connection_string
```

## Deploy your code

After `terraform apply` provisions the infrastructure, deploy the .NET apps:

```bash
# From McpAzureServer/
dotnet publish -c Release -o ./publish
cd publish && zip -r ../deploy.zip . && cd ..
az webapp deploy \
  --name $(terraform output -raw mcp_server_url | sed 's|https://||') \
  --resource-group $(terraform output -raw resource_group_name) \
  --src-path deploy.zip

# From McpAiAgent/
dotnet publish -c Release -o ./publish
cd publish && zip -r ../deploy.zip . && cd ..
az webapp deploy \
  --name $(terraform output -raw ai_agent_url | sed 's|https://||') \
  --resource-group $(terraform output -raw resource_group_name) \
  --src-path deploy.zip
```

Or use the VS Code Azure App Service extension: right-click the app → Deploy to Web App.

## Key variables to customise

| Variable | Default | When to change |
|---|---|---|
| `project_name` | `aiagent` | Change to your name/team to avoid name collisions |
| `environment` | `dev` | Change to `test` or `prod` for other environments |
| `location` | `eastus` | Change if you need a different Azure region |
| `apim_sku` | `Developer_1` | Use `Basic_1` for an SLA-backed environment |
| `app_service_sku` | `B1` | Use `F1` for free tier (cold starts, no always-on) |
| `apim_rate_limit_calls` | `20` | Calls per tenant per window |
| `apim_cache_duration_seconds` | `300` | How long responses are cached (seconds) |
| `apim_tenant_subscriptions` | 2 tenants | Add more entries to get more subscription keys |

## Cleanup

When you are done with the lab, destroy everything in one command:

```bash
terraform destroy
# Type 'yes' when prompted
```

Or via Azure CLI (faster, skips Terraform state):

```bash
az group delete --name $(terraform output -raw resource_group_name) --yes --no-wait
```

> ⚠️ The Developer tier APIM instance costs ~$50/month. Clean up after the lab if you are on a paid subscription.

## Estimated costs (Developer tier, B1 App Service)

| Resource | Estimated monthly cost |
|---|---|
| APIM Developer tier | ~$50 |
| App Service Plan B1 (2 apps) | ~$13 |
| Azure OpenAI | Pay-per-token (free tier available) |
| Application Insights | First 5 GB/month free |
| Log Analytics | First 5 GB/month free |
| **Total (approx)** | **~$63/month** |

If you are using an Azure free trial ($200) or student credits ($100), this covers roughly 1–2 months of running this environment.

---

*Global Azure 2026 · [github.com/mistryhardik/community](https://github.com/mistryhardik/community)*
`#mumtechup` `#GlobalAzure2026` `#AzureAPIM` `#dotnet` `#Terraform`
