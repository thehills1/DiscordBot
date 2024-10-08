﻿using Chloe.Annotations;
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

		[ExcelColumn("Ник в игре")]
		public string Nickname { get; set; }

		[ExcelColumn("Номер счёта")]
		public string BankNumber { get; set; }

		[ExcelColumn("Количество действий")]
		public int ActionsCount { get; set; }

		[ExcelColumn("Зарплата")]
		public int Salary { get; set; }

		[ExcelColumn("Выговоры")]
		public long Reprimands { get; set; }

		public DateTime PeriodStartDate { get; set; }

		public DateTime PeriodEndDate { get; set; }

		public string[] GetMap()
		{
			return new string[]
			{
				nameof(User),
				nameof(ServerName),
				nameof(Nickname),
				nameof(BankNumber),
				nameof(ActionsCount),
				nameof(Salary),
				nameof(Reprimands),
				nameof(Id)
			};
		}
	}
}
