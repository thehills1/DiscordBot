using DiscordBot.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Server.Commands
{
	public abstract class BaseServerCommands
	{
		protected async Task SendCommandExecutionResult(InteractionContext context, CommandResult result)
		{
			if (context == null) return;
			if (result == null)
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbedWithErrorResult("Произошла неизвестная ошибка, сообщите разработчику."));
				return;
			} 

			if (result.Success)
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbedWithSuccessResult(result.Message));
			}
			else
			{
				await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbedWithErrorResult(result.Message));
			}
		}

		protected async Task SendCommandExecutionResult(DiscordInteraction interaction, CommandResult result)
		{
			if (interaction == null) return;
			if (result == null)
			{
				await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbedWithErrorResult("Произошла неизвестная ошибка, сообщите разработчику."));
				return;
			} 

			if (result.Success)
			{
				await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbedWithSuccessResult(result.Message));
			}
			else
			{
				await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbedWithErrorResult(result.Message));
			}
		}
	}
}
