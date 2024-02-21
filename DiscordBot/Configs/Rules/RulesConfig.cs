using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using DiscordBot.Extensions;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace DiscordBot.Configs.Rules
{
	public class RulesConfig : BaseConfig<RulesConfig>
	{
		[JsonProperty]
		public ulong RulesChannelId { get; private set; }

		[JsonIgnore]
		public ReadOnlyCollection<RulesSection> Sections => new ReadOnlyCollection<RulesSection>(_sections);

		[JsonProperty(nameof(Sections))]
		private List<RulesSection> _sections = new();

		public void AddSection(RulesSection section)
		{
			if (section.Number > _sections.Count)
			{
				_sections.Add(section);
			}
			else
			{
				_sections.Insert(section.Number - 1, section);
			}

			UpdateNumbers();
		}

		public void MoveSection(RulesSection section, int newNumber)
		{
			if (!_sections.Contains(section)) return;
			if (_sections.IndexOf(section) + 1 == newNumber) return;

			_sections.Remove(section);
			
			section.SetNumber(newNumber);
			if (newNumber > _sections.Count)
			{
				_sections.Add(section);
			}
			else
			{
				_sections.Insert(newNumber, section);
			}

			UpdateNumbers();
		}

		public void RemoveSection(RulesSection section)
		{
			if (!_sections.Contains(section)) return;

			_sections.Remove(section);
			UpdateNumbers();
		}

		public void GenerateForumText(Bot bot, DiscordGuild currentGuild)
		{
			var filePath = $"Rules to forum {DateTime.Now.ToString().Replace(":", "-").Replace("/", ".")}.txt";
			using (var writer = new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach (var section in _sections)
				{
					writer.WriteLine($"[SIZE=5][B][COLOR=rgb(147, 101, 184)]{section.FullName}[/COLOR][/B][/SIZE]");

					writer.WriteLine("[SIZE=4]");

					foreach (var rule in section.Rules)
					{
						var coloredRule = $"[COLOR=rgb(243, 121, 52)]{rule.FullNumberWithBracket}[/COLOR]{rule.Name}";

						if (!rule.Punishment.IsNullOrEmpty()) coloredRule += $"[COLOR=rgb(184, 49, 47)] {rule.Punishment}[/COLOR]";

						ReplaceChannelMentions(bot, ref coloredRule);
						ReplaceRolesMentions(currentGuild, ref coloredRule);

						writer.WriteLine(coloredRule);

						if (rule.Notes.Any())
						{
							writer.WriteLine($"[COLOR=rgb(243, 121, 52)]{rule.NotesTitle}[/COLOR]");

							for (int i = 0; i < rule.Notes.Count; i++)
							{
								var note = $"• {rule.Notes[i]}";

								ReplaceChannelMentions(bot, ref note);
								ReplaceRolesMentions(currentGuild, ref note);

								writer.WriteLine(note);
							}
						}
					}

					writer.WriteLine("[/SIZE]");
				}
			}
		}

		private void UpdateNumbers()
		{
			for (int i = 0; i < _sections.Count; i++)
			{
				_sections[i].SetNumber(i + 1);
			}
		}

		private void ReplaceChannelMentions(Bot bot, ref string input)
		{
			foreach (var channelMention in Regex.Matches(input, @"<#[0-9]{1,}>").Select(match => match.Value))
			{
				var channelId = ulong.Parse(channelMention.Substring(2, channelMention.Length - 3));
				input = input.Replace(channelMention, bot.GetChannelAsync(channelId).Result.Name);
			}
		}

		private void ReplaceRolesMentions(DiscordGuild guild, ref string input)
		{
			foreach (var roleMention in Regex.Matches(input, @"<@&[0-9]{1,}>").Select(match => match.Value))
			{
				var roleId = ulong.Parse(roleMention.Substring(3, roleMention.Length - 4));
				input = input.Replace(roleMention, guild.GetRole(roleId).Name);
			}
		}
	}
}
