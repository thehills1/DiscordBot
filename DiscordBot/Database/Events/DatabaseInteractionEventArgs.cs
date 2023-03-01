using DiscordBot.Database.Tables;

namespace DiscordBot.Database.Events
{
	public class DatabaseInteractionEventArgs : EventArgs
	{
		public BaseTable Table { get; set; }

		public DatabaseInteractionEventArgs(BaseTable table)
		{
			Table = table;
		}
	}
}
