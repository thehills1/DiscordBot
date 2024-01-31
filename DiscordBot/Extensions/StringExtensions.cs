namespace DiscordBot.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string value)
		{
			return value is null || value == string.Empty;
		}
	}
}
