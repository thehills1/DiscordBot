using DiscordBot.Configs.Rules;
using DiscordBot.Extensions;
using DiscordBot.Server.Commands.ModalForms;
using DSharpPlus.Entities;

namespace DiscordBot.Server.Commands
{
	public class ServerRulesCommandsManager
	{
		private readonly Bot _bot;

		public RulesConfig RulesConfig { get; }

		public ServerRulesCommandsManager(Bot bot, RulesConfig rules)
		{
			_bot = bot;
			RulesConfig = rules;
		}

		public async Task<CommandResult> TryAddSection(IReadOnlyDictionary<string, string> formValues)
		{
			var result = CheckSectionNumber(formValues[EditSectionModalForm.SectionNumberCustomId], out var number);
			if (!result.Success) return result;
			
			var description = formValues[EditSectionModalForm.SectionDescriptionCustomId];
			var name = formValues[EditSectionModalForm.SectionNameCustomId];

			var section = new RulesSection(number, name, description);
			RulesConfig.AddSection(section);

			RulesConfig.Save();
			await UpdateMessages();

			return new CommandResult(true, "Раздел успешно добавлен.");
		}

		public async Task<CommandResult> TryEditSection(RulesSection oldSection, IReadOnlyDictionary<string, string> formValues)
		{
			var result = CheckSectionNumber(formValues[EditSectionModalForm.SectionNumberCustomId], out var number, true);
			if (!result.Success) return result;

			var description = formValues[EditSectionModalForm.SectionDescriptionCustomId];
			var name = formValues[EditSectionModalForm.SectionNameCustomId];

			oldSection.SetDescription(description);
			oldSection.SetName(name);

			var numberChanged = oldSection.Number != number;
			if (numberChanged) RulesConfig.MoveSection(oldSection, number);

			RulesConfig.Save();

			await UpdateMessages(numberChanged, numberChanged ? 0 : number);

			return new CommandResult(true, "Раздел успешно изменён.");
		}

		public async Task<CommandResult> TryRemoveSection(string rawNumber)
		{
			var result = CheckSectionNumber(rawNumber, out var number, true);
			if (!result.Success) return result;

			var section = RulesConfig.Sections[number - 1];
			foreach (var messageId in section.MessageIds)
			{
				await _bot.DeleteMessageAsync(RulesConfig.RulesChannelId, messageId);
			}

			RulesConfig.RemoveSection(section);

			RulesConfig.Save();

			var sectionNumbersToUpdate = RulesConfig.Sections.Where(section => section.Number >= number).Select(section => section.Number).ToArray();
			await UpdateMessages(editingSectionsNumbers: sectionNumbersToUpdate);

			return new CommandResult(true, "Раздел успешно удалён.");
		}

		public async Task<CommandResult> TryAddRule(IReadOnlyDictionary<string, string> formValues)
		{
			var rawSectionAndRuleNumbers = formValues[EditRuleModalForm.SectionAndRuleNumberCustomId];
			var result = CheckFullRuleNumber(rawSectionAndRuleNumbers, out var sectionNumber, out var ruleNumber, out _);
			if (!result.Success) return result;

			result = CheckRuleSubNumber(formValues[EditRuleModalForm.RuleSubNumberCustomId], out var ruleSubNumber);
			if (!result.Success) return result;

			var ruleName = formValues[EditRuleModalForm.RuleNameCustomId];
			var rulePunishment = formValues[EditRuleModalForm.RulePunishmentCustomId];
			var notes = formValues[EditRuleModalForm.RuleNotesCustomId];
			var notesList = notes.IsNullOrEmpty() ? new List<string>() : notes.Split("\n").Where(str => str != string.Empty).ToList();

			var rule = new Rule(sectionNumber, ruleNumber, ruleName, ruleSubNumber, rulePunishment, notesList);

			RulesConfig.Sections[sectionNumber - 1].AddRule(rule);
			RulesConfig.Save();

			await UpdateMessages(editingSectionsNumbers: sectionNumber);

			return new CommandResult(true, "Правило успешно добавлено.");
		}

