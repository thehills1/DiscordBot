using DiscordBot.Database.Enums;
using DiscordBot.Database.Extensions;
using DiscordBot.Extensions.Excel;

namespace DiscordBot.Database.Tables
{
	public abstract class BaseModeratorTable : BaseTable
	{
		[ExcelColumn("Дискорд")]
		public string User { get; set; }

		[ExcelColumn("Уровень доступа")]
		public PermissionLevel PermissionLevel { get; set; }

		[ExcelColumn("Дата постановления")]
		public DateTime DecisionDate { get; set; }

		[ExcelColumn("Ник в игре")]
		[AllowCommandChange]
		public string Nickname { get; set; }

		[ExcelColumn("SID")]
		[AllowCommandChange]
		public string Sid { get; set; }

		[ExcelColumn("Сервер")]
		[AllowCommandChange]
		public ServerName ServerName { get; set; }

		[ExcelColumn("Форумный аккаунт")]
		[AllowCommandChange]
		public string ForumLink { get; set; }
	}
}
