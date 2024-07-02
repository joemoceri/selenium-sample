using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;

namespace SeleniumFFmpeg
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var app = serviceProvider.GetService<App>();

            using (app)
            {
                await app.Run();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // add options
            services.AddOptions();
            services.Configure<ApplicationOptions>(Configuration.GetSection("Settings"));

            // add app and services
            services.AddTransient<App>();
            services.AddTransient<IScreenRecorder, ScreenRecorder>();
        }
    }
}
