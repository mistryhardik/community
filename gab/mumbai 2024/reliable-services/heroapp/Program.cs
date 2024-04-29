using heroapp.Repository;

var builder = WebApplication.CreateBuilder(args);

// The following line enables Application Insights telemetry collection.
builder.Services.AddApplicationInsightsTelemetry();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Simulate dependency tracking
builder.Services.AddScoped<IExternalService, ExternalService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();