using SOA.NotificationService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddHostedService<GradeCreatedConsumer>();

builder.Services.AddHttpClient("faas", client =>
{
    var baseAddress = builder.Configuration["Faas:BaseAddress"] ?? "http://localhost:8080";
    client.BaseAddress = new Uri(baseAddress);
});

var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/health", () => "OK");

app.Run();
