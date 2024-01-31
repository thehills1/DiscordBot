﻿using DiscordBot.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Server.Commands
{
	public abstract class BaseServerCommands
	{
		protected async Task SendCommandExecutionResult(InteractionContext context, CommandResult result)
		{
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
