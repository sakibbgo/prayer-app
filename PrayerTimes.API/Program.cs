using PrayerTimes.API.Services;


var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to support both HTTP and HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000); // HTTP binding
    options.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS binding
    });
});

// Register the PrayerTimeService (Fixing the error)
builder.Services.AddHttpClient<PrayerTimeService>();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Enables minimal APIs for Swagger
builder.Services.AddSwaggerGen(); // Adds Swagger generation

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Enables the Swagger UI
}

app.UseHttpsRedirection();

// Add a default route for the root endpoint
app.MapControllers();

app.Run();
