using DiscordBot.Database.Tables;
using DiscordBot.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands.AutocompleteProviders
{
	public class TablePropertyValueAutocompleteProvider<T> : AutocompleteProvider where T : BaseTable
	{
		public override async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext context)
		{
			if (!string.IsNullOrEmpty(context.OptionValue.ToString())) return null;
			if (Choices.Any()) return Choices;

			var properties = typeof(T).GetAllowedToChangeProperties();
			foreach (var property in properties)
			{
				if (!property.PropertyType.IsEnum) continue;

				var enumValues = property.PropertyType.GetEnumValues();
				foreach (var enumValue in enumValues)
				{
					var stringValue = enumValue.ToString();
					Choices.Add(new DiscordAutoCompleteChoice(stringValue, stringValue));
				}
			}

			return Choices;
		}
	}
}
