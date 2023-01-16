using DiscordBot.Configs;

namespace DiscordBot.Server
{
	public class ServerService
	{
		private ulong _serverId;
		private ServerConfig _serverConfig;

		public ulong ServerId => _serverId;
		public ServerConfig ServerConfig => _serverConfig;

		public string RootServerDataPath => Path.Combine(BotEnvironment.ServersDirectoryPath, _serverId.ToString());

		public ServerService() 
		{

		}

		public void Initialize(ulong serverId)
		{
			_serverId = serverId;

			LoadConfig();
		}

		private void LoadConfig()
		{
			_serverConfig = ServerConfig.LoadOrCreate(Path.Combine(RootServerDataPath, "config.json"));
		}

		private void CreateDatabaseConnector()
		{

		}
	}
}
