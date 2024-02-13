using System.Collections.ObjectModel;
using System.Text;
using DiscordBot.Extensions;
using Newtonsoft.Json;

namespace DiscordBot.Configs.Rules
{
	public class Rule
	{
		[JsonProperty]
		public int SectionNumber { get; private set; }

		[JsonProperty]
		public int Number { get; private set; }

		[JsonProperty]
		public int SubNumber { get; private set; }

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public string Punishment { get; private set; }

		[JsonIgnore]
		public ReadOnlyCollection<string> Notes => new ReadOnlyCollection<string>(_notes);

		[JsonProperty(nameof(Notes))]
		private List<string> _notes = new();

		public Rule(int sectionNumber, int number, string name, int subNumber = 0, string punishment = null, List<string> notes = null)
		{
			SetSectionNumber(sectionNumber);
			SetNumber(number);
			SetSubNumber(subNumber);
			SetName(name);
			SetPunishment(punishment);

			if (notes == null || !notes.Any()) return;

 			foreach (var note in notes)
			{
				AddNote(note);
			}
		}

		public void SetSectionNumber(int sectionNumber)
		{
			if (sectionNumber <= 0) throw new ArgumentException($"Номер раздела правил не может быть меньше нуля или равен нулю, [{nameof(sectionNumber)}]=[{sectionNumber}].");

			SectionNumber = sectionNumber;
		}

		public void SetNumber(int number)
		{
			if (number <= 0) throw new ArgumentException($"Номер правила не может быть меньше нуля или равен нулю, [{nameof(number)}]=[{number}].");

			Number = number;
		}

		public void SetSubNumber(int subNumber)
		{
			if (subNumber < 0) throw new ArgumentException($"Номер подпункта правила не может быть меньше нуля, [{nameof(subNumber)}]=[{subNumber}].");

			SubNumber = subNumber;
		}

		public void SetName(string name)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentException($"Имя правила не может быть пустым или null, [{nameof(name)}]=[{name}].");

			Name = name;
		}

		public void SetPunishment(string punishment)
		{
			if (punishment == null) punishment = string.Empty;

			Punishment = punishment;
		}

		public void AddNote(string note)
		{
			if (note.IsNullOrEmpty()) throw new ArgumentException($"Примечание не может быть пустым или null, [{nameof(note)}]=[{note}].");

			_notes.Add(note);
		}

		public void RemoveNote(string note)
		{
			_notes.Remove(note);
		}

		public void ClearNotes()
		{
			_notes.Clear();
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			var ruleNumber = SubNumber == 0 ? $"{SectionNumber}.{Number}" : $"{SectionNumber}.{Number}.{SubNumber}";
			sb.AppendLine($"**{ruleNumber}) {Name}**");

			if (!Punishment.IsNullOrEmpty()) sb.AppendLine(Punishment);

			if (_notes?.Count > 0)
			{
				sb.AppendLine();
				sb.AppendLine($"Примечания к правилу {ruleNumber}:");

				for (int i = 1; i <= _notes.Count; i++)
				{
					sb.AppendLine($"{i}. {_notes[i - 1]}");
				}
			}

			sb.AppendLine();

			return sb.ToString();
		}
	}
}
