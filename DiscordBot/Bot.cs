using DiscordBot.Commands;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
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

			SetupInteractivity();
			SetupCommandsRegistration();
			await RunAsync();
		}

		
		#region Messages
		public async Task<DiscordMessage> GetMessageAsync(ulong channelId, ulong messageId) => await (await GetChannelAsync(channelId)).GetMessageAsync(messageId);

		public async Task<DiscordMessage> SendMessageAsync(ulong channelId, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			return await SendMessageAsync(await GetChannelAsync(channelId), content, embeds, file);
		}

		public async Task<DiscordMessage> SendMessageAsync(DiscordChannel channel, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			var messageBuilder = new DiscordMessageBuilder();

			if (content != null) messageBuilder = messageBuilder.WithContent(content);
			if (file != null) messageBuilder = messageBuilder.AddFile(file);

			if (embeds != null)
			{
				foreach (var embed in embeds)
				{
					messageBuilder = messageBuilder.AddEmbed(embed);
				}
			}

			messageBuilder = messageBuilder.WithAllowedMentions(Mentions.All);

			return await messageBuilder.SendAsync(channel);
		}

		public async Task<List<DiscordMessage>> SendMessagesAsync(DiscordChannel channel, List<string> contents)
		{
			var output = new List<DiscordMessage>();

			foreach (var content in contents)
			{
				output.Add(await SendMessageAsync(channel, content));
			}

			return output;
		}

		public async Task<List<DiscordMessage>> EditMessagesAsync(ulong channelId, List<ulong> messageIds, List<string> contents)
		{
			if (messageIds.Count != contents.Count) return null;

			var channel = await GetChannelAsync(channelId);
			var messages = new List<DiscordMessage>();
			for (int i = 0; i < messageIds.Count; i++)
			{
				messages.Add(await channel.GetMessageAsync(messageIds[i]));
			}

			return await EditMessagesAsync(messages, contents);
		}

		public async Task<List<DiscordMessage>> EditMessagesAsync(List<DiscordMessage> messages, List<string> contents)
		{
			if (messages.Count != contents.Count) return null;

			var output = new List<DiscordMessage>();
			for (int i = 0; i < messages.Count; i++)
			{
				output.Add(await messages[i].ModifyAsync(contents[i]));
			}

			return output;
		}

		public async Task<DiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, string content = null, List<DiscordEmbed> embeds = null)
		{
			return await EditMessageAsync(await GetChannelAsync(channelId), messageId, content, embeds);
		}

		public async Task<DiscordMessage> EditMessageAsync(DiscordChannel channel, ulong messageId, string content = null, List<DiscordEmbed> embeds = null)
		{
			return await EditMessageAsync(await channel.GetMessageAsync(messageId), content, embeds);
		}

		public async Task<DiscordMessage> EditMessageAsync(DiscordMessage message, string content = null, List<DiscordEmbed> embeds = null)
		{
			var builder = new DiscordMessageBuilder().WithContent(content).AddEmbeds(embeds).WithAllowedMentions(Mentions.All);
			return await message.ModifyAsync(builder);
		}

		public async Task DeleteMessageAsync(DiscordMessage message)
		{
			await message.DeleteAsync();
		}

		public async Task DeleteMessageAsync(DiscordChannel channel, ulong messageId)
		{
			await (await channel.GetMessageAsync(messageId)).DeleteAsync();
		}

		public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
		{
			await (await GetMessageAsync(channelId, messageId)).DeleteAsync();
		}

		public async Task<bool> MessageExistsAsync(ulong channelId, ulong messageId)
		{
			var channel = await GetChannelAsync(channelId);
			return await MessageExistsAsync(channel, messageId);
		}

		public async Task<bool> MessageExistsAsync(DiscordChannel channel, ulong messageId)
		{
			try
			{
				var message = await channel.GetMessageAsync(messageId);
				return message != null;
			}
			catch
			{
				return false;
			}
		}
		#endregion

		#region Channels
		public async Task<DiscordChannel> GetChannelAsync(ulong id) => await _client.GetChannelAsync(id);

		public async Task DeleteChannelAsync(ulong channelId) => await (await GetChannelAsync(channelId)).DeleteAsync();

		public async Task<IReadOnlyList<DiscordChannel>> GetAllChannelsAsync(ulong guildId) => await (await GetGuildAsync(guildId)).GetChannelsAsync();
		#endregion

		#region Guilds
		public async Task<DiscordGuild> GetGuildAsync(ulong guildId)
		{
			return await _client.GetGuildAsync(guildId);
		}
		#endregion

		#region Users
		public async Task<DiscordUser> GetUserAsync(ulong id) => await _client.GetUserAsync(id);
		public async Task<DiscordMember> GetUserInGuildAsync(ulong guildId, ulong userId) => await (await GetGuildAsync(guildId)).GetMemberAsync(userId);
		#endregion

		#region Reactions
		public async Task SetReactionAsync(DiscordMessage message, string reactionName)
		{
			await SetReactionAsync(message, GetReactionByName(reactionName));
		}

		public async Task SetReactionAsync(DiscordMessage message, DiscordEmoji reaction)
		{
			await message.CreateReactionAsync(reaction);
		}

		public DiscordEmoji GetReactionByName(string name)
		{
			return DiscordEmoji.FromName(_client, name);
		}
		#endregion

		private void SetupCommandsRegistration()
		{
			var cmds = _client.UseSlashCommands(new SlashCommandsConfiguration() { Services = _serviceProvider });

			_client.GuildAvailable += async (s, e) => await RegisterCommandsAndUpdate(cmds, e.Guild.Id);
			_client.GuildCreated += async (s, e) => await RegisterCommandsAndUpdate(cmds, e.Guild.Id);
		}

		private void SetupInteractivity()
		{
			_client.UseInteractivity(new InteractivityConfiguration()
			{
				Timeout = TimeSpan.FromMinutes(5)
			});

			_client.ModalSubmitted += async (s, e) => await e.Interaction.DeferAsync(true);
		}

		private async Task RegisterCommandsAndUpdate(SlashCommandsExtension cmds, ulong guildId)
		{
			cmds.RegisterCommands<GlobalCommands>(guildId);
			cmds.RegisterCommands<ShopCommands>(guildId);
			cmds.RegisterCommands<RulesCommands>(guildId);
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
