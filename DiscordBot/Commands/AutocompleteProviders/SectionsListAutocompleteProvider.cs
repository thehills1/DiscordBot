using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands.AutocompleteProviders
{
	public class SectionsListAutocompleteProvider : AutocompleteProvider
	{
		public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext context)
		{
			var serverService = ((ServiceManager) context.Services.GetService(typeof(ServiceManager))).GetServerService(context.Guild.Id);
			var rules = serverService.ServerRulesCommands.CommandsManager.RulesConfig;

			foreach (var section in rules.Sections)
			{
				Choices.Add(new DiscordAutoCompleteChoice($"{section.Number}. {section.Name}", section.Number.ToString()));
			}

			return Choices;
		}
	}
}
