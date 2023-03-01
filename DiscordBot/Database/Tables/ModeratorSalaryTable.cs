using DiscordBot.Database.Enums;
using DiscordBot.Extensions.Excel;

namespace DiscordBot.Database.Tables
{
	public class ModeratorSalaryTable : BaseTable
	{
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

		public override string[] GetMap()
		{
			return new string[]
			{
				nameof(User),
				nameof(BankNumber),
				nameof(ActionsCount),
				nameof(Salary),
				nameof(Id)
			};
		}
	}
}
