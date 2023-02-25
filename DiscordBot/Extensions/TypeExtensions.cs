using System.Reflection;
using DiscordBot.Database.Extensions;

namespace DiscordBot.Extensions
{
	public static class TypeExtensions
	{
		public static List<PropertyInfo> GetAllowedToChangeProperties(this Type type)
		{
			var output = new List<PropertyInfo>();

			var properties = type.GetProperties();
			foreach (var property in properties)
			{
				var changeAttribute = (AllowCommandChangeAttribute) Attribute.GetCustomAttribute(property, typeof(AllowCommandChangeAttribute));
				if (changeAttribute == null) continue;

				output.Add(property);
			}

			return output;
		}
	}
}
