using Chloe.Annotations;

namespace DiscordBot.Database.Tables
{
	public abstract class BaseTable
	{
		[Column(IsPrimaryKey = true)]
		public ulong Id { get; set; }

		public abstract string[] GetMap();
	}
}
