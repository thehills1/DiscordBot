using DiscordBot.Database.Tables;
using DiscordBot.Extensions;
using DiscordBot.Extensions.Excel;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands.AutocompleteProviders
{
	public class TablePropertyAutocompleteProvider<T> : AutocompleteProvider where T : BaseTable
	{
		public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext context)
		{
			if (Choices.Any()) return Choices;

			var properties = typeof(T).GetAllowedToChangeProperties();
			foreach (var property in properties)
			{
				var excelColumnAttribute = Attribute.GetCustomAttribute(property, typeof(ExcelColumnAttribute)) as ExcelColumnAttribute;
				Choices.Add(new DiscordAutoCompleteChoice(excelColumnAttribute?.Name ?? property.Name, property.Name));
			}

			return Choices;
		}
	}
}
