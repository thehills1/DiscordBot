using DiscordBot.Database.Tables;

namespace DiscordBot.Database.Events
{
	public class TableAddedEventArgs : EventArgs
	{
		public ITable Table { get; set; }

		public TableAddedEventArgs(ITable table)
		{
			Table = table;
		}
	}
}
