using System.Linq.Expressions;
using DiscordBot.Database.Tables;

namespace DiscordBot.Database
{
	public interface IDatabaseManager
	{
		void Initialize();

		void AddOrUpdateTableDB<T>(T table) where T : BaseTable;

		Task RemoveTable<T>(T table) where T : BaseTable;

		Task<List<T>> GetTablesList<T>() where T : BaseTable;

		Task<T> GetTableDB<T>(Expression<Func<T, bool>> selector) where T : BaseTable;
	}
}