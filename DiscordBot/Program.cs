using DiscordBot.Configs;
using DiscordBot.Server;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
	public class Program
    {
        public static void Main(string[] args)
        {
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddSingleton<ServiceManager>();
			serviceCollection.AddSingleton<Bot>();

			serviceCollection.AddSingleton(container => BotConfig.LoadOrCreate("config.json"));
			serviceCollection.AddSingleton(container => SalaryConfig.LoadOrCreate("salary_config.json"));

			InitializeServerServiceScope(serviceCollection);

			var container = serviceCollection.BuildServiceProvider();

			container.GetService<ServiceManager>().Initialize();

			Console.ReadKey();
        }

		public static void InitializeServerServiceScope(IServiceCollection serviceCollection)
		{
			serviceCollection.AddScoped<ServerService>();
		}
    }
}
