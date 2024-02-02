namespace DiscordBot.Extensions
{
	public static class IEnumerableExtensions
	{
		public static string Join(this IEnumerable<char> items, char c)
		{
			return string.Join(c, items);
		}

		public static string Join(this IEnumerable<char> items, string s)
		{
			return string.Join(s, items);
		}
	}
}
