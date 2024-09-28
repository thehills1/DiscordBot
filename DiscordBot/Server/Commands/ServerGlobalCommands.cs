using DiscordBot.Commands;
using DiscordBot.Database.Enums;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Server.Commands
{
	public class ServerGlobalCommands : BaseServerCommands, IGlobalCommands
	{
		private readonly ServerGlobalCommandsManager _commandsManager;

		public ServerGlobalCommands(ServerGlobalCommandsManager commandsManager)
		{
			_commandsManager = commandsManager;
		}

		public async Task AddModerator(
			InteractionContext context, 
			DiscordUser user,
			PermissionLevel permissionLevel,
			long reprimands, 
			string nickname, 
			string sid, 
			ServerName serverName, 
			string bankNumber, 
			string forumLink)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryAddModeratorTable(
				user, 
				permissionLevel, 
				DateTime.Now, 
				reprimands, 
				nickname, 
				sid, 
				serverName, 
				bankNumber, 
				forumLink);

			SendCommandExecutionResult(context, result);
		}

		public async Task SetModeratorPermissionLevel(
			InteractionContext context, 
			DiscordUser user, 
			PermissionLevel permissionLevel, 
			string dismissionReason = null, 
			string reinstatement = null)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TrySetModeratorPermissionLevelAsync(user, permissionLevel, dismissionReason, reinstatement);

			SendCommandExecutionResult(context, result);
		}

		public async Task WarnModerator(InteractionContext context, DiscordUser user)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryWarnModeratorAsync(user);

			SendCommandExecutionResult(context, result);
		}

		public async Task UnWarnModerator(InteractionContext context, DiscordUser user)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryUnWarnModerator(user);

			SendCommandExecutionResult(context, result);
		}

		public async Task EditModeratorInfo(InteractionContext context, DiscordUser user, string property, string value)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryEditModeratorInfo(user, property, value);

			SendCommandExecutionResult(context, result);
		}

		public async Task SendStaffInfo(InteractionContext context, DiscordChannel channel, bool allTables)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TrySendExcelStaffWorksheetAsync(channel, allTables);

			SendCommandExecutionResult(context, result);
		}

		public async Task SendExcelSalaryWorksheet(InteractionContext context, long weeks = 2)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TrySendExcelSalaryWorksheetAsync((int) weeks);

			SendCommandExecutionResult(context, result);
		}

		public async Task GetModeratorSalaryInfo(InteractionContext context, DiscordUser user = null)
		{
			await context.DeferAsync(true);

			var author = user == null;
			var result = await _commandsManager.TryGetModeratorSalaryInfoAsync(author ? context.User.Id : user.Id, author);

			SendCommandExecutionResult(context, result);
		}

		public async Task SetNorm(InteractionContext context, long count)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TrySetNorm((int) count);

			SendCommandExecutionResult(context, result);
		}
	}
}
