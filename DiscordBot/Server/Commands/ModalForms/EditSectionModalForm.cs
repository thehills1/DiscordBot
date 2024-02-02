using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBot.Server.Commands.ModalForms
{
	public static class EditSectionModalForm
	{
		public const string SectionDescriptionCustomId = "section_description";
		public const string SectionNumberCustomId = "section_number";
		public const string SectionNameCustomId = "section_name";

		private static object _createSync = new();
		private const int MaxSectionNameLength = 240;

		public static int UniqueId = 0;

		public static ModalFormInfo Create(string formTitle)
		{
			return Create(formTitle, null, null, null);
		}

		public static ModalFormInfo Create(string formTitle, string description, string sectionNumber, string sectionName)
		{
			lock (_createSync)
			{
				var customId = $"add_section_form_{UniqueId++}";
				var form = new DiscordInteractionResponseBuilder()
					.WithCustomId(customId)
					.WithTitle(formTitle)
					.AddComponents(new TextInputComponent("Описание раздела", SectionDescriptionCustomId, value: description, required: false, style: TextInputStyle.Paragraph))
					.AddComponents(new TextInputComponent("Номер раздела", SectionNumberCustomId, value: sectionNumber))
					.AddComponents(new TextInputComponent("Название раздела", SectionNameCustomId, value: sectionName, max_length: MaxSectionNameLength));
				return new ModalFormInfo(customId, form);
			}
		}
	}
}
