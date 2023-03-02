using DiscordBot.Database.Tables;

namespace DiscordBot.Database.Events
{
	public class DatabaseInteractionEventArgs : EventArgs
	{
		public ITable Table { get; set; }

		public DatabaseInteractionEventArgs(ITable table)
		{
			Table = table;
		}
	}
}
