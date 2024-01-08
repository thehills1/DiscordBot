using DiscordBot.Database;
using DiscordBot.Database.Tables;

namespace DiscordBot.Server.Database
{
	public sealed class ServerDatabaseManager : DatabaseManager<ServerDatabaseConnector>
	{
		public ServerDatabaseManager(ServerDatabaseConnector databaseConnector) : base(databaseConnector)
		{
		}

		public async Task RemoveTable<T>(ulong id) where T : BaseTable
		{
			RemoveTable(await GetTable<T>(id));
		}

		public async Task<T> GetTable<T>(ulong id) where T : BaseTable
		{
			return await GetTableDB<T>(obj => obj.Id == id);
		}
	}
}
