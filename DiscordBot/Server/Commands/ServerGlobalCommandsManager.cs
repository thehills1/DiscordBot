using System.Text;
using System.Text.RegularExpressions;
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
using PermissionLevel = DiscordBot.Database.Enums.PermissionLevel;

namespace DiscordBot.Server.Commands
{
	public class ServerGlobalCommandsManager
	{
		private readonly Bot _bot;
		private readonly ServerContext _serverContext;
		private readonly ServerDatabaseManager _databaseManager;
		private readonly SalaryConfig _salaryConfig;
		private readonly ServerConfig _serverConfig;
		private readonly StaffInfoMessagesConfig _infoMessagesConfig;

		public ServerGlobalCommandsManager(
			Bot bot,
			ServerContext serverContext,
			ServerDatabaseManager databaseManager,
			SalaryConfig salaryConfig,
			ServerConfig serverConfig,
			StaffInfoMessagesConfig infoMessagesConfig)
		{
			_bot = bot;
			_serverContext = serverContext;
			_databaseManager = databaseManager;
			_salaryConfig = salaryConfig;
			_serverConfig = serverConfig;
			_infoMessagesConfig = infoMessagesConfig;
		}

		public CommandResult TryAddModeratorTable(
			DiscordUser user,
			PermissionLevel permissionLevel,
            DateTime desicionDate,
            long reprimands,
            string nickname,
            string sid,
			ServerName serverName,
            string bankNumber,
            string forumLink)
        {
            if (CheckModeratorTableExists(user.Id, out _, out var message)) return new CommandResult(false, message);
            if (CheckNickname(nickname, out message)) return new CommandResult(false, message);
            if (CheckSid(sid, out message)) return new CommandResult(false, message);
            if (CheckForumLink(forumLink, out message)) return new CommandResult(false, message);

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

			_databaseManager.AddOrUpdateTableDB(table);

            return new CommandResult(true, $"Добавил {user.Mention} в список модераторов.");
        }

		public async Task<CommandResult> TrySetModeratorPermissionLevelAsync(
			DiscordUser user, 
			PermissionLevel permissionLevel,
			string dismissionReason = null,
			string reinstatement = null)
		{
			if (CheckModeratorTableNotExists(user.Id, out var table, out var message)) return new CommandResult(false, message);
			if (table.PermissionLevel == permissionLevel) return new CommandResult(false, "У модератора уже установлен данный уровень доступа.");
			
			if (permissionLevel == PermissionLevel.None)
			{
				if (dismissionReason == null) return new CommandResult(false, "Вы не указали причину снятия.");

				reinstatement ??= table.Reprimands == 0 ? "да" : "нет";

				return await RemoveModeratorTable(table, dismissionReason, reinstatement);
			}

			table.PermissionLevel = permissionLevel;
			table.PromotionDate = DateTime.Now;

			_databaseManager.AddOrUpdateTableDB(table);

			return new CommandResult(true, $"Вы изменили уровень модератора {user.Mention} на **{permissionLevel}.**");
		}

		public async Task<CommandResult> TryWarnModeratorAsync(DiscordUser user)
		{
			if (CheckModeratorTableNotExists(user.Id, out var table, out var message)) return new CommandResult(false, message);

			if (table.Reprimands == ModeratorTable.ReprimandsLimit - 1)
			{
				return await RemoveModeratorTable(table, $"{ModeratorTable.ReprimandsLimit}/{ModeratorTable.ReprimandsLimit}", "нет");
			}

			table.Reprimands++;
			_databaseManager.AddOrUpdateTableDB(table);

			message = $"Вы выдали модератору {user.Mention} предупреждение. " +
				$"\nТекущее количество предупреждений модератора: **{table.Reprimands}/{ModeratorTable.ReprimandsLimit}.**";
			return new CommandResult(true, message);
		}

		public CommandResult TryUnWarnModerator(DiscordUser user)
		{
			if (CheckModeratorTableNotExists(user.Id, out var table, out var message)) return new CommandResult(false, message);
			if (table.Reprimands == 0) return new CommandResult(false, "У данного модератора 0 выговоров.");

			table.Reprimands--;
			_databaseManager.AddOrUpdateTableDB(table);

			message = $"Вы сняли модератору {user.Mention} предупреждение. " +
				$"\nТекущее количество предупреждений модератора: **{table.Reprimands}/{ModeratorTable.ReprimandsLimit}.**";
			return new CommandResult(true, message);
		}

