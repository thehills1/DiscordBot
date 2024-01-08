using Chloe.Annotations;
using DiscordBot.Database.Enums;

namespace DiscordBot.Database.Tables
{
	[Table(Name = "shopmessages")]
	public class ShopListMessageTable : ITable
	{
		[Column(IsPrimaryKey = true)]
		public ServerName ServerName { get; set; }

		public ulong MessageId { get; set; }

		public string[] GetMap() => null;
	}
}
