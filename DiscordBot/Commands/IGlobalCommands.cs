using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands
{
	public interface IGlobalCommands
	{
		Task Test(InteractionContext context, string text);
	}
}
