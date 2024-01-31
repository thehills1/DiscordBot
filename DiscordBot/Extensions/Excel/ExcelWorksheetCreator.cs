using DiscordBot.Database.Enums;
using DiscordBot.Extensions.Collections;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DiscordBot.Extensions.Excel
{
	public static class ExcelWorksheetCreator
	{
		public static bool TryGenerateAndSaveFile(List<ITableCollection> insertTables, string fileName)
		{
			try
			{
				var package = new ExcelPackage();

				foreach (var insertTable in insertTables)
				{
					if (insertTable.Count != 0)
					{
						var tableType = insertTable.GetTableType();
						var listName = Attribute.GetCustomAttribute(tableType, typeof(ExcelListAttribute)) as ExcelListAttribute;

						var sheet = package.Workbook.Worksheets.Add(listName.Name);
						var props = insertTable.First().GetMap();

						FillColumnNames(sheet, props, tableType);
						FillColumnValues(sheet, insertTable, props, tableType);

						sheet.Cells.AutoFitColumns();
						sheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					}
				}

				package.SaveAs(new FileInfo(fileName));

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		private static void FillColumnNames(ExcelWorksheet sheet, string[] props, Type tableType)
		{
			var propsInfo = props.Select(tableType.GetProperty).ToArray();

			for (int i = 0; i < props.Length; i++)
			{
				string name;
				var columnName = Attribute.GetCustomAttribute(propsInfo[i], typeof(ExcelColumnAttribute)) as ExcelColumnAttribute;

				if (columnName == null)
				{
					name = propsInfo[i].Name;
				}
				else
				{
					name = columnName.Name;
				}

				sheet.Cells[1, i + 1].Value = name;
			}
		}

		private static void FillColumnValues(ExcelWorksheet sheet, ITableCollection tables, string[] props, Type tableType)
		{
			for (int row = 0; row < tables.Count(); row++)
			{
				var table = tables[row];

				for (int column = 0; column < props.Count(); column++)
				{
					var propertyName = props[column];
					var value = tableType.GetProperty(propertyName).GetValue(table)?.ToString();

					switch(tableType.GetProperty(propertyName).PropertyType.Name)
					{
						case nameof(DateTime):
							value = DateTime.Parse(value).ToShortDateString();
							break;
						case nameof(PermissionLevel):
							value = ((int) Enum.Parse<PermissionLevel>(value)).ToString();
							break;
					}

					sheet.Cells[row + 2, column + 1].Value = value;
				}
			}
		}
	}
}
