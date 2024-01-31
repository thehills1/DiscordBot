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
			[Option(nameof(user), "Пользователь")] DiscordUser user,
			[Option(nameof(permissionLevel), "Уровень доступа")] PermissionLevel permissionLevel,
			[Option(nameof(reprimands), "Выговоры")] long reprimands,
			[Option(nameof(nickname), "Ник в игре")] string nickname,
			[Option(nameof(sid), "SID")] string sid,
			[Option(nameof(serverName), "Название сервера")] ServerName serverName,
			[Option(nameof(bankNumber), "Номер банковского счёта")] 
			[MinimumLength(8)] 
			[MaximumLength(8)] string bankNumber,
			[Option(nameof(forumLink), "Ссылка на форумный аккаунт")] string forumLink)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.AddModerator(context, user, permissionLevel, reprimands, nickname, sid, serverName, bankNumber, forumLink);
		}

		[SlashCommand("makemod", "Измененить уровень доступа модератора.", false)]
		public async Task SetModeratorPermissionLevel(
			InteractionContext context,
			[Option(nameof(user), "Пользователь")] DiscordUser user,
			[Option(nameof(permissionLevel), "Уровень доступа")] PermissionLevel permissionLevel,
			[Option(nameof(dismissionReason ), "Причина снятия")] string dismissionReason = null,
			[Option(nameof(reinstatement), "Восстановление")] 
			[Choice("Да", "да")] 
			[Choice("Нет", "нет")] string reinstatement = null)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.SetModeratorPermissionLevel(context, user, permissionLevel, dismissionReason, reinstatement);
		}

		[SlashCommand("mwarn", "Выдать предупреждение модератору.", false)]
		public async Task WarnModerator(InteractionContext context, 
			[Option(nameof(user), "Пользователь")] DiscordUser user)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.WarnModerator(context, user);
		}

		[SlashCommand("editinfo", "Изменить информацию о модераторе.", false)]
		public async Task EditModeratorInfo(
			InteractionContext context, 
			[Option(nameof(user), "Пользователь")] DiscordUser user,
			[Option(nameof(property), "Изменяемые данные")] [Autocomplete(typeof(TablePropertyAutocompleteProvider<ModeratorTable>))] string property,
			[Option(nameof(value), "Новое значение")] [Autocomplete(typeof(TablePropertyValueAutocompleteProvider<ModeratorTable>))] string value)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.EditModeratorInfo(context, user, property, value);
		}

		[SlashCommand("sendstaffinfo", "Отправить лист excel с данными о составе модераторов.", false)]
		public async Task SendStaffInfo(
			InteractionContext context,
			[Option(nameof(channel), "Канал для отправки")] DiscordChannel channel,
			[Option(nameof(allTables), "Отправить все дополнительные таблицы")] bool allTables)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.SendStaffInfo(context, channel, allTables);
		}

		[SlashCommand("sendsalary", "Отправляет таблицу с зарплатой за прошедшие 2 недели.", false)]
		public async Task SendExcelSalaryWorksheet(
			InteractionContext context,
			[Option(nameof(weeks), "Количество недель, зарплату для которых надо просчитать")] long weeks = 2)
		
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.SendExcelSalaryWorksheet(context, weeks);
		}

		[SlashCommand("salaryinfo", "Вывод информацию о полученной зарплате за всё время.", false)]
		public async Task GetModeratorSalaryInfo(
			InteractionContext context,
			[Option(nameof(user), "Пользователь")] DiscordUser user = null)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.GetModeratorSalaryInfo(context, user);
		}

		[SlashCommand("setnorm", "Изменить норму наказаний в неделю.", false)]
		public async Task SetNorm(InteractionContext context, 
			[Option(nameof(count), "Количество действий за неделю")] long count)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerGlobalCommands
				.SetNorm(context, count);
		}
	}
}
