using Microsoft.AspNetCore.Connections;
using Serilog;
using Serilog.Sinks.MSSqlServer;
string outPutFolderPath=AppDomain.CurrentDomain.BaseDirectory;
string logFilePath = Path.Combine(outPutFolderPath, "logs/PTKDotNetCore.WebApp.txt");
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional:false , reloadOnChange:true).Build();
string connectionString = configuration.GetConnectionString("DbConnection");
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Hour)
    .WriteTo
    .MSSqlServer(
        connectionString: connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Tbl_logEvent",
            AutoCreateSqlTable = true,
        })
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
