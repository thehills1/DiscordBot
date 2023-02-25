using DiscordBot.Commands.AutocompleteProviders;
using DiscordBot.Database.Enums;
using DiscordBot.Database.Tables;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands
{
	[SlashModuleLifespan(SlashModuleLifespan.Singleton)]
	public class GlobalCommands : ApplicationCommandModule, IGlobalCommands
	{
		private readonly ServiceManager _serviceManager;

		public GlobalCommands(ServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[SlashCommand("addmod", "Постановить участника на пост модератора.", false)]
		public async Task AddModerator(
			InteractionContext context,
			[Option("User", "Пользователь")] DiscordUser user,
			[Option("Permission", "Уровень доступа")] PermissionLevel permissionLevel,
			[Option("Reprimands", "Выговоры")] long reprimands,
			[Option("Nickname", "Ник в игре")] string nickname,
			[Option("SID", "SID")] string sid,
			[Option("Server", "Название сервера")] ServerName serverName,
			[Option("Bank", "Номер банковского счёта")] 
			[MinimumLength(8)] 
			[MaximumLength(8)] string bankNumber,
			[Option("Forum", "Ссылка на форумный аккаунт")] string forumLink)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.AddModerator(context, user, permissionLevel, reprimands, nickname, sid, serverName, bankNumber, forumLink);
		}

		[SlashCommand("makemod", "Измененить уровень доступа модератора.", false)]
		public async Task SetModeratorPermissionLevel(
			InteractionContext context,
			[Option("User", "Пользователь")] DiscordUser user,
			[Option("Permission", "Уровень доступа")] PermissionLevel permissionLevel,
			[Option("Reason", "Причина снятия")] string dismissionReason = null,
			[Option("Reinstatement", "Восстановление")] 
			[Choice("Да", "да")] 
			[Choice("Нет", "нет")] string reinstatement = null)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.SetModeratorPermissionLevel(context, user, permissionLevel, dismissionReason, reinstatement);
		}

		[SlashCommand("mwarn", "Выдать предупреждение модератору.", false)]
		public async Task WarnModerator(InteractionContext context, [Option("user", "Пользователь")] DiscordUser user)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.WarnModerator(context, user);
		}

		[SlashCommand("editinfo", "Изменить информацию о модераторе.", false)]
		public async Task EditModeratorInfo(
			InteractionContext context, 
			[Option("User", "Пользователь")] DiscordUser user,
			[Option("Property", "Изменяемые данные")] [Autocomplete(typeof(TablePropertyAutocompleteProvider<ModeratorTable>))] string property,
			[Option("Value", "Новое значение")] [Autocomplete(typeof(TablePropertyValueAutocompleteProvider<ModeratorTable>))] string value)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands.
				EditModeratorInfo(context, user, property, value);
		}
	}
}
