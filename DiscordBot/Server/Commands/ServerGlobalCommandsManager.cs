using Chloe.Threading.Tasks;
using DiscordBot.Configs;
using DiscordBot.Database.Enums;
using DiscordBot.Database.Tables;
using DiscordBot.Extensions;
using DiscordBot.Extensions.Collections;
using DiscordBot.Extensions.Excel;
using DiscordBot.Server.Database;
using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using PermissionLevel = DiscordBot.Database.Enums.PermissionLevel;

namespace DiscordBot.Server.Commands
{
    public class ServerGlobalCommandsManager
	{
		private readonly Bot _bot;
		private readonly IServerServiceAccessor _serverServiceAccessor;
		private readonly SalaryConfig _salaryConfig;

		public ServerGlobalCommandsManager(
			Bot bot,
			IServerServiceAccessor serverServiceAccessor,
			SalaryConfig salaryConfig)
		{
			_bot = bot;
			_serverServiceAccessor = serverServiceAccessor;
			_salaryConfig = salaryConfig;
		}

		public bool TryAddModeratorTable(
			DiscordUser user,
			PermissionLevel permissionLevel,
            DateTime desicionDate,
            long reprimands,
            string nickname,
            string sid,
			ServerName serverName,
            string bankNumber,
            string forumLink,
            out string message)
        {
            message = "";

            if (CheckModeratorTableExists(user.Id, out _, out message)) return false;
            if (CheckNickname(nickname, out message)) return false;
            if (CheckSid(sid, out message)) return false;
            if (CheckForumLink(forumLink, out message)) return false;

            var table = new ModeratorTable
            {
                Id = user.Id,
                User = user.GetUser(),
                PermissionLevel = permissionLevel,
                DecisionDate = desicionDate,
                PromotionDate = desicionDate,
                Reprimands = reprimands,
                Nickname = nickname,
                Sid = sid,
                ServerName = serverName,
                BankNumber = bankNumber,
                ForumLink = forumLink
			};

			_serverServiceAccessor.Service.DatabaseManager.AddOrUpdateTableDB(table);

            message = $"Добавил {user.Mention} в список модераторов.";
            return true;
        }

		public bool TrySetModeratorPermissionLevel(
			DiscordUser user, 
			PermissionLevel permissionLevel,
			out string message,
			string dismissionReason = null,
			string reinstatement = null)
		{
			message = "";

			if (CheckModeratorTableNotExists(user.Id, out var table, out message)) return false;

			if (table.PermissionLevel == permissionLevel)
			{
				message = "У модератора уже установлен данный уровень доступа.";
				return false;
			}
			
			if (permissionLevel == PermissionLevel.None)
			{
				if (dismissionReason == null)
				{
					message = "Вы не указали причину снятия.";
					return false;
				}

				reinstatement ??= table.Reprimands == 0 ? "да" : "нет";

				RemoveModeratorTable(table, dismissionReason, reinstatement, out message);
				return true;
			}

			table.PermissionLevel = permissionLevel;
			_serverServiceAccessor.Service.DatabaseManager.AddOrUpdateTableDB(table);

			message = $"Вы изменили уровень модератора {user.Mention} на **{permissionLevel}.**\nСменить покрас: {table.ForumLink}";
			return true;
		}

		public bool TryWarnModerator(DiscordUser user, out string message)
		{
			message = "";

			if (CheckModeratorTableNotExists(user.Id, out var table, out message)) return false;

			if (table.Reprimands == ModeratorTable.ReprimandsLimit - 1)
			{
				RemoveModeratorTable(table, $"{ModeratorTable.ReprimandsLimit}/{ModeratorTable.ReprimandsLimit}", "нет", out message);
				return true;
			}

			table.Reprimands++;
			_serverServiceAccessor.Service.DatabaseManager.AddOrUpdateTableDB(table);

			message = $"Вы выдали модератору {user.Mention} предупреждение. " +
				$"\nТекущее количество предупреждений модератора: **{table.Reprimands}/{ModeratorTable.ReprimandsLimit}.**";
			return true;
		}

