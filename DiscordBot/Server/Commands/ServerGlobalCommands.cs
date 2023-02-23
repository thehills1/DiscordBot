using DiscordBot.Commands;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Server.Commands
{
	public class ServerGlobalCommands : IGlobalCommands
	{
		public async Task Test(InteractionContext context, string text)
		{
			//File.Create(Path.Combine(BotEnvironment.ServersDirectoryPath, context.Guild.Id.ToString(), $"{text}.txt"));
			await context.CreateResponseAsync("created");
		}
	}
}
