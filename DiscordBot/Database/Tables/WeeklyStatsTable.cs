namespace DiscordBot.Database.Tables
{
	public class WeeklyStatsTable : ITable
	{
		public ulong Id { get; set; }
		public int Punishments { get; set; }
		public int Tickets { get; set; }

		public string[] GetMap() => null;
	}
}
