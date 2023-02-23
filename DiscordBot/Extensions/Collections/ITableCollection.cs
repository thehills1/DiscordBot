using DiscordBot.Database.Tables;

namespace DiscordBot.Extensions.Collections
{
	public interface ITableCollection : IReadOnlyCollection<BaseTable>
	{
		BaseTable this[int index] { get; }
		Type GetTableType();
	}
}