		public CommandResult TryEditModeratorInfo(DiscordUser user, string property, string value)
		{
			if (CheckModeratorTableNotExists(user.Id, out var table, out var message)) return new CommandResult(false, message);

			switch (property)
			{
				case nameof(ModeratorTable.Sid):
					if (!TryEditSid(table, value, out message)) return new CommandResult(false, message);			
					break;
				case nameof(ModeratorTable.BankNumber):
					if (!TryEditBankNumber(table, value, out message)) return new CommandResult(false, message);
					break;
				case nameof(ModeratorTable.Nickname):
					if (!TryEditNickname(table, value, out message)) return new CommandResult(false, message);					
					break;
				case nameof(ModeratorTable.ServerName):
					if (!TryEditServerName(table, value, out message)) return new CommandResult(false, message);
					break;
				case nameof(ModeratorTable.ForumLink):
					if (!TryEditForumLink(table, value, out message)) return new CommandResult(false, message);				
					break;
			}

			_databaseManager.AddOrUpdateTableDB(table);

			message = $"Вы изменили данные модератора {user.Mention}. " +
				$"\nБыли изменены данные: **{property}.** " +
				$"\nНовое значение: **{value}.**";
			return new CommandResult(true, message);
		}

		public async Task<CommandResult> TrySendExcelStaffWorksheetAsync(DiscordChannel channel, bool allTables = false, bool pinMessage = false)
		{
			var tables = new List<ITableCollection>();

			var modTables = await _databaseManager.GetMultyDataDBDesc<ModeratorTable, PermissionLevel>(table => table.PermissionLevel);
			tables.Add(new TableCollection<ModeratorTable>(modTables));

			await UpdateActualUsernames(modTables);

			if (allTables)
			{
				var dismissedModTables = (await _databaseManager.GetMultyDataDB<DismissedModeratorTable>()).OrderBy(table => table.DismissionDate).ToList();
				tables.Add(new TableCollection<DismissedModeratorTable>(dismissedModTables));
			}

			var fileName = Path.Combine(_serverContext.ModeratorsWorksheetsPath, $"moderators {DateTime.Now.ToString().Replace(":", ".")}.xlsx");
			var messageContent = "Актуальный список модераторов:";

			var oldMessage = _infoMessagesConfig.SentMessagesIds?.FirstOrDefault(msg => msg.ChannelId == channel.Id && msg.AllTables == allTables);
			if (oldMessage is not null)
			{
				await _bot.DeleteMessageAsync(channel, oldMessage.MessageId);

				_infoMessagesConfig.SentMessagesIds.Remove(oldMessage);
			}

			if (!ExcelWorksheetCreator.TryGenerateAndSaveFile(tables, fileName))
			{
				return new CommandResult(false, "Произошла ошибка при создании файла с excel таблицей. Возможно, она была пуста.");
			}
			
			using (var fileStream = new FileStream(fileName, FileMode.Open))
			{
				var sentMessage = await new DiscordMessageBuilder()
					.WithContent(messageContent)
					.AddFile(fileStream)
					.SendAsync(channel);

				if (pinMessage) await sentMessage.PinAsync();

				_infoMessagesConfig.SentMessagesIds ??= new List<MessageInfo>();

				var messageInfo = new MessageInfo()
				{
					ChannelId = channel.Id,
					MessageId = sentMessage.Id,
					AllTables = allTables
				};

				_infoMessagesConfig.SentMessagesIds.Add(messageInfo);
				
				_infoMessagesConfig.Save();
			}

			return new CommandResult(true, $"Лист с таблицами был успешно отправлен в канал {channel.Mention}.");
		}

