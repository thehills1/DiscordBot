using DiscordBot.Database.Tables;

namespace DiscordBot.Extensions.Collections
{
	public interface ITableCollection : IReadOnlyCollection<ITable>
	{
		ITable this[int index] { get; }
		Type GetTableType();
	}
}
