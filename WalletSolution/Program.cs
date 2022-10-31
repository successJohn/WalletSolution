using WalletSolution.Infrastructure;
using WalletSolution.Utils.BackgroundJobService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenExt();
builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddCronJob<WalletUpdateBackgroundJob>(c =>
{
    c.TimeZoneInfo = TimeZoneInfo.Local;
    c.CronExpression = @"0 0 * * *";

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
