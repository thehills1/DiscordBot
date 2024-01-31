using DiscordBot.Commands;
using DiscordBot.Database.Enums;
using DiscordBot.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Server.Commands
{
	public class ServerShopCommands : BaseServerCommands, IShopCommands
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

			var result = await _commandsManager.TryAddShopAsync(owner.Id, serverName, name, paidUntil, firstDeputy?.Id ?? default, secondDeputy?.Id ?? default, imageUrl);

			SendCommandExecutionResult(context, result);
		}

		public async Task AddDeputy(InteractionContext context, string name, DiscordUser deputy)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryAddDeputyAsync(name, deputy.Id);

			SendCommandExecutionResult(context, result);
		}

		public async Task ChangeName(InteractionContext context, string name, string newName)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryChangeNameAsync(name, newName);

			SendCommandExecutionResult(context, result);
		}

		public async Task DeleteShop(InteractionContext context, string name)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryDeleteShopAsync(name);

			SendCommandExecutionResult(context, result);
		}

		public async Task ExtendShop(InteractionContext context, string name, TimeCmd time, long count)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryExtendShopAsync(name, time, (int)count);

			SendCommandExecutionResult(context, result);
		}

		public async Task RemoveDeputy(InteractionContext context, string name, DiscordUser deputy)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryRemoveDeputyAsync(name, deputy.Id);

			SendCommandExecutionResult(context, result);
		}

		public async Task SetOwner(InteractionContext context, string name, DiscordUser newOwner)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TrySetOwnerAsync(name, newOwner.Id);

			SendCommandExecutionResult(context, result);
		}

		public async Task UpdateImage(InteractionContext context, string name, string imageUrl)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryUpdateImageAsync(name, imageUrl);

			SendCommandExecutionResult(context, result);
		}

		public async Task UpdateList(InteractionContext context, ServerName serverName)
		{
			await context.DeferAsync(true);

			var result = await _commandsManager.TryUpdateListAsync(serverName);

			SendCommandExecutionResult(context, result);
		}
	}
}
