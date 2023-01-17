using DiscordBot.Database;

namespace DiscordBot.Server.Database
{
	public sealed class ServerDatabaseManager : DatabaseManager<ServerDatabaseConnector>
	{
		public ServerDatabaseManager(ServerDatabaseConnector databaseConnector) : base(databaseConnector)
		{
		}
	}
}
