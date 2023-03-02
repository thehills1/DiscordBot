using System.Linq.Expressions;
using DiscordBot.Database.Tables;

namespace DiscordBot.Database
{
	public interface IDatabaseManager
	{
		void Initialize();

		void AddOrUpdateTableDB<T>(T table) where T : ITable;

		Task RemoveTable<T>(T table) where T : ITable;

		Task<List<T>> GetTablesList<T>() where T : ITable;

		Task<T> GetTableDB<T>(Expression<Func<T, bool>> selector) where T : ITable;
	}
}