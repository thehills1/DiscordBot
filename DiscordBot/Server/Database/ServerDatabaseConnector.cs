using DiscordBot.Database;

namespace DiscordBot.Server.Database
{
	public class ServerDatabaseConnector : DatabaseConnectorBase
	{
		public const string ServerBasicDatabaseName = "base.db";

		public override string DatabasePath => Path.Combine(_serverServiceAccessor.Service.RootServerPath, ServerBasicDatabaseName);

		private readonly IServerServiceAccessor _serverServiceAccessor;

		public ServerDatabaseConnector(IServerServiceAccessor serverServiceAccessor) : base()
		{
			_serverServiceAccessor = serverServiceAccessor;
		}
	}
}