		public async Task<CommandResult> TryEditRule(Rule oldRule, IReadOnlyDictionary<string, string> formValues)
		{
			var rawSectionAndRuleNumbers = formValues[EditRuleModalForm.SectionAndRuleNumberCustomId];
			var result = CheckFullRuleNumber(rawSectionAndRuleNumbers, out var sectionNumber, out var ruleNumber, out _, true);
			if (!result.Success) return result;

			result = CheckRuleSubNumber(formValues[EditRuleModalForm.RuleSubNumberCustomId], out var ruleSubNumber);
			if (!result.Success) return result;
			
			var ruleName = formValues[EditRuleModalForm.RuleNameCustomId];
			var rulePunishment = formValues[EditRuleModalForm.RulePunishmentCustomId];
			var notes = formValues[EditRuleModalForm.RuleNotesCustomId];
			var notesList = notes.IsNullOrEmpty() ? new List<string>() : notes.Split("\n").ToList();
			
			oldRule.SetName(ruleName);
			oldRule.SetPunishment(rulePunishment);

			oldRule.ClearNotes();
			foreach (var note in notesList)
			{
				oldRule.AddNote(note);
			}

			var sectionNumberChanged = oldRule.SectionNumber != sectionNumber;
			var oldSectionNumber = oldRule.SectionNumber;
			if (oldRule.SectionNumber == sectionNumber && oldRule.Number != ruleNumber)
			{
				var currentSection = RulesConfig.Sections[sectionNumber - 1];
				currentSection.MoveRule(oldRule, ruleNumber, ruleSubNumber);
			}
			else
			{
				RulesConfig.Sections[oldRule.SectionNumber - 1].RemoveRule(oldRule);

				oldRule.SetSectionNumber(sectionNumber);
				oldRule.SetNumber(ruleNumber);
				oldRule.SetSubNumber(ruleSubNumber);

				RulesConfig.Sections[sectionNumber - 1].AddRule(oldRule);
			}

			RulesConfig.Save();

			var editingSectionsNumbers = sectionNumberChanged ? new int[] { oldSectionNumber, oldRule.SectionNumber } : new int[] { oldRule.SectionNumber };
			await UpdateMessages(editingSectionsNumbers: editingSectionsNumbers);

			return new CommandResult(true, "Правило успешно изменено.");
		}

		public async Task<CommandResult> TryRemoveRule(string rawFullRuleNumber)
		{
			var result = CheckFullRuleNumber(rawFullRuleNumber, out var sectionNumber, out var ruleNumber, out _, true);
			if (!result.Success) return result;

			var currentSection = RulesConfig.Sections[sectionNumber - 1];
			currentSection.RemoveRule(currentSection.Rules[ruleNumber - 1]);

			RulesConfig.Save();

			await UpdateMessages(editingSectionsNumbers: sectionNumber);

			return new CommandResult(true, "Правило успешно удалено.");
		}

		public CommandResult TryGetSectionByNumber(string rawSectionNumber, out RulesSection section)
		{
			section = null;

			var result = CheckSectionNumber(rawSectionNumber, out var sectionNumber);
			if (!result.Success) return result;

			section = RulesConfig.Sections[sectionNumber - 1];
			return new CommandResult(true);
		}

		public CommandResult TryGetRuleByNumber(string rawSectionAndRuleNumbers, out Rule rule)
		{
			rule = null;

			var result = CheckFullRuleNumber(rawSectionAndRuleNumbers, out var sectionNumber, out var ruleNumber, out var ruleSubNumber, true);
			if (!result.Success) return result;

			rule = RulesConfig.Sections[sectionNumber - 1].Rules.First(r => r.Number == ruleNumber && r.SubNumber == ruleSubNumber);
			return new CommandResult(true);
		}

		public CommandResult GenerateForumText(InteractionContext context)
		{
			RulesConfig.GenerateForumText(_bot, context.Guild);

			return new CommandResult(true, "Файл успешно сгенерирован.");
		}

