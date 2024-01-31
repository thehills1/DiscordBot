using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands.AutocompleteProviders
{
	public class RulesListAutocompleteProvider : AutocompleteProvider
	{
		public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext context)
		{
			var serverService = ((ServiceManager) context.Services.GetService(typeof(ServiceManager))).GetServerService(context.Guild.Id);
			var rules = serverService.ServerRulesCommands.CommandsManager.Rules;

			var ruleNamePart = context.OptionValue.ToString();
			foreach (var section in rules.Sections)
			{
				foreach (var rule in section.Rules)
				{
					if (rule.Name.ToLower().Contains(ruleNamePart.ToLower()))
					{
						var sectionAndRuleNumber = $"{rule.SectionNumber}.{rule.Number}";
						Choices.Add(new DiscordAutoCompleteChoice($"{sectionAndRuleNumber}.{rule.Name}", sectionAndRuleNumber));
					}
				}
			}

			return Choices;
		}
	}
}
