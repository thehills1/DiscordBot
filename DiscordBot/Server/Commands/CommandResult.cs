namespace DiscordBot.Server.Commands
{
	public class CommandResult
	{
		public bool Success { get; }

		public string Message { get; }

		public CommandResult(bool success) : this(success, "")
		{
		}

		public CommandResult(bool success, string message)
		{
			Success = success;
			Message = message;
		}
	}
}
