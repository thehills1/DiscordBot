using DiscordBot.Database.Tables;

namespace DiscordBot.Database
{
	public interface IDatabaseManager
	{
		void Initialize();

		void AddOrUpdateTableDB<T>(T table) where T : BaseTable;

		void RemoveTable<T>(T table) where T : BaseTable;
	}
}