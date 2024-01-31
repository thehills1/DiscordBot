using System.Text;
using DiscordBot.Extensions;
using DSharpPlus.Entities;

namespace DiscordBot.Configs
{
	public class RulesConfig : BaseConfig<RulesConfig>
	{
		public ulong RulesChannelId { get; set; }

		public List<RulesSection> Sections { get; set; }

		public void UpdateNumbers()
		{
			for (int i = 1; i <= Sections.Count; i++)
			{
				Sections[i - 1].Number = i;
			}
		}
	}

	public class RulesSection
	{
		private const int MaxEmbedContentLength = 4096;
		private const int MaxMessageContentLength = 6000;

		public string Description { get; set; }

		public int Number { get; set; }

		public string Name { get; set; }

		public List<Rule> Rules { get; set; }

		public List<ulong> MessageIds { get; set; }

		public List<DiscordMessageBuilder> GenerateMessagesToSend()
		{
			var messages = new List<DiscordMessageBuilder>();

			var totalCharacters = 0;
			var charactersInEmbed = 0;
			var embedTitle = $"__**Раздел {Number}. {Name}**__";

			var currentMessage = new DiscordMessageBuilder();
			if (!Description.IsNullOrEmpty()) currentMessage = currentMessage.WithContent(Description);

			var currentEmbed = new DiscordEmbedBuilder() { Title = embedTitle, Color = DiscordRgbColor.LightBlue };
			
			foreach (var rule in Rules)
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

		public void AddRule(Rule rule)
		{
			if (rule.Number > Rules.Count)
			{
				Rules.Add(rule);
			}
			else
			{
				Rules.Insert(rule.Number - 1, rule);
				IncrementNumbers(rule.Number);
			}
		}

		public void IncrementNumbers(int startFromIndex = 0)
		{
			for (int i = startFromIndex; i < Rules.Count; i++)
			{
				Rules[i].Number++;
			}
		}

		public void DecrementNumbers(int startFromIndex = 0)
		{
			for (int i = startFromIndex; i < Rules.Count; i++)
			{
				Rules[i].Number--;
			}
		}
	}

	public class Rule
	{
		public int SectionNumber { get; set; }

		public int Number { get; set; }

		public int SubNumber { get; set; }

		public string Name { get; set; }

		public string Punishment { get; set; }

		public List<string> Notes { get; set; }

		public override string ToString()
		{
			var sb = new StringBuilder();
			var ruleNumber = SubNumber == 0 ? $"{SectionNumber}.{Number}" : $"{SectionNumber}.{Number}.{SubNumber}";
			sb.AppendLine($"**{ruleNumber}.{Name}**");

			if (!Punishment.IsNullOrEmpty()) sb.AppendLine(Punishment);

			if (Notes?.Count > 0)
			{
				sb.AppendLine();
				sb.AppendLine($"Примечания к правилу {ruleNumber}:");
				for (int i = 1; i <= Notes.Count; i++)
				{
					sb.AppendLine($"{i}. {Notes[i - 1]}");
					sb.AppendLine();
				}
			}

			sb.AppendLine();

			return sb.ToString();
		}
	}
}
