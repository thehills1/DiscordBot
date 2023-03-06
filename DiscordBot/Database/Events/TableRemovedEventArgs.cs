using DiscordBot.Database.Tables;

namespace DiscordBot.Database.Events
{
	public class TableRemovedEventArgs : EventArgs
	{
		public ITable Table { get; set; }

		public TableRemovedEventArgs(ITable table)
		{
			Table = table;
		}
	}
}
