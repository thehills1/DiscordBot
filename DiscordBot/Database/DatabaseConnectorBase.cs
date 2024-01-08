using Chloe.SQLite;
using DiscordBot.Database.Tables;

namespace DiscordBot.Database
{
	public abstract class DatabaseConnectorBase : IDatabaseConnector
	{
		public abstract string DatabasePath { get; }

		protected DatabaseConnectorBase() { }

		public virtual SQLiteContext GetDBContext()
		{
			var context = new SQLiteContext(() => new System.Data.SQLite.SQLiteConnection($"Data Source={DatabasePath}"));

			CreateDatabaseIfNotExists(context);

			return context;
		}

		private void CreateDatabaseIfNotExists(SQLiteContext context)
		{
			try
			{
				if (File.Exists(DatabasePath)) return;

				File.Create(DatabasePath).Close();

				TableHelper.InitTable<ModeratorTable>(context);
				TableHelper.InitTable<DismissedModeratorTable>(context);
				TableHelper.InitTable<ModeratorSalaryTable>(context);
				TableHelper.InitTable<ShopTable>(context);
				TableHelper.InitTable<ShopListMessageTable>(context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}