using DiscordBot.Commands;
using DiscordBot.Database.Enums;
using DiscordBot.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Server.Commands
{
	public class ServerShopCommands : IShopCommands
	{
		private readonly ServerShopCommandsManager _commandsManager;

		public ServerShopCommands(ServerShopCommandsManager commandsManager)
		{
			_commandsManager = commandsManager;
		}

		public async Task Add(
			InteractionContext context, 
			DiscordUser owner, 
			ServerName serverName, 
			string name, 
			string paidUntil, 
			DiscordUser firstDeputy = null, 
			DiscordUser secondDeputy = null, 
			string imageUrl = null)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryAddShop(owner.Id, serverName, name, paidUntil, firstDeputy?.Id ?? default, secondDeputy?.Id ?? default, out var message, imageUrl);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task AddDeputy(InteractionContext context, string name, DiscordUser deputy)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryAddDeputy(name, deputy.Id, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task ChangeName(InteractionContext context, string name, string newName)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryChangeName(name, newName, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task Delete(InteractionContext context, string name)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryDelete(name, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task Extend(InteractionContext context, string name, TimeCmd time, long count)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryExtend(name, time, (int) count, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task RemoveDeputy(InteractionContext context, string name, DiscordUser deputy)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryRemoveDeputy(name, deputy.Id, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task SetOwner(InteractionContext context, string name, DiscordUser newOwner)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TrySetOwner(name, newOwner.Id, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task UpdateImage(InteractionContext context, string name, string imageUrl)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryUpdateImage(name, imageUrl, out var message);

			SendCommandExecutionResult(context, result, message);
		}

		public async Task UpdateList(InteractionContext context, ServerName serverName)
		{
			await context.DeferAsync(true);

			var result = _commandsManager.TryUpdateList(serverName, out var message);

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
