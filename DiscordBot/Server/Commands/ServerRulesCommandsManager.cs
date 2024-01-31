using DiscordBot.Configs;
using DiscordBot.Extensions;
using DiscordBot.Server.Commands.ModalForms;

namespace DiscordBot.Server.Commands
{
	public class ServerRulesCommandsManager
	{
		private readonly Bot _bot;

		public RulesConfig Rules { get; }

		public ServerRulesCommandsManager(Bot bot, RulesConfig rules)
		{
			_bot = bot;
			Rules = rules;
		}

		public async Task<CommandResult> TryAddSection(IReadOnlyDictionary<string, string> formValues)
		{
			if (!CheckSectionNumber(formValues[EditSectionModalForm.SectionNumberCustomId], out var sectionNumber, out var result)) return result;
			
			var sectionDescription = formValues[EditSectionModalForm.SectionDescriptionCustomId];
			var sectionName = formValues[EditSectionModalForm.SectionNameCustomId];

			var section = new RulesSection() 
			{ 
				Description = sectionDescription,
				Number = sectionNumber, 
				Name = sectionName, 
				Rules = new List<Rule>() 
			};

			if (sectionNumber > Rules.Sections.Count)
			{
				Rules.Sections.Add(section);
			}
			else
			{
				Rules.Sections.Insert(sectionNumber - 1, section);
			}

			Rules.UpdateNumbers();
			Rules.Save();

			await UpdateMessages();

			return new CommandResult(true, "Раздел успешно добавлен.");
		}

		public async Task<CommandResult> TryEditSection(RulesSection oldSection, IReadOnlyDictionary<string, string> formValues)
		{
			if (!CheckSectionNumber(formValues[EditSectionModalForm.SectionNumberCustomId], out var sectionNumber, out var result)) return result;

			var sectionDescription = formValues[EditSectionModalForm.SectionDescriptionCustomId];
			var sectionName = formValues[EditSectionModalForm.SectionNameCustomId];

			oldSection.Description = sectionDescription;
			oldSection.Name = sectionName;

			if (oldSection.Number != sectionNumber)
			{
				if (sectionNumber > Rules.Sections.Count)
				{
					Rules.Sections.Add(oldSection);
				}
				else
				{
					Rules.Sections.Insert(sectionNumber - 1, oldSection);
				}

				Rules.Sections.RemoveAt(oldSection.Number - 1);
			}

			Rules.UpdateNumbers();
			Rules.Save();

			await UpdateMessages();

			return new CommandResult(true, "Раздел успешно добавлен.");
		}

		public async Task<CommandResult> TryRemoveSection(string rawSectionNumber)
		{
			if (!int.TryParse(rawSectionNumber, out var sectionNumber))
			{
				return new CommandResult(false, "Введён некорректный номер раздела.");
			}

			Rules.Sections.RemoveAt(sectionNumber - 1);
			Rules.UpdateNumbers();
			Rules.Save();

			await UpdateMessages();

			return new CommandResult(true, "Раздел успешно удалён.");
		}

		public async Task<CommandResult> TryAddRule(IReadOnlyDictionary<string, string> formValues)
		{
			var rawSectionAndRuleNumbers = formValues[EditRuleModalForm.SectionAndRuleNumberCustomId];
			if (!CheckRuleNumber(rawSectionAndRuleNumbers, out var sectionNumber, out var ruleNumber, out var result)) return result;

			var ruleSubNumber = 0;
			if (!formValues[EditRuleModalForm.RuleSubNumberCustomId].IsNullOrEmpty() && !int.TryParse(formValues[EditRuleModalForm.RuleSubNumberCustomId], out ruleSubNumber))
			{
				return new CommandResult(false, "Введён некорректный подпункт правила.");
			}

			var ruleName = formValues[EditRuleModalForm.RuleNameCustomId];
			var rulePunishment = formValues[EditRuleModalForm.RulePunishmentCustomId];
			var notes = formValues[EditRuleModalForm.RuleNotesCustomId];
			var notesList = notes.IsNullOrEmpty() ? new List<string>() : notes.Split("\n").ToList();

			var rule = new Rule() 
			{ 
				SectionNumber = sectionNumber,
				Number = ruleNumber, 
				SubNumber = ruleSubNumber,
				Name = ruleName, 
				Punishment = rulePunishment,
				Notes = notesList
			};

			Rules.Sections[sectionNumber - 1].AddRule(rule);
			Rules.Save();

			await UpdateMessages();

			return new CommandResult(true, "Правило успешно добавлено.");
		}

		public async Task<CommandResult> TryEditRule(Rule oldRule, IReadOnlyDictionary<string, string> formValues)
		{
			var rawSectionAndRuleNumbers = formValues[EditRuleModalForm.SectionAndRuleNumberCustomId];
			if (!CheckRuleNumber(rawSectionAndRuleNumbers, out var sectionNumber, out var ruleNumber, out var result)) return result;

			var ruleSubNumber = 0;
			if (!formValues[EditRuleModalForm.RuleSubNumberCustomId].IsNullOrEmpty() && !int.TryParse(formValues[EditRuleModalForm.RuleSubNumberCustomId], out ruleSubNumber))
			{
				return new CommandResult(false, "Введён некорректный подпункт правила.");
			}

			var ruleName = formValues[EditRuleModalForm.RuleNameCustomId];
			var rulePunishment = formValues[EditRuleModalForm.RulePunishmentCustomId];
			var notes = formValues[EditRuleModalForm.RuleNotesCustomId];
			var notesList = notes.IsNullOrEmpty() ? new List<string>() : notes.Split("\n").ToList();

			oldRule.Number = ruleNumber;
			oldRule.SubNumber = ruleSubNumber;
			oldRule.Name = ruleName;
			oldRule.Punishment = rulePunishment;
			oldRule.Notes = notesList;

			if (oldRule.SectionNumber != sectionNumber)
			{
				var newSection = Rules.Sections[sectionNumber - 1];
				Rules.Sections[oldRule.SectionNumber - 1].Rules.Remove(oldRule);

				oldRule.SectionNumber = sectionNumber;

				newSection.AddRule(oldRule);
			}

			Rules.Save();

			await UpdateMessages();

			return new CommandResult(true, "Правило успешно изменено.");
		}

