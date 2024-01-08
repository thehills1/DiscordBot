using DiscordBot.Configs;
using DiscordBot.Server.Commands;
using DiscordBot.Server.Database;

namespace DiscordBot.Server
{
	public class ServerService
	{
		public ServerGlobalCommands ServerGlobalCommands { get; }
		public ServerShopCommands ServerShopCommands { get; }
		public ServerDatabaseManager DatabaseManager { get; }

		private readonly ServerDatabaseEventHandler _databaseEventHandler;
		private readonly ServerContext _serverContext;

		public ServerService(
			ServerGlobalCommands serverGlobalCommands,
			ServerShopCommands serverShopCommands,
			ServerDatabaseManager databaseManager,
			ServerDatabaseEventHandler databaseEventHandler,
			ServerContext serverContext) 
		{
			ServerGlobalCommands = serverGlobalCommands;
			ServerShopCommands = serverShopCommands;
			DatabaseManager = databaseManager;
			_databaseEventHandler = databaseEventHandler;
			_serverContext = serverContext;
		}

		public void Initialize()
		{
			DatabaseManager.Initialize();
			InitializeServerDirectories();
			_databaseEventHandler.Initialize();

			Console.WriteLine($"Server service for server with id: [{_serverContext.ServerId}] was initialized.");
		}

		private void InitializeServerDirectories()
		{
			var directoriesToInitialize = new string[]
			{
				_serverContext.ModeratorsWorksheetsPath,
				_serverContext.SalaryWorksheetsPath
			};

			foreach (var directory in directoriesToInitialize)
			{
				if (Directory.Exists(directory)) continue;

				Directory.CreateDirectory(directory);
			}
		}
	}
}
