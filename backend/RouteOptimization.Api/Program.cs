using RouteOptimization.Api.Services;
using RouteOptimization.Api.Controllers;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);
DotEnv.Load();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register services
builder.Services.AddScoped<IGoogleRoutesService, GoogleRoutesService>();
builder.Services.AddScoped<IRouteOptimizerService, RouteOptimizerService>();

// Add HttpClient for Google API calls
builder.Services.AddHttpClient<IGoogleRoutesService, GoogleRoutesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
