using NLog;
using NLog.Web;

namespace GATIntegrations;

public class Program
{
    public static void Main(string[] args)
    {
        // Early init of NLog to allow startup and exception logging, before host is built
        var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        logger.Debug("init main");

        try
        {
            logger.Info("Initializing application...");
            CreateHostBuilder(args).Build().Run();
        }
        catch (System.Exception exception)
        {
            // NLog: catch setup errors
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
}
