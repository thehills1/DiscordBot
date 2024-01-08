using DiscordBot.Commands;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using OfficeOpenXml;

namespace DiscordBot
{
	public class Bot : IDisposable
    {
		private readonly DiscordClient _client;
		private readonly IServiceProvider _serviceProvider;

		public Bot(DiscordClient client, 
			IServiceProvider serviceProvider)
        {
			_client = client;
			_serviceProvider = serviceProvider;
		}

		public async void Initialize()
		{
			ExcelPackage.LicenseContext = LicenseContext.Commercial;
			
			SetupCommandsRegistration();
			await RunAsync();
		}

		public async Task<DiscordMessage> GetMessageAsync(ulong channelId, ulong messageId) => await (await GetChannelAsync(channelId)).GetMessageAsync(messageId);

		public async Task<DiscordChannel> GetChannelAsync(ulong id) => await _client.GetChannelAsync(id);

        public async Task<DiscordUser> GetUserAsync(ulong id) => await _client.GetUserAsync(id);

        private void SetupCommandsRegistration()
		{
			var cmds = _client.UseSlashCommands(new SlashCommandsConfiguration() { Services = _serviceProvider });

			_client.GuildAvailable += async (s, e) => await RegisterCommandsAndUpdate(cmds, e.Guild.Id);
			_client.GuildCreated += async (s, e) => await RegisterCommandsAndUpdate(cmds, e.Guild.Id);
		}

		private async Task RegisterCommandsAndUpdate(SlashCommandsExtension cmds, ulong guildId)
		{
			cmds.RegisterCommands<GlobalCommands>(guildId);
			cmds.RegisterCommands<ShopCommands>(guildId);
			await cmds.RefreshCommands();
		}

		private async Task RunAsync()
		{
			await _client.ConnectAsync();
			await Task.Delay(-1);
		}	

		public void Dispose()
		{
			_client.Dispose();
		}
	}
}
