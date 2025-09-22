using Microsoft.EntityFrameworkCore;
using TestAPI;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add logging to see which connection string is being used
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Connection String: {connectionString?.Substring(0, Math.Min(50, connectionString.Length))}...");

try
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString,
            new MySqlServerVersion(new Version(8, 0, 21)), // Use specific MySQL version instead of auto-detect
            mysqlOptions => {
                mysqlOptions.CommandTimeout(30); // Add 30 second timeout
                mysqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null); // Add retry logic
            })
    );
    Console.WriteLine("Database context configured successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Error configuring database: {ex.Message}");
}

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Test database connection and create database if needed
if (builder.Environment.IsProduction())
{
    Console.WriteLine("Testing production database connection...");
    try
    {
        // First try to connect to the server without specifying database
        var serverConnectionString = "Server=103.154.241.250;Port=5576;Uid=db_admin_x4p9;Pwd=N5!fR2z@q8X#kL1v;Connect Timeout=30;";
        using var serverConnection = new MySqlConnector.MySqlConnection(serverConnectionString);
        await serverConnection.OpenAsync();
        
        // Create database if it doesn't exist
        using var createDbCommand = new MySqlConnector.MySqlCommand("CREATE DATABASE IF NOT EXISTS aspdotnetapi CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;", serverConnection);
        await createDbCommand.ExecuteNonQueryAsync();
        Console.WriteLine("✅ Database 'aspdotnetapi' ensured to exist");
        
        serverConnection.Close();
        
        // Now test connection to the specific database
        using var testConnection = new MySqlConnector.MySqlConnection(connectionString);
        await testConnection.OpenAsync();
        Console.WriteLine("✅ Production database connection test successful!");
        testConnection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Production database connection test failed: {ex.Message}");
        Console.WriteLine("Application will continue but may have database issues.");
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

// Enable static files serving
app.UseStaticFiles();

app.UseAuthorization();

// Configure API routes
app.MapControllers();

// Configure MVC routes for any remaining controller actions
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action=Index}/{id?}");

// Fallback to serve React app for all other routes
app.MapFallbackToFile("index.html");

app.Run();
