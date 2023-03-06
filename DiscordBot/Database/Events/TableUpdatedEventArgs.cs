using DiscordBot.Database.Tables;

namespace DiscordBot.Database.Events
{
	public class TableUpdatedEventArgs : EventArgs
	{
		public ITable Table { get; set; }

		public TableUpdatedEventArgs(ITable table)
		{
			Table = table;
		}
	}
}
