using Chloe.Annotations;

namespace DiscordBot.Database.Tables
{
	public abstract class BaseTable
	{
		[Column(IsPrimaryKey = true, Size = 24)]
		public ulong Id { get; set; }

		public abstract string[] GetMap();
	}
}
