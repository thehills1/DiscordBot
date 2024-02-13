using System.Collections.ObjectModel;
using DiscordBot.Extensions;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace DiscordBot.Configs.Rules
{
	public class RulesSection
	{
		private const int MaxEmbedContentLength = 4096;
		private const int MaxMessageContentLength = 6000;

		[JsonProperty]
		public string Description { get; private set; }

		[JsonProperty]
		public int Number { get; private set; }

		[JsonProperty]
		public string Name { get; private set; }

		[JsonIgnore]
		public ReadOnlyCollection<Rule> Rules => new ReadOnlyCollection<Rule>(_rules);

		[JsonProperty(nameof(Rules))]
		private List<Rule> _rules = new();

		[JsonIgnore]
		public ReadOnlyCollection<ulong> MessageIds => new ReadOnlyCollection<ulong>(_messageIds);

		[JsonProperty(nameof(MessageIds))]
		private List<ulong> _messageIds = new();

		public RulesSection(int number, string name, string description = null)
		{
			Number = number;
			Name = name;
			Description = description;
		}

		public void SetDescription(string description)
		{
			if (description == null) description = string.Empty;

			Description = description;
		}

		public void SetNumber(int number)
		{
			if (number <= 0) throw new ArgumentException($"Номер раздела не может быть меньше нуля или равен нулю, [{nameof(number)}]=[{number}].");

			Number = number;

			foreach (var rule in _rules)
			{
				rule.SetSectionNumber(number);
			}
		}

		public void SetName(string name)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentException($"Имя раздела не может быть пустым или null, [{nameof(name)}]=[{name}].");

			Name = name;
		}

		public void AddRule(Rule rule)
		{
			if (rule.SectionNumber != Number)
			{
				throw new Exception($"Невозможно вставить правило из другого раздела в данный. [RuleSectionNumber]=[{rule.SectionNumber}]; [SectionNumber]=[{Number}]");
			}

			if (rule.Number > _rules.Count)
			{
				_rules.Add(rule);
				rule.SetNumber(_rules.Count);
				return;
			}

			var ruleAtInsertingNumber = _rules[rule.Number - 1];
			var rulesWithSameNumber = _rules.Where(r => r.Number == rule.Number).ToList();
			if (rule.SubNumber == 0 || ruleAtInsertingNumber.SubNumber == 0 || !rulesWithSameNumber.Any())
			{
				_rules.Insert(rule.Number - 1, rule);
				IncrementNumbers(rule.Number);
				return;
			}

			if (rule.SubNumber > rulesWithSameNumber.Count)
			{
				var lastRuleWithSameNumber = rulesWithSameNumber.Last();
				_rules.Insert(_rules.IndexOf(lastRuleWithSameNumber) + 1, rule);
				rule.SetSubNumber(lastRuleWithSameNumber.SubNumber + 1);
			}
			else
			{
				var ruleWithSameSubNumber = rulesWithSameNumber.First(r => r.SubNumber == rule.SubNumber);
				var indexToInsert = _rules.IndexOf(ruleWithSameSubNumber);
				_rules.Insert(indexToInsert, rule);
				for (int i = rulesWithSameNumber.IndexOf(ruleWithSameSubNumber); i < rulesWithSameNumber.Count; i++)
				{
					rulesWithSameNumber[i].SetSubNumber(rulesWithSameNumber[i].SubNumber + 1);
				}
			}
		}

		public void MoveRule(Rule rule, int newNumber, int newSubNumber)
		{
			if (!_rules.Contains(rule)) return;
			if (_rules.IndexOf(rule) + 1 == rule.Number && rule.SubNumber == newSubNumber) return;

			RemoveRule(rule);

			rule.SetNumber(newNumber);
			rule.SetSubNumber(newSubNumber);

			AddRule(rule);
		}

		public void RemoveRule(Rule rule)
		{
			if (!_rules.Contains(rule)) return;

			_rules.Remove(rule);

			var rulesWithCurrentNumber = _rules.Where(r => r.Number == rule.Number && r.SubNumber > rule.SubNumber);
			if (rule.SubNumber == 0 || rulesWithCurrentNumber.Count() == 0)
			{
				DecrementNumbers(rule.Number - 1);
				return;
			}
			
			foreach (var ruleToUpdate in rulesWithCurrentNumber)
			{
				ruleToUpdate.SetSubNumber(ruleToUpdate.SubNumber - 1);
			}
		}

		public void AddMessageId(ulong messageId)
		{
			_messageIds.Add(messageId);
		}

		public void RemoveMessageId(ulong messageId)
		{
			_messageIds.Remove(messageId);
		}

		public void ClearMessageIds()
		{
			_messageIds.Clear();
		}

		public List<DiscordMessageBuilder> GenerateMessagesToSend()
		{
			var messages = new List<DiscordMessageBuilder>();

			var totalCharacters = 0;
			var charactersInEmbed = 0;
			var embedTitle = $"__**Раздел {Number}. {Name}**__";

			var currentMessage = new DiscordMessageBuilder();
			if (!Description.IsNullOrEmpty()) currentMessage = currentMessage.WithContent(Description);

			var currentEmbed = new DiscordEmbedBuilder() { Title = embedTitle, Color = DiscordRgbColor.LightBlue };

			foreach (var rule in _rules)
			{
				var stringRule = rule.ToString();
				if (totalCharacters + stringRule.Length > MaxMessageContentLength)
				{
					currentMessage.AddEmbed(currentEmbed);
					messages.Add(currentMessage);

					currentMessage = new DiscordMessageBuilder();
					currentEmbed = new DiscordEmbedBuilder() { Title = embedTitle, Color = DiscordRgbColor.LightBlue };

					totalCharacters = 0;
					charactersInEmbed = 0;
				}

				if (charactersInEmbed + stringRule.Length > MaxEmbedContentLength)
				{
					currentMessage.AddEmbed(currentEmbed);
					currentEmbed = new DiscordEmbedBuilder() { Title = embedTitle, Color = DiscordRgbColor.LightBlue };
					charactersInEmbed = 0;
				}

				currentEmbed.Description += stringRule;
				totalCharacters += stringRule.Length;
				charactersInEmbed += stringRule.Length;
			}

			currentMessage.AddEmbed(currentEmbed);
			messages.Add(currentMessage);

			return messages;
		}

		private void IncrementNumbers(int startFromIndex = 0)
		{
			for (int i = startFromIndex; i < _rules.Count; i++)
			{
				var rule = _rules[i];
				rule.SetNumber(rule.Number + 1);
			}
		}

		private void DecrementNumbers(int startFromIndex = 0)
		{
			for (int i = startFromIndex; i < _rules.Count; i++)
			{
				var rule = _rules[i];
				rule.SetNumber(rule.Number - 1);
			}
		}
	}
}
