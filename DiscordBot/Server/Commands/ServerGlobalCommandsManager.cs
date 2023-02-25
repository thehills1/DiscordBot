using System.Text.RegularExpressions;
using Chloe.Threading.Tasks;
using DiscordBot.Database.Enums;
using DiscordBot.Database.Tables;
using DiscordBot.Extensions;
using DiscordBot.Server.Database;
using DSharpPlus;
using DSharpPlus.Entities;
using PermissionLevel = DiscordBot.Database.Enums.PermissionLevel;

namespace DiscordBot.Server.Commands
{
	public class ServerGlobalCommandsManager
	{
		private readonly ServerDatabaseManager _databaseManager;

		public ServerGlobalCommandsManager(ServerDatabaseManager databaseManager)
		{
			_databaseManager = databaseManager;
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

            _databaseManager.AddOrUpdateTableDB(table);

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
			_databaseManager.AddOrUpdateTableDB(table);

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
			_databaseManager.AddOrUpdateTableDB(table);

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
					if (!TryEdit\Nickname(table, value, out message)) return false;					
					break;
				case nameof(ModeratorTable.ServerName):
					if (!TryEditServerName(table, value, out message)) return false;
					break;
				case nameof(ModeratorTable.ForumLink):
					if (!TryEditForumLink(table, value, out message)) return false;				
					break;
			}

			_databaseManager.AddOrUpdateTableDB(table);

			message = $"Вы изменили данные модератора {user.Mention}. " +
				$"\nБыли изменены данные: **{property}.** " +
				$"\nНовое значение: **{value}.**";
			return true;
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

			_databaseManager.RemoveTable(table).GetResult();

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

			_databaseManager.AddOrUpdateTableDB(dismissedTable);

			message = $"Вы сняли {table.Id.GetMention(MentionType.Username)} с поста модератора по причине: {dismissionReason}.";
			message += table.PermissionLevel >= PermissionLevel.Moderator ? $"\nСнять покрас: {table.ForumLink}" : null;
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
