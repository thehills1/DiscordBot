using DiscordBot.Extensions.Excel;

namespace DiscordBot.Database.Tables
{
	public abstract class BaseModeratorTable : BaseTable
	{
		[ExcelColumn("Дискорд")]
		public string User { get; set; }

		[ExcelColumn("Должность")]
		public long Permission { get; set; }

		[ExcelColumn("Дата постановления")]
		public DateTime DecisionDate { get; set; }

		[ExcelColumn("Ник в игре")]
		public string Nickname { get; set; }

		[ExcelColumn("SID")]
		public string Sid { get; set; }

		[ExcelColumn("Сервер")]
		public long ServerNumber { get; set; }

		[ExcelColumn("Форумный аккаунт")]
		public string Forum { get; set; }
	}
}
