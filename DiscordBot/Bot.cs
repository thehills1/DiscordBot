using DiscordBot.Configs;

namespace DiscordBot
{
    public class Bot
    {
        private readonly BotConfig _botConfig;
		private readonly SalaryConfig _salaryConfig;

		public Bot(BotConfig botConfig, SalaryConfig salaryConfig)
        {
            _botConfig = botConfig;
			_salaryConfig = salaryConfig;
		}

		public void Initialize()
		{
			
		}
    }
}
