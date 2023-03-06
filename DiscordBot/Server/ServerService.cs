using DiscordBot.Configs;
using DiscordBot.Server.Commands;
using DiscordBot.Server.Database;

namespace DiscordBot.Server
{
	public class ServerService
	{
		public ServerGlobalCommands ServerGlobalCommands { get; }

		private readonly ServerDatabaseManager _databaseManager;
		private readonly ServerDatabaseEventHandler _databaseEventHandler;
		private readonly ServerContext _serverContext;

		public ServerService(
			ServerGlobalCommands serverGlobalCommands, 
			ServerDatabaseManager databaseManager,
			ServerDatabaseEventHandler databaseEventHandler,
			ServerContext serverContext) 
		{
			ServerGlobalCommands = serverGlobalCommands;
			_databaseManager = databaseManager;
			_databaseEventHandler = databaseEventHandler;
			_serverContext = serverContext;
		}

		public void Initialize()
		{
			_databaseManager.Initialize();
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