		public bool TryEditModeratorInfo(DiscordUser user, string property, string value, out string message)
		{
			message = "";

			if (CheckModeratorTableNotExists(user.Id, out var table, out message)) return false;

			switch (property)
			{
				case nameof(ModeratorTable.Sid):
					if (!TryEditSid(table, value, out message)) return false;			
					break;
				case nameof(ModeratorTable.BankNumber):
					if (!TryEditBankNumber(table, value, out message)) return false;
					break;
				case nameof(ModeratorTable.Nickname):
					if (!TryEditNickname(table, value, out message)) return false;					
					break;
				case nameof(ModeratorTable.ServerName):
					if (!TryEditServerName(table, value, out message)) return false;
					break;
				case nameof(ModeratorTable.ForumLink):
					if (!TryEditForumLink(table, value, out message)) return false;				
					break;
			}

			_serverServiceAccessor.Service.DatabaseManager.AddOrUpdateTableDB(table);

			message = $"Вы изменили данные модератора {user.Mention}. " +
				$"\nБыли изменены данные: **{property}.** " +
				$"\nНовое значение: **{value}.**";
			return true;
		}

		public bool TrySendExcelStaffWorksheet(DiscordChannel channel, bool allTables, out string message)
		{
			message = "";

			var tables = new List<ITableCollection>();

			var modTables = _serverServiceAccessor.Service.DatabaseManager
				.GetTablesList<ModeratorTable>().Result
				.OrderByDescending(table => table.PermissionLevel).ToList();

			tables.Add(new TableCollection<ModeratorTable>(modTables));

			if (allTables)
			{
				var dismissedModTables = _serverServiceAccessor.Service.DatabaseManager
					.GetTablesList<DismissedModeratorTable>().Result
					.OrderBy(table => table.DismissionDate).ToList();

				tables.Add(new TableCollection<DismissedModeratorTable>(dismissedModTables));
			}

			var fileName = Path.Combine(_serverServiceAccessor.Service.ModeratorsWorksheetsPath, $"moderators {DateTime.Now.ToString().Replace(":", ".")}.xlsx");
			var messageContent = "Актуальный список модераторов:";

			var lastMessages = channel.GetMessagesAsync().Result;
			foreach (var discordMessage in lastMessages)
			{
				if (discordMessage.Content.Contains(messageContent))
				{
					discordMessage.DeleteAsync().GetResult();
					break;
				}
			}

			ExcelWorksheetCreator.GenerateAndSaveFile(tables, fileName);

			using (var fileStream = new FileStream(fileName, FileMode.Open))
			{
				_ = new DiscordMessageBuilder().WithContent(messageContent).AddFile(fileStream).SendAsync(channel).Result;
			}

			message = $"Лист с таблицами был успешно отправлен в канал {channel.Mention}.";
			return true;
		}

		public bool TrySendExcelSalaryWorksheet(out string message, int weeks = 2)
		{
			message = "";

			var channelToDownload = _bot.GetChannelAsync(_serverServiceAccessor.Service.ServerConfig.BotStatsChannelId).Result;
			var channelToSend = _bot.GetChannelAsync(_serverServiceAccessor.Service.ServerConfig.StatsChannelId).Result;

			var moderatorsActions = GetModeratorsActions(
					channelToDownload, 
					out var periodStartDate, 
					out var periodEndDate, 
					weeks);
			CheckAndRemoveExtraModeratorsActions(moderatorsActions, weeks, out var totalActions);
			var salaryTables = CalculateSalary(moderatorsActions, weeks, totalActions, periodStartDate, periodEndDate);

			var dateTimeNow = DateTime.Now;
			var date0 = dateTimeNow.AddDays(-weeks * 7).ToShortDateString();
			var date1 = dateTimeNow.ToShortDateString();
			var datesString = $"{date0} - {date1}";

			var fileName = Path.Combine(_serverServiceAccessor.Service.SalaryWorksheetsPath, $"salary {datesString}.xlsx");
			ExcelWorksheetCreator.GenerateAndSaveFile(new() { salaryTables }, fileName);

			using (var fileStream = new FileStream(fileName, FileMode.Open))
			{
				_ = new DiscordMessageBuilder()
					.WithContent($"Зарплата за {datesString}.")
					.AddFile(fileStream)
					.SendAsync(channelToSend)
					.Result;
			}

			message = $"Лист зарплатой был успешно отправлен в канал {channelToSend.Mention}.";
			return true;
		}