		public async Task UpdateMessages(bool fullUpdate = false, params int[] editingSectionsNumbers)
		{
			if (!RulesConfig.Sections.Any()) return;

			var channelToSend = await _bot.GetChannelAsync(RulesConfig.RulesChannelId);
			if (fullUpdate || RulesConfig.Sections.Any(section => section.MessageIds == null || section.MessageIds?.Count == 0))
			{
				ClearSectionsMessages(channelToSend);

				foreach (var section in RulesConfig.Sections)
				{
					var messagesToSend = section.GenerateMessagesToSend();
					foreach (var message in messagesToSend)
					{
						var sentMessage = await message.SendAsync(channelToSend);
						section.AddMessageId(sentMessage.Id);
					}
				}

				RulesConfig.Save();
				return;
			}

			if (RulesConfig.Sections.Any(section => section.MessageIds.Count != section.GenerateMessagesToSend().Count))
			{
				ClearSectionsMessages(channelToSend);

				await UpdateMessages(true);
				return;
			}		

			foreach (var editingSectionNumber in editingSectionsNumbers)
			{
				if (editingSectionNumber <= 0 || editingSectionNumber > RulesConfig.Sections.Count) return;

				var editingSection = RulesConfig.Sections[editingSectionNumber - 1];
				var messagesToUpdate = editingSection.GenerateMessagesToSend();
				for (int i = 0; i < editingSection.MessageIds.Count; i++)
				{
					var result = await _bot.EditMessageAsync(channelToSend, editingSection.MessageIds[i], messagesToUpdate[i].Content, messagesToUpdate[i].Embeds.ToList());
					if (result == null)
					{
						editingSection.RemoveMessageId(editingSection.MessageIds[i]);
						await UpdateMessages();
						return;
					}
				}
			}
		}

		private async Task ClearSectionsMessages(DiscordChannel channel)
		{
			foreach (var section in RulesConfig.Sections)
			{
				foreach (var oldMessageId in section?.MessageIds)
				{
					await _bot.DeleteMessageAsync(channel, oldMessageId);
				}

				section.ClearMessageIds();
			}
		}

		private CommandResult CheckFullRuleNumber(string rawFullRuleNumbers, out int sectionNumber, out int ruleNumber, out int ruleSubNumber, bool checkRuleExists = false)
		{
			sectionNumber = ruleNumber = ruleSubNumber = 0;

			var split = rawFullRuleNumbers.Split('.');
			if (!rawFullRuleNumbers.Contains('.') || split.Length < 2 || split.Length > 3) return new CommandResult(false, "Неверно указаны номер раздела и правила.");

			var result = CheckSectionNumber(split[0], out sectionNumber, true);
			if (!result.Success) return result;

			result = CheckRuleNumber(sectionNumber, split[1], out ruleNumber, checkRuleExists);
			if (!result.Success) return result;

			if (split.Length == 3)
			{
				result = CheckRuleSubNumber(split[2], out ruleSubNumber);
				if (!result.Success) return result;
			}

			return new CommandResult(true);
		}

		private CommandResult CheckSectionNumber(string rawSectionNumber, out int sectionNumber, bool checkOnExists = false)
		{
			if (!int.TryParse(rawSectionNumber, out sectionNumber))
			{
				return new CommandResult(false, "Введён некорректный номер раздела правил.");
			}

			if (sectionNumber <= 0)
			{
				return new CommandResult(false, "Номер раздела не может быть меньше нуля или равен нулю.");
			}

			if (sectionNumber > RulesConfig.Sections.Count + 1)
			{
				return new CommandResult(false, "Нельзя добавить раздел правил, номер которого больше чем на 1 по сравнению с последним.");
			}

			if (checkOnExists)
			{
				if (RulesConfig.Sections.Count < sectionNumber) return new CommandResult(false, "Раздел с таким номером не существует.");
			}

			return new CommandResult(true);
		}

		private CommandResult CheckRuleNumber(int sectionNumber, string rawRuleNumber, out int ruleNumber, bool checkOnExists = false)
		{
			if (!int.TryParse(rawRuleNumber, out ruleNumber))
			{
				return new CommandResult(false, "Введён некорректный номер правила.");
			}

			if (ruleNumber <= 0)
			{
				return new CommandResult(false, "Номер правила не может быть меньше нуля или равен нулю.");
			}

			if (ruleNumber > RulesConfig.Sections[sectionNumber - 1].Rules.Count + 1)
			{
				return new CommandResult(false, "Нельзя добавить правило, номер которого больше чем на 1 по сравнению с последним.");
			}

			if (checkOnExists)
			{
				if (RulesConfig.Sections[sectionNumber - 1].Rules.Count < ruleNumber) return new CommandResult(false, "Правило с таким номером не существует.");
			}

			return new CommandResult(true);
		}

		private CommandResult CheckRuleSubNumber(string rawSubNumber, out int subNumber)
		{
			subNumber = 0;

			if (rawSubNumber.IsNullOrEmpty()) return new CommandResult(true);

			if (!int.TryParse(rawSubNumber, out subNumber))
			{
				return new CommandResult(false, "Введён некорректный номер подпункта.");
			}

			if (subNumber < 0)
			{
				return new CommandResult(false, "Номер подпункта не может быть меньше нуля.");
			}

			return new CommandResult(true);
		}
	}
}
