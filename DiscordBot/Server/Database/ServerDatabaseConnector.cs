using DiscordBot.Database;

namespace DiscordBot.Server.Database
{
	public class ServerDatabaseConnector : DatabaseConnectorBase
	{
		private const string ServerBasicDatabaseName = "base.db";
		
		public override string DatabasePath => Path.Combine(_serverContext.RootServerPath, ServerBasicDatabaseName);

		private readonly ServerContext _serverContext;

		public ServerDatabaseConnector(ServerContext serverContext) : base()
		{
			_serverContext = serverContext;
		}
	}
}
