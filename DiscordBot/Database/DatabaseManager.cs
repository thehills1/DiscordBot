using System.Linq.Expressions;
using Chloe.SQLite;
using DiscordBot.Database.Events;
using DiscordBot.Database.Tables;

namespace DiscordBot.Database
{
	public class DatabaseManager<TDatabaseConnector> : IDatabaseManager where TDatabaseConnector : IDatabaseConnector
	{
		public event EventHandler<TableAddedEventArgs> TableAdded;
		public event EventHandler<TableUpdatedEventArgs> TableUpdated;
		public event EventHandler<TableRemovedEventArgs> TableRemoved;

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
				AddTableDB(table);
			}
			else
			{
				_databaseContext.Update(table);

				TableUpdated?.Invoke(this, new TableUpdatedEventArgs(table));
			}
		}	

		public void AddTableDB<T>(T table) where T : ITable
		{
			_databaseContext.Insert(table);

			TableAdded?.Invoke(this, new TableAddedEventArgs(table));
		}

		public async Task RemoveTableAsync<T>(T table) where T : ITable
		{
			await _databaseContext.DeleteAsync(table);

			TableRemoved?.Invoke(this, new TableRemovedEventArgs(table));
		}

		public async Task<List<T>> GetMultyDataDB<T>() where T : ITable
		{
			return await _databaseContext.Query<T>().ToListAsync();
		}

		public async Task<List<T>> GetMultyDataDBAsync<T>(Expression<Func<T, bool>> selector) where T : ITable
		{
			return await _databaseContext.Query<T>().Where(selector).ToListAsync();
		}

		public async Task<List<T>> GetMultyDataDBAsc<T, TOrder>(Expression<Func<T, TOrder>> orderSelector) where T : ITable
		{
			return await _databaseContext.Query<T>().OrderBy(orderSelector).ToListAsync();
		}

		public async Task<List<T>> GetMultyDataDBDesc<T, TOrder>(Expression<Func<T, TOrder>> orderSelector) where T : ITable
		{
			return await _databaseContext.Query<T>().OrderByDesc(orderSelector).ToListAsync();
		}

		public async Task<List<T>> GetMultyDataDBAsc<T, TOrder>(Expression<Func<T, bool>> selector, Expression<Func<T, TOrder>> orderSelector) where T : ITable
		{
			return await _databaseContext.Query<T>().Where(selector).OrderBy(orderSelector).ToListAsync();
		}

		public async Task<List<T>> GetMultyDataDBDesc<T, TOrder>(Expression<Func<T, bool>> selector, Expression<Func<T, TOrder>> orderSelector) where T : ITable
		{
			return await _databaseContext.Query<T>().Where(selector).OrderByDesc(orderSelector).ToListAsync();
		}

		public async Task<T> GetTableDB<T>(Expression<Func<T, bool>> selector) where T : ITable
		{
			return await _databaseContext.Query<T>().FirstOrDefaultAsync(selector);
		}
	}
}
