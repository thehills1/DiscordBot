using DSharpPlus.Entities;

namespace DiscordBot
{
	public class MessageManager
	{
		private readonly Bot _bot;

		public MessageManager(Bot bot) 
		{
			_bot = bot;
		}

		public async Task<DiscordMessage> SendMessageAsync(ulong channelId, string content = null, List<DiscordEmbed> embeds = null, FileStream file = null)
		{
			var messageBuilder = new DiscordMessageBuilder();

			if (content != null) messageBuilder = messageBuilder.WithContent(content);
			if (file != null) messageBuilder = messageBuilder.AddFile(file);

			foreach (var embed in embeds)
			{
				messageBuilder = messageBuilder.AddEmbed(embed);
			}

			return await messageBuilder.SendAsync(await _bot.GetChannelAsync(channelId));
		}

		public async Task<DiscordMessage> EditMessageAsync(ulong channelId, ulong messageId, string content = null, List<DiscordEmbed> embeds = null)
		{
			return await (await _bot.GetMessageAsync(channelId, messageId)).ModifyAsync(content, embeds);
		}

		public async void DeleteMessageAsync(ulong channelId, ulong messageId)
		{
			await (await _bot.GetMessageAsync(channelId, messageId)).DeleteAsync();
		}
	}
}
