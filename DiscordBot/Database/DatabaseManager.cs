using System.Linq.Expressions;
using Chloe.SQLite;
using DiscordBot.Database.Tables;

namespace DiscordBot.Database
{
	public class DatabaseManager<TDatabaseConnector> : IDatabaseManager where TDatabaseConnector : IDatabaseConnector
	{
		private SQLiteContext _databaseContext;
		private IDatabaseConnector _databaseConnector;

		public DatabaseManager(TDatabaseConnector databaseConnector)
		{
			_databaseConnector = databaseConnector;
		}

		public void Initialize()
		{
			_databaseContext = _databaseConnector.GetDBContext();
		}

		public void AddOrUpdateTableDB<T>(T table) where T : BaseTable
		{
			int results = _databaseContext.Update(table);

			if (results == 0)
			{
				_databaseContext.Insert(table);
			}
			else
			{
				_databaseContext.Update(table);
			}

			//if (table is ModeratorTable || table is DismissedModeratorTable)
			//{
			//	SendActualModeratorsFile();
			//	if (table is not DismissedModeratorTable) UpdateModeratorsUsername();
			//}
		}	

		public async Task RemoveTable<T>(T table) where T : BaseTable
		{
			await _databaseContext.DeleteAsync(table);

			//if (table is ModeratorTable) SendActualModeratorsFile();
		}

		public async Task<List<T>> GetTablesList<T>() where T : BaseTable
		{
			return await _databaseContext.Query<T>().ToListAsync();
		}

		public async Task<T> GetTableDB<T>(Expression<Func<T, bool>> selector) where T : BaseTable
		{
			return await _databaseContext.Query<T>().FirstOrDefaultAsync(selector);
		}
	}
}
