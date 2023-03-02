using DiscordBot.Configs;
using DiscordBot.Server.Commands;
using DiscordBot.Server.Database;

namespace DiscordBot.Server
{
	public class ServerService
	{
		private readonly string ServerDataPath = "data";

		private ulong _serverId;
		private ServerConfig _serverConfig;

		public ulong ServerId => _serverId;
		public ServerConfig ServerConfig => _serverConfig;

		public string RootServerPath => Path.Combine(BotEnvironment.ServersDirectoryPath, _serverId.ToString());
		public string ModeratorsWorksheetsPath => Path.Combine(RootServerPath, ServerDataPath, "ModeratorsWorksheets");
		public string SalaryWorksheetsPath => Path.Combine(RootServerPath, ServerDataPath, "SalaryWorksheets");

		public ServerGlobalCommands ServerGlobalCommands { get; }
		public ServerDatabaseManager DatabaseManager { get; }

		public ServerService(ServerGlobalCommands serverGlobalCommands, ServerDatabaseManager databaseManager) 
		{
			ServerGlobalCommands = serverGlobalCommands;
			DatabaseManager = databaseManager;
		}

		public void Initialize(ulong serverId)
		{
			_serverId = serverId;

			LoadConfig();
			DatabaseManager.Initialize();
			InitializeServerDirectories();

			Console.WriteLine($"Server service for server with id: [{serverId}] was initialized.");
		}

		private void LoadConfig()
		{
			_serverConfig = ServerConfig.LoadOrCreate(Path.Combine(RootServerPath, "config.json"));
		}

		private void InitializeServerDirectories()
		{
			var directoriesToInitialize = new string[]
			{
				ModeratorsWorksheetsPath,
				SalaryWorksheetsPath
			};

			foreach (var directory in directoriesToInitialize)
			{
				if (Directory.Exists(directory)) continue;

				Directory.CreateDirectory(directory);
			}
		}
	}
}
