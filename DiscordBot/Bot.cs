using DiscordBot.Commands;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace DiscordBot
{
	public class Bot : IDisposable
    {
		private readonly DiscordClient _client;
		private readonly IServiceProvider _serviceProvider;

		private ServiceManager _serviceManager;

		public Bot(DiscordClient client, 
			IServiceProvider serviceProvider)
        {
			_client = client;
			_serviceProvider = serviceProvider;
		}

		public async void Initialize()
		{
			ConfigurateClient();
			SetupCommandsRegistration();

			await RunAsync();
		}

		private async Task RunAsync()
		{
			await _client.ConnectAsync();
			await Task.Delay(-1);
		}

		private void ConfigurateClient()
		{			
			_client.UseInteractivity(new InteractivityConfiguration()
			{
				PollBehaviour = PollBehaviour.KeepEmojis,
				Timeout = TimeSpan.FromSeconds(60)
			});

			//_client.MessageUpdated += HandleMessageUpdated;
			//_client.MessageDeleted += HandleMessageDeleted;
			//_client.VoiceStateUpdated += HandleVoiceStateUpdated;
		}

		private void SetupCommandsRegistration()
		{
			var cmds = _client.UseSlashCommands(new SlashCommandsConfiguration() { Services = _serviceProvider });

			_client.GuildAvailable += async (s, e) => await RegisterCommandsAndUpdate(cmds, e.Guild.Id);
			_client.GuildCreated += async (s, e) => await RegisterCommandsAndUpdate(cmds, e.Guild.Id);
		}

		private async Task RegisterCommandsAndUpdate(SlashCommandsExtension cmds, ulong guildId)
		{
			await _client.DeleteGlobalApplicationCommandAsync(1064687244281118852);
			cmds.RegisterCommands<GlobalCommands>(guildId);
			await cmds.RefreshCommands();
		}	

		public void Dispose()
		{
			_client.Dispose();
		}
	}
}
