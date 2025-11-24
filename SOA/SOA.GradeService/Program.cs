using Microsoft.EntityFrameworkCore;
using SOA.GradeService.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GradesDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("Default")!;
    opt.UseNpgsql(cs, npg => npg.MigrationsAssembly("SOA.Infrastructure"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/health", () => "OK");
app.Run();
