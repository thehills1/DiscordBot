using DiscordBot.Commands.AutocompleteProviders;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands
{
	[SlashModuleLifespan(SlashModuleLifespan.Singleton)]
	[SlashCommandGroup("rules", "Команды для редактирования списка правил.", false)]
	public class RulesCommands : ApplicationCommandModule, IRulesCommands
	{
		private readonly ServiceManager _serviceManager;

		public RulesCommands(ServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[SlashCommand("addsection", "Добавить раздел правил.", false)]
		public async Task AddSection(InteractionContext context)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerRulesCommands.AddSection(context);
		}

		[SlashCommand("editsection", "Редактировать раздел правил.", false)]
		public async Task EditSection(InteractionContext context,
			[Option(nameof(section), "Раздел, который нужно отредактировать.", true)][Autocomplete(typeof(SectionsListAutocompleteProvider))] string section)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerRulesCommands.EditSection(context, section);
		}

		[SlashCommand("removesection", "Удалить раздел правил.", false)]
		public async Task RemoveSection(InteractionContext context,
			[Option(nameof(section), "Раздел, который нужно удалить.", true)][Autocomplete(typeof(SectionsListAutocompleteProvider))] string section)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerRulesCommands.RemoveSection(context, section);
		}

		[SlashCommand("addrule", "Добавить правило в раздел.", false)]
		public async Task AddRule(InteractionContext context)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerRulesCommands.AddRule(context);
		}

		[SlashCommand("editrule", "Редактировать правило.", false)]
		public async Task EditRule(InteractionContext context, 
			[Option(nameof(rule), "Правило, которое нужно отредактировать.", true)][Autocomplete(typeof(RulesListAutocompleteProvider))] string rule)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerRulesCommands.EditRule(context, rule);
		}

		[SlashCommand("removerule", "Редактировать правило.", false)]
		public async Task RemoveRule(InteractionContext context,
			[Option(nameof(rule), "Правило, которое нужно отредактировать.", true)][Autocomplete(typeof(RulesListAutocompleteProvider))] string rule)
		{
			await _serviceManager.GetServerService(context.Guild.Id).ServerRulesCommands.RemoveRule(context, rule);
		}
	}
}
