using DiscordBot.Database;

namespace DiscordBot.Server.Database
{
	public class ServerDatabaseConnector : DatabaseConnectorBase
	{
		private readonly IServerServiceAccessor _serverServiceAccessor;

		public ServerDatabaseConnector(IServerServiceAccessor serverServiceAccessor)
			: base($"Data Source={Path.Combine(serverServiceAccessor.Service.RootServerDataPath, "base.db")}")
		{
			_serverServiceAccessor = serverServiceAccessor;
		}
	}
}
