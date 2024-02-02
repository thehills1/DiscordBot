using DiscordBot.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands.AutocompleteProviders
{
	public class RulesListAutocompleteProvider : AutocompleteProvider
	{
		public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext context)
		{
			var serverService = ((ServiceManager) context.Services.GetService(typeof(ServiceManager))).GetServerService(context.Guild.Id);
			var rules = serverService.ServerRulesCommands.CommandsManager.RulesConfig;

			var ruleNamePart = context.OptionValue.ToString();
			foreach (var section in rules.Sections)
			{
				foreach (var rule in section.Rules)
				{
					if (Choices.Count == 25) return Choices;
					
					var fullRuleNumber = rule.SubNumber == 0 ? $"{rule.SectionNumber}.{rule.Number}" : $"{rule.SectionNumber}.{rule.Number}.{rule.SubNumber}";
					var fullRuleName = $"{fullRuleNumber}.{rule.Name}";
					if (fullRuleName.ToLower().Contains(ruleNamePart.ToLower()))
					{
						Choices.Add(new DiscordAutoCompleteChoice(fullRuleName.Take(100).Join(""), fullRuleNumber));
					}
				}
			}

			return Choices;
		}
	}
}
