namespace DiscordBot.Database.Tables
{
	public class WeeklyStatsTable : BaseTable
	{
		public int Punishments { get; set; }
		public int Tickets { get; set; }

		public override string[] GetMap() => null;
	}
}
