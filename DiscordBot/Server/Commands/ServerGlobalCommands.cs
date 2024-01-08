using DiscordBot.Commands;
using DiscordBot.Database.Enums;
using DiscordBot.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Server.Commands
{
	public class ServerGlobalCommands : IGlobalCommands
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
				forumLink, 
				out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task SetModeratorPermissionLevel(
			InteractionContext context, 
			DiscordUser user, 
			PermissionLevel permissionLevel, 
			string dismissionReason = null, 
			string reinstatement = null)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TrySetModeratorPermissionLevel(user, permissionLevel, out var message, dismissionReason, reinstatement);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task WarnModerator(InteractionContext context, DiscordUser user)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryWarnModerator(user, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task EditModeratorInfo(InteractionContext context, DiscordUser user, string property, string value)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryEditModeratorInfo(user, property, value, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task SendStaffInfo(InteractionContext context, DiscordChannel channel, bool allTables)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TrySendExcelStaffWorksheet(channel, allTables, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task SendExcelSalaryWorksheet(InteractionContext context, long weeks = 2)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TrySendExcelSalaryWorksheet(out var message, (int) weeks);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task GetModeratorSalaryInfo(InteractionContext context, DiscordUser user = null)
		{
			await context.DeferAsync(true);

			var author = user == null;
			var result = _commandsManager.TryGetModeratorSalaryInfo(author ? context.User.Id : user.Id, author, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task SetNorm(InteractionContext context, long count)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TrySetNorm((int) count, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		private async void SendCommandExecutionResult(InteractionContext context, bool result, string message)
		{
			if (result)
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbedWithSuccessResult(message));
			}
			else
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbedWithErrorResult(message));
			}
		}
	}
}
