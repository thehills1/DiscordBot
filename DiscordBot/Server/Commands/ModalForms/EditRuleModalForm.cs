using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBot.Server.Commands.ModalForms
{
	public static class EditRuleModalForm
	{
		public const string SectionAndRuleNumberCustomId = "section_and_rule_number";
		public const string RuleSubNumberCustomId = "rule_subnumber";
		public const string RuleNameCustomId = "rule_name";
		public const string RulePunishmentCustomId = "rule_punishment";
		public const string RuleNotesCustomId = "rule_notes";

		private static object _createSync = new();

		public static int UniqueId = 0;

		public static ModalFormInfo Create(string formTitle)
		{
			return Create(formTitle, null, null, null, null, null);
		}

		public static ModalFormInfo Create(string formTitle, string sectionAndRuleNumber, string ruleSubNumber, string ruleName, string rulePunishment, string ruleNotes)
		{
			lock (_createSync)
			{
				var customId = $"add_rule_form_{UniqueId++}";
				var form = new DiscordInteractionResponseBuilder()
					.WithCustomId(customId)
					.WithTitle(formTitle)
					.AddComponents(new TextInputComponent("Номер раздела и правила:", SectionAndRuleNumberCustomId, "Номер раздела и правила через точку.", sectionAndRuleNumber))
					.AddComponents(new TextInputComponent("Подпункт правила:", RuleSubNumberCustomId, value: ruleSubNumber, required: false))
					.AddComponents(new TextInputComponent("Название правила:", RuleNameCustomId, value: ruleName))
					.AddComponents(new TextInputComponent("Мера наказания:", RulePunishmentCustomId, value: rulePunishment, required: false))
					.AddComponents(
					new TextInputComponent(
						"Примечания:",
						RuleNotesCustomId,
						"Примечания указывайте без нумерации, каждое примечание должно начинаться с новой строки.",
						ruleNotes,
						required: false,
						style: TextInputStyle.Paragraph));

				return new ModalFormInfo(customId, form);
			}
		}
	}
}