		public async Task<CommandResult> TrySendExcelSalaryWorksheetAsync(int weeks = 2)
		{
			var channelToDownload = await _bot.GetChannelAsync(_serverConfig.BotStatsChannelId);
			var channelToSend = await _bot.GetChannelAsync(_serverConfig.StatsChannelId);

			var moderatorsActions = GetModeratorsActions(
					channelToDownload, 
					out var periodStartDate, 
					out var periodEndDate, 
					weeks);

			var sortedActions = SortAndRemoveExtraModeratorsActions(moderatorsActions, weeks, out var totalActions);
			var salaryTables = CalculateSalary(sortedActions, weeks, totalActions, periodStartDate, periodEndDate);

			var datesString = $"{periodStartDate.ToShortDateString()} - {periodEndDate.ToShortDateString()}";

			var fileName = Path.Combine(_serverContext.SalaryWorksheetsPath, $"salary {datesString}.xlsx");

			if (!ExcelWorksheetCreator.TryGenerateAndSaveFile(new() { salaryTables }, fileName))
			{
				return new CommandResult(false, "Произошла ошибка при создании файла с excel таблицей. Возможно, она была пуста.");
			}

			using (var fileStream = new FileStream(fileName, FileMode.Open))
			{
				await new DiscordMessageBuilder().WithContent($"Зарплата за {datesString}.").AddFile(fileStream).SendAsync(channelToSend);
			}

			return new CommandResult(true, $"Лист с зарплатой был успешно отправлен в канал {channelToSend.Mention}.");
		}

		public CommandResult TrySetNorm(int count)
		{
			if (count < 0) return new CommandResult(false, "Количество действий не может быть меньше нуля.");

			_salaryConfig.ActionsPerWeekToSalary = count;
			_salaryConfig.Save();

			return new CommandResult(true, $"Норма наказаний успешно изменена на **{count}** в неделю.");
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

			periodStartDate = messages.Last().Timestamp.DateTime.AddDays(-7);
			periodEndDate = messages.First().Timestamp.DateTime;

			return moderatorsActions;
		}

		private Dictionary<ServerName, Dictionary<ModeratorTable, int>> SortAndRemoveExtraModeratorsActions(
			Dictionary<ulong, int> actions, 
			int weeks, 
			out Dictionary<ServerName, int> totalActions)
		{
			var sortedActions = new Dictionary<ServerName, Dictionary<ModeratorTable, int>>();
			totalActions = new Dictionary<ServerName, int>();

			foreach (var moderatorAction in actions)
			{
				var table = _databaseManager.GetTable<ModeratorTable>(moderatorAction.Key).Result;
				if (table == null) continue;

				if (!sortedActions.ContainsKey(table.ServerName))
				{
					sortedActions.Add(table.ServerName, new Dictionary<ModeratorTable, int>());
				}

				var coeff = table.Reprimands switch
				{
					0 => 1f,
					1 => 0.75f,
					2 => 0f
				};

				var moderatorActions = (int) (moderatorAction.Value * coeff);
				sortedActions[table.ServerName].Add(table, moderatorActions);

				if (table.PermissionLevel > PermissionLevel.Curator 
					|| moderatorActions < _salaryConfig.ActionsPerWeekToSalary * weeks
					|| coeff == 0f) continue;

				if (!totalActions.TryAdd(table.ServerName, moderatorActions))
				{
					totalActions[table.ServerName] += moderatorActions;
				}
			}

			return sortedActions;
		}

		private TableCollection<ModeratorSalaryTable> CalculateSalary(
			Dictionary<ServerName, Dictionary<ModeratorTable, int>> actions, 
			int weeks, 
			Dictionary<ServerName, int> totalActions,
			DateTime periodStartDate,
			DateTime periodEndDate)
		{
			var output = new List<ModeratorSalaryTable>();

			foreach (var actionsByServer in actions) 
			{
				foreach (var moderatorActions in actionsByServer.Value)
				{
					var moderatorTable = moderatorActions.Key;
					var salary = CalculateChiefsSalary(moderatorTable, weeks);

					if (moderatorTable.PermissionLevel <= PermissionLevel.Curator)
					{
						salary += CalculateActionsSalary(actionsByServer.Key, moderatorActions.Value, weeks, totalActions[actionsByServer.Key]);
					}

					var salaryTable = new ModeratorSalaryTable()
					{
						Id = moderatorTable.Id,
						User = moderatorTable.User,
						ServerName = moderatorTable.ServerName,
						Nickname = moderatorTable.Nickname,
						BankNumber = moderatorTable.BankNumber,
						ActionsCount = moderatorActions.Value,
						Salary = salary,
						Reprimands = moderatorTable.Reprimands,
						PeriodStartDate = periodStartDate,
						PeriodEndDate = periodEndDate
					};

					_databaseManager.AddTableDB(salaryTable);

					output.Add(salaryTable);
				}

			}

			return new TableCollection<ModeratorSalaryTable>(output);
		}

