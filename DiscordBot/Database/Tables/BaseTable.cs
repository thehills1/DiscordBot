using Chloe.Annotations;

namespace DiscordBot.Database.Tables
{
	public abstract class BaseTable : ITable
	{
		[Column(IsPrimaryKey = true)]
		public ulong Id { get; set; }

		public abstract string[] GetMap();
	}
}
