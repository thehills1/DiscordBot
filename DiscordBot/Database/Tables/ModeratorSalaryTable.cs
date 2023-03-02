using Chloe.Annotations;
using DiscordBot.Database.Enums;
using DiscordBot.Extensions.Excel;

namespace DiscordBot.Database.Tables
{
	[Table(Name = "salary")]
	[ExcelList("Зарплата модераторов")]
	public class ModeratorSalaryTable : ITable
	{
		[Column(IsPrimaryKey = true)]
		[AutoIncrement]
		public int UniqueId { get; set; }

		public ulong Id { get; set; }

		[ExcelColumn("Дискорд")]
		public string User { get; set; }

		[ExcelColumn("Сервер")]
		public ServerName ServerName { get; set; }

		[ExcelColumn("Номер счёта")]
		public string BankNumber { get; set; }

		[ExcelColumn("Количество действий")]
		public int ActionsCount { get; set; }

		[ExcelColumn("Зарплата")]
		public int Salary { get; set; }

		[ExcelColumn("Начало расчетного периода")]
		public DateTime PeriodStartDate { get; set; }

		[ExcelColumn("Конец расчетного периода")]
		public DateTime PeriodEndDate { get; set; }

		public string[] GetMap()
		{
			return new string[]
			{
				nameof(User),
				nameof(ServerName),
				nameof(BankNumber),
				nameof(ActionsCount),
				nameof(PeriodStartDate),
				nameof(PeriodEndDate),
				nameof(Salary),
				nameof(Id)
			};
		}
	}
}
