using Chloe.Annotations;
using DiscordBot.Database.Extensions;
using DiscordBot.Extensions.Excel;

namespace DiscordBot.Database.Tables
{
	[Table(Name = "moderators")]
	[ExcelList("Состав модераторов")]
	public class ModeratorTable : BaseModeratorTable
	{
		public const int ReprimandsLimit = 3;

		[ExcelColumn("Дата повышения")]
		public DateTime PromotionDate { get; set; }

		[ExcelColumn("Выговоры")]
		public long Reprimands { get; set; }

		[ExcelColumn("Номер счёта")]
		[AllowCommandChange]
		public string BankNumber { get; set; }

		public override string[] GetMap()
		{
			return new string[]
			{
				nameof(User),
				nameof(PermissionLevel),
				nameof(DecisionDate),
				nameof(PromotionDate),
				nameof(Reprimands),
				nameof(Nickname),
				nameof(Sid),
				nameof(ServerName),
				nameof(BankNumber),
				nameof(ForumLink),
				nameof(Id)
			};
		}
	}
}
