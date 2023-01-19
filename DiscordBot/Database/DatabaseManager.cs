namespace DiscordBot.Database
{
	public class DatabaseManager<TDatabaseConnector> : IDatabaseManager where TDatabaseConnector : IDatabaseConnector
	{
		private IDatabaseConnector _databaseConnector;

		public DatabaseManager(TDatabaseConnector databaseConnector)
		{
			_databaseConnector = databaseConnector;
		}

		public void Connect()
		{
			_databaseConnector.Connect();
		}
	}
}
