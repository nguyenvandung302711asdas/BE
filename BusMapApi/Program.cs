using api.Data;
using BusMapApi.Services.Implementations;
using BusMapApi.Services.Interfaces;
using BusMapApi.Services;
using Microsoft.EntityFrameworkCore;
using BusMap.Interfaces;
using BusMap.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Đăng ký IHttpClientFactory TRƯỚC khi gọi `builder.Build()`
builder.Services.AddHttpClient();

// Đăng ký OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
// Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddScoped<IUserAdminChatService, UserAdminChatService>();

builder.Services.AddScoped<IGroupChatService, GroupChatService>();

builder.Services.AddScoped<IFavoriteRouteService, FavoriteRouteService>();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Tạo ứng dụng
var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Cấu hình OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Cấu hình Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
