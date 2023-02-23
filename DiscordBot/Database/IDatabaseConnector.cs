using Chloe.SQLite;

namespace DiscordBot.Database
{
	public interface IDatabaseConnector
	{
		string DatabasePath { get; }
		SQLiteContext GetDBContext();
	}
}
