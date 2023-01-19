using DiscordBot.Configs;
using DiscordBot.Server;
using DiscordBot.Server.Commands;
using DiscordBot.Server.Database;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
	public class Program
    {
        public static void Main(string[] args)
        {
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddSingleton<ServiceManager>();
			serviceCollection.AddSingleton<Bot>();
			serviceCollection.AddSingleton(container =>
			{
				var config = serviceCollection.BuildServiceProvider().GetService<BotConfig>();

				return new DiscordClient(new DiscordConfiguration
				{
					Token = config.Token,
					TokenType = TokenType.Bot,
					AutoReconnect = true,
					MinimumLogLevel = LogLevel.Debug,
					Intents = DiscordIntents.All
				});
			});

			serviceCollection.AddSingleton(container => BotConfig.LoadOrCreate("config.json"));
			serviceCollection.AddSingleton(container => SalaryConfig.LoadOrCreate("salary_config.json"));

			InitializeServerServiceScope(serviceCollection);

			using (var container = serviceCollection.BuildServiceProvider())
			{
				container.GetService<ServiceManager>().Initialize();

				Console.ReadKey();
			}	
        }

		public static void InitializeServerServiceScope(IServiceCollection serviceCollection)
		{
			serviceCollection.AddScoped<IServerServiceAccessor, ServerServiceAccessor>();
			serviceCollection.AddScoped<ServerService>();
			serviceCollection.AddScoped<ServerDatabaseConnector>();
			serviceCollection.AddScoped<ServerDatabaseManager>();

			serviceCollection.AddScoped<ServerGlobalCommands>();
		}
    }
}
