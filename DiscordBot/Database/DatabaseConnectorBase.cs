using Chloe.SQLite;

namespace DiscordBot.Database
{
	public abstract class DatabaseConnectorBase : IDatabaseConnector
	{
		private readonly string _connectionString;

		protected DatabaseConnectorBase(string connectionString)
		{
			_connectionString = connectionString;
		}

		public SQLiteContext Connect()
		{
			return new SQLiteContext(() => new System.Data.SQLite.SQLiteConnection(_connectionString));
		}
	}
}