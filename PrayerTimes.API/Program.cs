using PrayerTimes.API.Services;


var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to support both HTTP and HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000);
    options.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

// Register the PrayerTimeService (Fixing the error)
builder.Services.AddHttpClient<PrayerTimeService>();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add routes
app.MapControllers();

app.Run();