		private Dictionary<ulong, int> GetModeratorsActions(
			DiscordChannel channel, 
			out DateTime periodStartDate,
			out DateTime periodEndDate,
			int weeks = 2)
		{
			var moderatorsActions = new Dictionary<ulong, int>();

			var messages = channel.GetMessagesAsync(weeks).Result;
			foreach (var message in messages)
			{
				var url = message.Attachments.First().Url;
				var json = Encoding.UTF8.GetString(new HttpClient().GetByteArrayAsync(url).Result);

				var settings = new JsonSerializerSettings() { ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() } };
				var tables = JsonConvert.DeserializeObject<WeeklyStatsTable[]>(json, settings);

				foreach (var table in tables)
                {
                    var id = table.Id;
                    var moderatorActions = table.Tickets + table.Punishments;

					if (moderatorsActions.ContainsKey(id))
					{
						moderatorsActions[id] += moderatorActions;
					}
					else
					{
						moderatorsActions.Add(id, moderatorActions);
					}
				}
			}

			periodStartDate = messages.First().Timestamp.DateTime.AddDays(-7);
			periodEndDate = messages.Last().Timestamp.DateTime;

			return moderatorsActions;
		}

		private void CheckAndRemoveExtraModeratorsActions(Dictionary<ulong, int> actions, int weeks, out int totalActions)
		{
			totalActions = 0;

			foreach (var moderatorAction in actions)
			{
				var table = _serverServiceAccessor.Service.DatabaseManager.GetTable<ModeratorTable>(moderatorAction.Key).Result;

				if (table == null)
				{
					actions.Remove(moderatorAction.Key);
					continue;
				}

				if (table.PermissionLevel <= PermissionLevel.Curator 
					&& moderatorAction.Value >= _salaryConfig.ActionsPerWeekToSalary * weeks)
				{
					totalActions += moderatorAction.Value;
				}
			}
		}

		private TableCollection<ModeratorSalaryTable> CalculateSalary(
			Dictionary<ulong, int> actions, 
			int weeks, 
			int totalActions,
			DateTime periodStartDate,
			DateTime periodEndDate)
		{
			var output = new List<ModeratorSalaryTable>();

			foreach (var moderatorAction in actions) 
			{
				var moderatorTable = _serverServiceAccessor.Service.DatabaseManager.GetTable<ModeratorTable>(moderatorAction.Key).Result;
				var salary = CalculateChiefsSalary(moderatorTable);

				if (moderatorTable.PermissionLevel <= PermissionLevel.Curator)
				{
					salary += CalculateActionsSalary(moderatorAction.Value, weeks, totalActions);
                }

				var salaryTable = new ModeratorSalaryTable()
				{
					Id = moderatorAction.Key,
					User = moderatorTable.User,
					ServerName = moderatorTable.ServerName,
					BankNumber = moderatorTable.BankNumber,
					ActionsCount = moderatorAction.Value,
					Salary = salary,
					PeriodStartDate = periodStartDate,
					PeriodEndDate = periodEndDate
				};

				_serverServiceAccessor.Service.DatabaseManager.AddTableDB(salaryTable);

				output.Add(salaryTable);
			}

			return new TableCollection<ModeratorSalaryTable>(output);
		}

		private int CalculateChiefsSalary(ModeratorTable table)
		{
			if (table.PermissionLevel < PermissionLevel.Curator) return 0;

			switch (table.PermissionLevel)
			{
				case PermissionLevel.Curator:
					return _salaryConfig.ChiefsSalary.CuratorSalary;
				case PermissionLevel.DeputyChiefModerator:
					return _salaryConfig.ChiefsSalary.DeputyChiefModeratorSalary;
				case PermissionLevel.ChiefModerator:
					return _salaryConfig.ChiefsSalary.ChiefModeratorSalary;
				default:
					return 0;
			}
		}

		private int CalculateActionsSalary(int moderatorActions, int weeks, int totalActions)
		{
			if (moderatorActions < _salaryConfig.ActionsPerWeekToSalary * weeks) return 0;

			var raw = (int) Math.Ceiling((double) moderatorActions / totalActions * _salaryConfig.Sum * weeks);
			return (int) Math.Floor((double) raw / 1000) * 1000;
        }

		private bool TryEditSid(ModeratorTable table, string value, out string message)
		{
			message = "";

			if (CheckSid(value, out message)) return false;
			if (value == table.Sid)
			{
				message = "Старый и новый SID совпадают.";
				return false;
			}

			table.Sid = value;

			return true;
		}

