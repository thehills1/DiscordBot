using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
	public class DatabaseManager<TDatabaseConnector> where TDatabaseConnector : IDatabaseConnector, new()
	{
		private IDatabaseConnector _databaseConnector;

		public DatabaseManager()
		{
			_databaseConnector = new TDatabaseConnector();
		}
	}
}