		public async Task<CommandResult> TryRemoveRule(string rawSectionAndRuleNumbers)
		{
			if (!rawSectionAndRuleNumbers.Contains('.')) return new CommandResult(false, "Неверно указаны номер раздела и правила.");

			var split = rawSectionAndRuleNumbers.Split('.');
			if (!int.TryParse(split[0], out var sectionNumber))
			{
				return new CommandResult(false, "Введён некорректный номер раздела.");
			}

			if (!int.TryParse(split[1], out var ruleNumber))
			{
				return new CommandResult(false, "Введён некорректный номер правила.");
			}

			Rules.Sections[sectionNumber - 1].Rules.RemoveAt(ruleNumber - 1);
			Rules.Sections[sectionNumber - 1].DecrementNumbers(ruleNumber - 1);

			Rules.Save();

			await UpdateMessages();

			return new CommandResult(true, "Правило успешно удалено.");
		}

		public CommandResult TryGetSectionByNumber(string rawSectionNumber, out RulesSection section)
		{
			section = null;

			if (!int.TryParse(rawSectionNumber, out var sectionNumber)) return new CommandResult(false, "Номер раздела не является числом.");

			section = Rules.Sections[sectionNumber - 1];
			return new CommandResult(true);
		}

		public CommandResult TryGetRuleByNumber(string rawRuleNumber, out Rule rule)
		{
			rule = null;

			if (!rawRuleNumber.Contains('.')) return new CommandResult(false, "Неверно указаны номер раздела и правила.");
			var split = rawRuleNumber.Split('.');
			if (!int.TryParse(split[0], out var sectionNumber)) return new CommandResult(false, "Номер раздела не является числом.");
			if (!int.TryParse(split[1], out var ruleNumber)) return new CommandResult(false, "Номер правила не является числом.");

			rule = Rules.Sections[sectionNumber - 1].Rules[ruleNumber - 1];
			return new CommandResult(true);
		}

		private async Task UpdateMessages()
		{
			var channelToSend = await _bot.GetChannelAsync(Rules.RulesChannelId);
			if (Rules.Sections.Any(section => section.MessageIds == null || section.MessageIds?.Count == 0))
			{
				foreach (var section in Rules.Sections)
				{
					section.MessageIds = new List<ulong>();

					var messagesToSend = section.GenerateMessagesToSend();
					foreach (var message in messagesToSend)
					{
						var sentMessage = await message.SendAsync(channelToSend);
						section.MessageIds.Add(sentMessage.Id);
					}
				}

				Rules.Save();
				return;
			}

			if (Rules.Sections.Any(section => section.MessageIds.Count != section.GenerateMessagesToSend().Count))
			{
				foreach (var section in Rules.Sections)
				{
					section.MessageIds.Clear();
				}

				await UpdateMessages();
				return;
			}

			foreach (var section in Rules.Sections)
			{
				var messagesToUpdate = section.GenerateMessagesToSend();
				for (int i = 0; i < section.MessageIds.Count; i++)
				{
					await _bot.EditMessageAsync(channelToSend, section.MessageIds[i], messagesToUpdate[i].Content, messagesToUpdate[i].Embeds.ToList());
				}
			}
		}

		private bool CheckRuleNumber(string rawSectionAndRuleNumbers, out int sectionNumber, out int ruleNumber, out CommandResult result)
		{
			result = null;
			sectionNumber = ruleNumber = 0;

			if (!rawSectionAndRuleNumbers.Contains('.'))
			{
				result = new CommandResult(false, "Неверно указаны номер раздела и правила.");
				return false;
			}

			var split = rawSectionAndRuleNumbers.Split('.');
			if (!int.TryParse(split[0], out sectionNumber))
			{
				result = new CommandResult(false, "Введён некорректный номер раздела.");
				return false;
			}

			if (!int.TryParse(split[1], out ruleNumber))
			{
				result = new CommandResult(false, "Введён некорректный номер правила.");
				return false;
			}

			if (ruleNumber <= 0)
			{
				result = new CommandResult(false, "Номер правила не может быть меньше нуля или равен нулю.");
				return false;
			}

			if (ruleNumber > Rules.Sections[sectionNumber - 1].Rules.Count + 1)
			{
				result = new CommandResult(false, "Нельзя добавить правило, номер которого больше чем на 1 по сравнению с последним.");
				return false;
			}

			return true;
		}

		private bool CheckSectionNumber(string rawSectionNumber, out int sectionNumber, out CommandResult result)
		{
			result = null;

			if (!int.TryParse(rawSectionNumber, out sectionNumber))
			{
				result = new CommandResult(false, "Введён некорректный номер раздела правил.");
				return false;
			}

			if (sectionNumber <= 0)
			{
				result = new CommandResult(false, "Номер раздела не может быть меньше нуля или равен нулю.");
				return false;
			}

			return true;
		}
	}
}
