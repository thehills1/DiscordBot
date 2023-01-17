using DiscordBot.Commands;
using DiscordBot.Configs;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
	public class Bot : IDisposable
    {
		private readonly IServiceProvider _serviceProvider;
		private readonly BotConfig _botConfig;
		private readonly SalaryConfig _salaryConfig;

		public static DiscordClient Client { get; private set; }

		public Bot(IServiceProvider serviceProvider, BotConfig botConfig, SalaryConfig salaryConfig)
        {
			_serviceProvider = serviceProvider;
			_botConfig = botConfig;
			_salaryConfig = salaryConfig;
		}

		public async Task RunAsync()
		{
			ConfigurateClient();

			await Client.ConnectAsync();
			await Task.Delay(-1);
		}

		private void ConfigurateClient()
		{
			Client = new DiscordClient(new DiscordConfiguration
			{
				Token = _botConfig.Token,
				TokenType = TokenType.Bot,
				AutoReconnect = true,
				MinimumLogLevel = LogLevel.Debug,
				Intents = DiscordIntents.All
			});
			
			Client.UseInteractivity(new InteractivityConfiguration()
			{
				PollBehaviour = PollBehaviour.KeepEmojis,
				Timeout = TimeSpan.FromSeconds(60)
			});

			SetupCommandsRegistration();

			//Client.MessageUpdated += HandleMessageUpdated;
			//Client.MessageDeleted += HandleMessageDeleted;
			//Client.VoiceStateUpdated += HandleVoiceStateUpdated;
		}

		public void SetupCommandsRegistration()
		{
			var cmds = Client.UseSlashCommands(new SlashCommandsConfiguration() { Services = _serviceProvider });

			Client.GuildAvailable += async (s, e) =>
			{
				cmds.RegisterCommands<GlobalCommands>(e.Guild.Id);
				await cmds.RefreshCommands();
				Console.WriteLine(e.Guild.Id);
			};

			Client.GuildCreated += async (s, e) =>
			{
				cmds.RegisterCommands<GlobalCommands>(e.Guild.Id);
				await cmds.RefreshCommands();
				Console.WriteLine(e.Guild.Id);
			};
		}

		public async void Initialize()
		{


			await RunAsync();
		}

		public void Dispose()
		{
			Client.Dispose();
		}
	}
}
