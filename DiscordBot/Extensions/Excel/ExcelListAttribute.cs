namespace DiscordBot.Extensions.Excel
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ExcelListAttribute : Attribute
	{
		public string Name { get; }

		public ExcelListAttribute(string name) 
		{
			Name = name;
		}	
	}
}
