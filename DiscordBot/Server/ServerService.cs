using DiscordBot.Configs;
using DiscordBot.Server.Commands;
using DiscordBot.Server.Database;

namespace DiscordBot.Server
{
	public class ServerService
	{
		private ulong _serverId;
		private ServerConfig _serverConfig;

		public ulong ServerId => _serverId;
		public ServerConfig ServerConfig => _serverConfig;

		public string RootServerDataPath => Path.Combine(BotEnvironment.ServersDirectoryPath, _serverId.ToString());

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

			Console.WriteLine($"Server service for server with id: [{serverId}] was initialized.");
		}

		private void LoadConfig()
		{
			_serverConfig = ServerConfig.LoadOrCreate(Path.Combine(RootServerDataPath, "config.json"));
		}
	}
}
