using DiscordBot.Database.Tables;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands.AutocompleteProviders
{
	public class ShopsAutocompleteProvider : AutocompleteProvider
	{
		public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext context)
		{
			var serverService = ((ServiceManager) context.Services.GetService(typeof(ServiceManager))).GetServerService(context.Guild.Id);
			var shopTables = serverService.DatabaseManager.GetMultyDataDB<ShopTable>().Result;

			foreach (var shopTable in shopTables)
			{
				Choices.Add(new DiscordAutoCompleteChoice(shopTable.Name, shopTable.Name));
			}

			return Choices;
		}
	}
}
