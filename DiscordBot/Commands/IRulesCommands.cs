using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands
{
	public interface IRulesCommands
	{
		Task AddSection(InteractionContext context);

		Task EditSection(InteractionContext context, string section);

		Task RemoveSection(InteractionContext context, string section);

		Task AddRule(InteractionContext context);

		Task EditRule(InteractionContext context, string rule);

		Task RemoveRule(InteractionContext context, string rule);
	}
}
