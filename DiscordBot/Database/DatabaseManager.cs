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
			//bool create = false;
			//if (!File.Exists(_databaseConnector.DatabasePath))
			//{
			//	File.Create(_databaseConnector.DatabasePath);
			//}

			_databaseContext = _databaseConnector.GetDBContext();
		}

		public void AddOrUpdateTableDB<T>(T table) where T : BaseTable
		{
			throw new NotImplementedException();
		}	

		public void RemoveTable<T>(T table) where T : BaseTable
		{
			throw new NotImplementedException();
		}
	}
}
