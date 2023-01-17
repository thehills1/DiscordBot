using Chloe.SQLite;

namespace DiscordBot.Database
{
	public interface IDatabaseConnector
	{
		SQLiteContext Connect();
	}
}
