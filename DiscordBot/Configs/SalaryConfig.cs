using DiscordBot.Database.Enums;

namespace DiscordBot.Configs
{
    public class SalaryConfig : BaseConfig<SalaryConfig>
    {
		/// <summary>
		/// Серверный бюджет на неделю для модераторов.
		/// </summary>
		public Dictionary<ServerName, int> Sum { get; set; }

		/// <summary>
		/// Недельная зарплата руководства.
		/// </summary>
		public Dictionary<ServerName, ChiefsSalary> ChiefsSalary { get; set; }

		/// <summary>
		/// Количество действий за неделю для получения зарплаты.
		/// </summary>
        public int ActionsPerWeekToSalary { get; set; }  
    }

    public class ChiefsSalary
    {
        public int ChiefModerator { get; set; }
        public int DeputyChiefModerator { get; set; }
        public int Curator { get; set; }
    }
}
