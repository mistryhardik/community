using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddSingleton(_ =>
{
    var endpoint = builder.Configuration["AzureOpenAI:Endpoint"]!;
    var key      = builder.Configuration["AzureOpenAI:Key"]!;
    return new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Default document so App Service root doesn't return IIS 403.14
app.MapGet("/", () => Results.Redirect("/swagger"));

// ── Health check ──────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// ── Chat endpoint ─────────────────────────────────────────────
app.MapPost("/api/chat", async (ChatRequest request, AzureOpenAIClient aiClient) =>
{
    if (string.IsNullOrWhiteSpace(request.Message))
        return Results.BadRequest(new { error = "Message cannot be empty." });

    // Allow model override per-request, fall back to config default
    var defaultDeployment = builder.Configuration["AzureOpenAI:DeploymentName"]!;
    var deployment = request.Model ?? defaultDeployment;

    // Build system prompt — use custom one if provided, else default
    var systemPrompt = string.IsNullOrWhiteSpace(request.SystemPrompt)
        ? "You are a helpful assistant. Be concise and clear."
        : request.SystemPrompt;

    var chatClient = aiClient.GetChatClient(deployment);

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage(systemPrompt),
        new UserChatMessage(request.Message)
    };

    var response = await chatClient.CompleteChatAsync(messages);
    var result   = response.Value;

    return Results.Ok(new ChatResponse(
        Reply:            result.Content[0].Text,
        Model:            deployment,
        PromptTokens:     result.Usage.InputTokenCount,
        CompletionTokens: result.Usage.OutputTokenCount,
        TotalTokens:      result.Usage.TotalTokenCount
    ));
})
.WithName("Chat")
.WithOpenApi();

app.Run();

// ── Models ────────────────────────────────────────────────────
record ChatRequest(string Message, string? Model = null, string? SystemPrompt = null);
record ChatResponse(string Reply, string Model, int PromptTokens, int CompletionTokens, int TotalTokens);