using Chloe.Annotations;
using DiscordBot.Extensions.Excel;

namespace DiscordBot.Database.Tables
{
	[Table(Name = "moderators")]
	[ExcelList("Состав модераторов")]
	public class ModeratorTable : BaseModeratorTable
	{
		[ExcelColumn("Дата повышения")]
		public DateTime PromotionDate { get; set; }

		[ExcelColumn("Выговоры")]
		public long Reprimands { get; set; }

		[ExcelColumn("Номер счёта")]
		public string BankNumber { get; set; }

		public override string[] GetMap()
		{
			return new string[]
			{
				nameof(User),
				nameof(Permission),
				nameof(DecisionDate),
				nameof(PromotionDate),
				nameof(Reprimands),
				nameof(Nickname),
				nameof(Sid),
				nameof(ServerNumber),
				nameof(BankNumber),
				nameof(Forum),
				nameof(Id)
			};
		}
	}
}