		private async Task UpdateActualUsernames(List<ModeratorTable> tables)
		{
			foreach (ModeratorTable table in tables)
			{
				var user = await _bot.GetUserAsync(table.Id);
				var actualUsername = user.GetUser();
				if (table.User == actualUsername) continue;

				table.User = actualUsername;

				_databaseManager.AddOrUpdateTableDB(table);
			}
		}

		private int CalculateChiefsSalary(ModeratorTable table, int weeks)
		{
			if (table.PermissionLevel < PermissionLevel.Curator) return 0;

			switch (table.PermissionLevel)
			{
				case PermissionLevel.Curator:
					return _salaryConfig.ChiefsSalary[table.ServerName].Curator * weeks;
				case PermissionLevel.DeputyChiefModerator:
					return _salaryConfig.ChiefsSalary[table.ServerName].DeputyChiefModerator * weeks;
				case PermissionLevel.ChiefModerator:
					return _salaryConfig.ChiefsSalary[table.ServerName].ChiefModerator * weeks;
				default:
					return 0;
			}
		}

		private int CalculateActionsSalary(ServerName server, int moderatorActions, int weeks, int totalActions)
		{
			if (moderatorActions < _salaryConfig.ActionsPerWeekToSalary * weeks) return 0;

			var raw = (int) Math.Ceiling((double) moderatorActions / totalActions * _salaryConfig.Sum[server] * weeks);
			return (int) Math.Floor((double) raw / 1000) * 1000;
        }

		public async Task<CommandResult> TryGetModeratorSalaryInfoAsync(ulong id, bool author)
		{
			if (CheckModeratorTableNotExists(id, out _, out var message)) return new CommandResult(false, message);

			var salaryTables = await _databaseManager.GetMultyDataDBAsync<ModeratorSalaryTable>(table => table.Id == id);
			if (salaryTables == null)
			{
				return new CommandResult(false, author ? "Вы ещё не получали зарплату." : "Зарплата данному пользователю ещё не выплачивалась.");
			}

			var stringBuilder = new StringBuilder();
			if (author)
			{
				stringBuilder.AppendLine("**Ваша зарплата за всё время:**");
			}
			else
			{
				stringBuilder.AppendLine($"**Зарплата пользователя {id.GetMention(MentionType.Username)} за всё время:**");
			}

			stringBuilder.AppendLine();

			foreach (var table in salaryTables)
			{
				stringBuilder.Append($"**С {table.PeriodStartDate.ToShortDateString()} ");
				stringBuilder.Append($"по {table.PeriodEndDate.ToShortDateString()}:** ");
				stringBuilder.Append($"{table.Salary} на сервере **{table.ServerName}.**");
				stringBuilder.AppendLine();
			}

			return new CommandResult(true, stringBuilder.ToString());
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

		private async Task<CommandResult> RemoveModeratorTable(ModeratorTable table, string dismissionReason, string reinstatement)
		{
			await _databaseManager.RemoveTableAsync(table);

			var dismissedTable = new DismissedModeratorTable()
			{
				Id = table.Id,
				User = table.User,
				PermissionLevel = table.PermissionLevel,
				Nickname = table.Nickname,
				Sid = table.Sid,
				ServerName = table.ServerName,
				DecisionDate = table.DecisionDate,
				DismissionDate = DateTime.Now,
				Reason = dismissionReason,
				Reinstatement = reinstatement,
				ForumLink = table.ForumLink
			};

			_databaseManager.AddOrUpdateTableDB(dismissedTable);

			return new CommandResult(true, $"Вы сняли {table.Id.GetMention(MentionType.Username)} с поста модератора по причине: {dismissionReason}.");
		}

		private bool CheckModeratorTableExists(ulong id, out ModeratorTable table, out string message)
			=> !CheckModeratorTableNotExists(id, out table, out message);

		private bool CheckModeratorTableNotExists(ulong id, out ModeratorTable table, out string message) 
		{
			message = "";
			
			table = _databaseManager.GetTable<ModeratorTable>(id).Result;

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

			if (!forumLink.Contains("forum.arizona-v.com"))
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
