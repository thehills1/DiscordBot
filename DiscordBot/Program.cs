﻿using DiscordBot.Configs;
using DiscordBot.Configs.Rules;
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
			InitializeBotEnvironmentDirectories();

			using (var container = serviceCollection.BuildServiceProvider())
			{
				container.GetService<ServiceManager>().Initialize();

				Console.ReadKey();
			}	
        }

		private static void InitializeServerServiceScope(IServiceCollection serviceCollection)
		{
			serviceCollection.AddScoped<ServerContext>();
			serviceCollection.AddScoped<IServerServiceAccessor, ServerServiceAccessor>();
			serviceCollection.AddScoped<ServerService>();
			serviceCollection.AddScoped<ServerDatabaseConnector>();
			serviceCollection.AddScoped<ServerDatabaseManager>();
			serviceCollection.AddScoped<ServerDatabaseEventHandler>();

			serviceCollection.AddScoped<ServerGlobalCommands>();
			serviceCollection.AddScoped<ServerGlobalCommandsManager>();

			serviceCollection.AddScoped<ServerShopCommands>();
			serviceCollection.AddScoped<ServerShopCommandsManager>();

			serviceCollection.AddScoped<ServerRulesCommands>();
			serviceCollection.AddScoped<ServerRulesCommandsManager>();

			serviceCollection.AddScoped(container =>
			{
				var serverContext = container.GetService<ServerContext>();
				return ServerConfig.LoadOrCreate(Path.Combine(serverContext.RootServerPath, "config.json"));
			});

			serviceCollection.AddScoped(container =>
			{
				var serverContext = container.GetService<ServerContext>();
				return ShopsConfig.LoadOrCreate(Path.Combine(serverContext.RootServerPath, "shops_config.json"));
			});

			serviceCollection.AddScoped(container =>
			{
				var serverContext = container.GetService<ServerContext>();
				return RulesConfig.LoadOrCreate(Path.Combine(serverContext.RootServerPath, "rules_config.json"));
			});

			serviceCollection.AddScoped(container =>
			{
				var serverContext = container.GetService<ServerContext>();
				return StaffInfoMessagesConfig.LoadOrCreate(Path.Combine(serverContext.RootServerPath, "staff_info_messages_config.json"));
			});
		}

		private static void InitializeBotEnvironmentDirectories()
		{
			var directories = new string[] 
			{ 
				BotEnvironment.ServersDirectoryPath,
				BotEnvironment.AutocompleteProvidersChoicesPath
			};

			foreach (var directory in directories)
			{
				if (Directory.Exists(directory)) continue;

				Directory.CreateDirectory(directory);
			}
		}
    }
}
