using System.Linq.Expressions;
using Chloe.SQLite;
using DiscordBot.Database.Events;
using DiscordBot.Database.Tables;

namespace DiscordBot.Database
{
	public class DatabaseManager<TDatabaseConnector> : IDatabaseManager where TDatabaseConnector : IDatabaseConnector
	{
		public event EventHandler<DatabaseInteractionEventArgs> DatabaseInteractionCompleted;

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

		public void AddOrUpdateTableDB<T>(T table) where T : ITable
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

			DatabaseInteractionCompleted?.Invoke(this, new DatabaseInteractionEventArgs(table));
		}	

		public void AddTableDB<T>(T table) where T : ITable
		{
			_databaseContext.Insert(table);

			DatabaseInteractionCompleted?.Invoke(this, new DatabaseInteractionEventArgs(table));
		}

		public async Task RemoveTable<T>(T table) where T : ITable
		{
			await _databaseContext.DeleteAsync(table);

			DatabaseInteractionCompleted?.Invoke(this, new DatabaseInteractionEventArgs(table));
		}

		public async Task<List<T>> GetTablesList<T>() where T : ITable
		{
			return await _databaseContext.Query<T>().ToListAsync();
		}

		public async Task<T> GetTableDB<T>(Expression<Func<T, bool>> selector) where T : ITable
		{
			return await _databaseContext.Query<T>().FirstOrDefaultAsync(selector);
		}
	}
}
