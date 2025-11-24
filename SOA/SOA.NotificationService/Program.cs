using SOA.NotificationService.Hubs;
using SOA.NotificationService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddHostedService<GradeCreatedConsumer>();

builder.Services.AddSingleton<GpaSseHub>();
builder.Services.AddCors(o => o.AddPolicy("sse", p => p
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost", "http://localhost:8081")));

builder.Services.AddHttpClient("faas", client =>
{
    var baseAddress = builder.Configuration["Faas:BaseAddress"] ?? "http://localhost:8080";
    client.BaseAddress = new Uri(baseAddress);
});

var app = builder.Build();
app.UseCors("sse");
app.UseSwagger(); 
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/health", () => "OK");

app.Run();
