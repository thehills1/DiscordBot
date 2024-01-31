using System.Linq.Expressions;
using DiscordBot.Database.Tables;

namespace DiscordBot.Database
{
	public interface IDatabaseManager
	{
		void Initialize();
		void AddOrUpdateTableDB<T>(T table) where T : ITable;
		Task RemoveTableAsync<T>(T table) where T : ITable;
		Task<List<T>> GetMultyDataDB<T>() where T : ITable;
		Task<List<T>> GetMultyDataDBAsync<T>(Expression<Func<T, bool>> selector) where T : ITable;
		Task<List<T>> GetMultyDataDBAsc<T, TOrder>(Expression<Func<T, TOrder>> orderSelector) where T : ITable;
		Task<List<T>> GetMultyDataDBDesc<T, TOrder>(Expression<Func<T, TOrder>> orderSelector) where T : ITable;
		Task<List<T>> GetMultyDataDBAsc<T, TOrder>(Expression<Func<T, bool>> selector, Expression<Func<T, TOrder>> orderSelector) where T : ITable;
		Task<List<T>> GetMultyDataDBDesc<T, TOrder>(Expression<Func<T, bool>> selector, Expression<Func<T, TOrder>> orderSelector) where T : ITable;
		Task<T> GetTableDB<T>(Expression<Func<T, bool>> selector) where T : ITable;
	}
}