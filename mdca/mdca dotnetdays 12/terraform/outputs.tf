################################################################################
# outputs.tf
# Everything printed after `terraform apply` — all URLs, keys, and
# copy-paste curl commands to verify the deployment immediately.
################################################################################

output "resource_group_name" {
  description = "Resource group name. Use in the cleanup command."
  value       = azurerm_resource_group.main.name
}

# ── Azure OpenAI ──────────────────────────────────────────────────────────────

output "openai_endpoint" {
  description = "Azure OpenAI endpoint URL."
  value       = azurerm_cognitive_account.openai.endpoint
}

output "openai_key" {
  description = "Azure OpenAI primary access key."
  value       = azurerm_cognitive_account.openai.primary_access_key
  sensitive   = true
}

output "openai_primary_deployment" {
  description = "Primary model deployment name (use in AzureOpenAI__DeploymentName app setting)."
  value       = var.openai_primary_deployment_name
}

output "openai_primary_model" {
  description = "Primary model name and version."
  value       = "${var.openai_primary_model_name} (${var.openai_primary_model_version})"
}

output "openai_mini_deployment" {
  description = "Mini model deployment name (for cost-comparison demo)."
  value       = var.openai_mini_deployment_name
}

output "openai_mini_model" {
  description = "Mini model name and version."
  value       = "${var.openai_mini_model_name} (${var.openai_mini_model_version})"
}

# ── App Services ──────────────────────────────────────────────────────────────

output "mcp_server_url" {
  description = "MCP Server URL. Health check: <url>/health"
  value       = "https://${azurerm_windows_web_app.mcp_server.default_hostname}"
}

output "ai_agent_url" {
  description = "AI Agent URL. Chat endpoint: <url>/api/chat"
  value       = "https://${azurerm_windows_web_app.ai_agent.default_hostname}"
}

output "ai_agent_swagger_url" {
  description = "Swagger UI — test the API in a browser without Postman."
  value       = "https://${azurerm_windows_web_app.ai_agent.default_hostname}/swagger"
}

# ── Monitoring ────────────────────────────────────────────────────────────────

output "app_insights_connection_string" {
  description = "Application Insights connection string. Already injected into both App Services."
  value       = azurerm_application_insights.main.connection_string
  sensitive   = true
}

# ── API Management ────────────────────────────────────────────────────────────

output "apim_gateway_url" {
  description = "APIM Gateway URL. All API calls go through here — never call the App Service directly."
  value       = azurerm_api_management.main.gateway_url
}

output "apim_developer_portal_url" {
  description = "APIM Developer Portal — explore APIs and get subscription keys."
  value       = azurerm_api_management.main.developer_portal_url
}

output "apim_chat_endpoint" {
  description = "Full chat endpoint URL through APIM. Use this in Postman and k6."
  value       = "${azurerm_api_management.main.gateway_url}/agent/api/chat"
}

output "tenant_subscription_keys" {
  description = "Primary keys per tenant. Pass in the Ocp-Apim-Subscription-Key header."
  sensitive   = true
  value = {
    for name, sub in azurerm_api_management_subscription.tenants :
    name => sub.primary_key
  }
}

# ── Quick-start commands ──────────────────────────────────────────────────────

output "quickstart_commands" {
  description = "Copy-paste commands to verify the full deployment."
  value       = <<-EOT

    ═══════════════════════════════════════════════════════════
      MODELS DEPLOYED
    ═══════════════════════════════════════════════════════════
      Primary : ${var.openai_primary_model_name} ${var.openai_primary_model_version}
                deployment name → ${var.openai_primary_deployment_name}
      Mini    : ${var.openai_mini_model_name} ${var.openai_mini_model_version}
                deployment name → ${var.openai_mini_deployment_name}

    ═══════════════════════════════════════════════════════════
      QUICK START
    ═══════════════════════════════════════════════════════════

    1. Get tenant keys:
       terraform output -json tenant_subscription_keys

    2. Health checks (direct, no APIM):
       curl https://${azurerm_windows_web_app.mcp_server.default_hostname}/health
       curl https://${azurerm_windows_web_app.ai_agent.default_hostname}/health

    3. Chat through APIM (replace KEY with tenant-a key):
       curl -X POST ${azurerm_api_management.main.gateway_url}/agent/api/chat \
         -H "Content-Type: application/json" \
         -H "Ocp-Apim-Subscription-Key: <TENANT_A_KEY>" \
         -d '{"message": "What is Azure APIM?"}'

    4. Mini model comparison (same prompt, cheaper model):
       curl -X POST ${azurerm_api_management.main.gateway_url}/agent/api/chat \
         -H "Content-Type: application/json" \
         -H "Ocp-Apim-Subscription-Key: <TENANT_A_KEY>" \
         -d '{"message": "What is Azure APIM?", "model": "${var.openai_mini_deployment_name}"}'

    5. Rate limit burst test (expects 429 after call 20):
       for i in $(seq 1 25); do
         curl -s -o /dev/null -w "Call $i: HTTP %%{http_code}\n" \
           -X POST ${azurerm_api_management.main.gateway_url}/agent/api/chat \
           -H "Content-Type: application/json" \
           -H "Ocp-Apim-Subscription-Key: <TENANT_A_KEY>" \
           -d '{"message":"Hello"}'
       done

    6. Cleanup (deletes everything):
       az group delete --name ${azurerm_resource_group.main.name} --yes --no-wait

    ═══════════════════════════════════════════════════════════
  EOT
}
