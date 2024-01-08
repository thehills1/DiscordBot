using Chloe.Annotations;
using DiscordBot.Database.Enums;
using Newtonsoft.Json;

namespace DiscordBot.Database.Tables
{
	[Table(Name = "shops")]
	public class ShopTable : ITable
	{
		public ulong OwnerId { get; set; }

		public ServerName ServerName { get; set; }

		[Column(IsPrimaryKey = true)]
		public string Name { get; set; }

		public DateTime PaidUntil { get; set; }

		public ulong FirstDeputyId { get; set; }

		public ulong SecondDeputyId { get; set; }		

		public string ImageUrl { get; set; }

		[JsonIgnore]
		public bool DeputiesExists => !(FirstDeputyId == 0 && SecondDeputyId == 0);

		public string[] GetMap() => null;
	}
}
