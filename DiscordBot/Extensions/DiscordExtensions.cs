using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBot.Extensions
{
	public static class DiscordExtensions
	{
		public static string GetUser(this DiscordUser user)
		{
			return user.Discriminator == "0" ? $"{user.Username}" : $"{user.Username}#{user.Discriminator}";
		}

		public static string GetMention(this ulong id, MentionType mentionType)
		{
			if (id.ToString().Length != 18) return null;

			return mentionType switch
			{
				MentionType.Role => $"<@&{id}>",
				MentionType.Username or MentionType.Nickname => $"<@{id}>",
				MentionType.Channel => $"<#{id}>",
				_ => null
			};
		}
	}
}
