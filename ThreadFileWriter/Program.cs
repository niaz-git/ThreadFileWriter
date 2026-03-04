using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThreadFileWriter;
internal class Program
{
    private const string LogDirectory = "log";
    private const string FilePath = "log/out.txt";


    static async Task<int> Main(string[] args)
    {
        // # Exception handling
        try
        {
            // Create log directory if it doesn't exist
            Directory.CreateDirectory(LogDirectory);
            using IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureLogging(logging =>
                 {
                     logging.ClearProviders();       // Remove default providers
                     logging.AddConsole();           // Linux-safe
                 })
                  .ConfigureServices((context, services) =>
                  {
                      services.AddSingleton<IFileWriter>(
                          _ => new AsyncFileWriter(FilePath));
                      services.AddSingleton<App>();

                  })
                  .Build();
            var app = host.Services.GetRequiredService<App>();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return 1;

        }
    }
}