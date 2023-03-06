namespace DiscordBot.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static T GetUnregisteredService<T>(this IServiceProvider container) where T : class
		{
			var constructorInfo = typeof(T).GetConstructors().OrderByDescending(info => info.GetParameters().Length).First();
			var parameters = constructorInfo.GetParameters();

			if (parameters == null || parameters.Length == 0)
			{
				return Activator.CreateInstance<T>();
			}

			var resolvedParams = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				resolvedParams[i] = container.GetService(parameters[i].ParameterType);
			}

			return (T) Activator.CreateInstance(typeof(T), resolvedParams);
		}
	}
}
