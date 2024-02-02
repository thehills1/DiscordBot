using System.Collections.ObjectModel;
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

		private void UpdateNumbers()
		{
			for (int i = 0; i < _sections.Count; i++)
			{
				_sections[i].SetNumber(i + 1);

				foreach (var rule in _sections[i].Rules)
				{
					rule.SetSectionNumber(i + 1);
				}
			}
		}
	}
}
