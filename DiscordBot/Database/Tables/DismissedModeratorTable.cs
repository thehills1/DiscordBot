﻿using Chloe.Annotations;
using DiscordBot.Extensions.Excel;

namespace DiscordBot.Database.Tables
{
	[Table(Name = "dismissedmoderators")]
	[ExcelList("Снятые модераторы")]
	public class DismissedModeratorTable : BaseModeratorTable
	{
		[ExcelColumn("Дата снятия")]
		public DateTime DismissionDate { get; set; }

		[ExcelColumn("Причина снятия")]
		public string Reason { get; set; }

		[ExcelColumn("Восстановление")]
		public string Reinstatement { get; set; }

		public override string[] GetMap()
		{
			return new string[]
			{
				nameof(User),
				nameof(Permission),
				nameof(DecisionDate),
				nameof(DismissionDate),
				nameof(Nickname),
				nameof(Sid),
				nameof(ServerNumber),
				nameof(Reason),
				nameof(Reinstatement),
				nameof(Forum),
				nameof(Id)
			};
		}
	}
}