		private bool TryEditBankNumber(ModeratorTable table, string value, out string message)
		{
			message = "";

			if (CheckBankNumber(value, out message)) return false;
			if (value == table.BankNumber)
			{
				message = "Старый и новый банковские счета совпадают.";
				return false;
			}

			table.BankNumber = value;

			return true;
		}

		private bool TryEditNickname(ModeratorTable table, string value, out string message)
		{
			message = "";

			if (CheckNickname(value, out message)) return false;
			if (value == table.Nickname)
			{
				message = "Старый и новый никнеймы совпадают.";
				return false;
			}

			table.Nickname = value;

			return true;
		}

		private bool TryEditServerName(ModeratorTable table, string value, out string message)
		{
			message = "";

			if (!Enum.TryParse<ServerName>(value, out var serverName))
			{
				message = "Некорректно указано название сервера.";
				return false;
			}

			if (serverName == table.ServerName)
			{
				message = "Старый и новый сервера совпадают.";
				return false;
			}

			table.ServerName = serverName;

			return true;
		}

		private bool TryEditForumLink(ModeratorTable table, string value, out string message)
		{
			message = "";

			if (CheckForumLink(value, out message)) return false;
			if (value == table.ForumLink)
			{
				message = "Старая и новая ссылки на форумный аккаунт совпадают.";
				return false;
			}

			table.ForumLink = value;

			return true;
		}

		private void RemoveModeratorTable(ModeratorTable table, string dismissionReason, string reinstatement, out string message)
		{
			message = "";

			_serverServiceAccessor.Service.DatabaseManager.RemoveTable(table).GetResult();

			var dismissedTable = new DismissedModeratorTable()
			{
				Id = table.Id,
				User = table.User,
				PermissionLevel = table.PermissionLevel,
				Nickname = table.Nickname,
				Sid = table.Sid,
				DecisionDate = table.DecisionDate,
				DismissionDate = DateTime.Now,
				Reason = dismissionReason,
				Reinstatement = reinstatement,
				ForumLink = table.ForumLink
			};

			_serverServiceAccessor.Service.DatabaseManager.AddOrUpdateTableDB(dismissedTable);

			message = $"Вы сняли {table.Id.GetMention(MentionType.Username)} с поста модератора по причине: {dismissionReason}.";
			message += table.PermissionLevel >= PermissionLevel.Moderator ? $"\nСнять покрас: {table.ForumLink}" : null;
		}

		private bool CheckModeratorTableExists(ulong id, out ModeratorTable table, out string message)
			=> !CheckModeratorTableNotExists(id, out table, out message);

		private bool CheckModeratorTableNotExists(ulong id, out ModeratorTable table, out string message) 
		{
			message = "";
			
			table = _serverServiceAccessor.Service.DatabaseManager.GetTable<ModeratorTable>(id).Result;

			bool result = table == null;
			if (result)
			{
				message = "Данного пользователя нет в списке модераторов.";
			}
			else
			{
				message = "Данный пользователь уже присутствует в списке модераторов.";
			}
			
			return result;
		}

		private bool CheckNickname(string name, out string message)
		{
			message = "";

			if (!Regex.IsMatch(name, @"^[A-Z]{1,}[A-z]*\s[A-Z]{1,}[A-z]*$"))
			{
				message = "Неверный формат никнейма.";
				return true;
			}

			return false;
		}

		private bool CheckSid(string sid, out string message)
		{
			message = "";

			if (!Regex.IsMatch(sid, @"^[A-Z0-9]*$"))
			{
				message = "Неверный формат SID.";
				return true;
			}

			return false;
		}

		private bool CheckForumLink(string forumLink, out string message)
		{
			message = "";

			if (!forumLink.Contains("forum.robo-hamster.com"))
			{
				message = "Некорректная ссылка на форумный аккаунт.";
				return true;
			}

			return false;
		}

		private bool CheckBankNumber(string bankNumber, out string message) 
		{
			message = "";

			if (bankNumber.Length != 8 || bankNumber.Any(symbol => !char.IsNumber(symbol)))
			{
				message = "Введён некорректный номер банковского счёта.";
				return true;
			}

			return false;
		}
	}
}
