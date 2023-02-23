using DiscordBot.Extensions.Collections;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DiscordBot.Extensions.Excel
{
	public static class ExcelWorksheetCreator
	{
		public static void GenerateAndSaveFile(List<ITableCollection> insertTables, string fileName)
		{
			try
			{
				var package = new ExcelPackage();

				foreach (var insertTable in insertTables)
				{
					if (insertTable.Count != 0)
					{
						var tableType = insertTable.GetType();
						var listName = (ExcelListAttribute) Attribute.GetCustomAttribute(tableType, typeof(ExcelListAttribute));

						var sheet = package.Workbook.Worksheets.Add(listName.Name);
						var props = insertTable.First().GetMap();

						FillColumnNames(sheet, props, tableType);
						FillColumnValues(sheet, insertTable, props, tableType);

						sheet.Cells.AutoFitColumns();
						sheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					}
				}

				package.SaveAs(new FileInfo(fileName));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private static void FillColumnNames(ExcelWorksheet sheet, string[] props, Type type)
		{
			var propsInfo = props.Select(type.GetProperty).ToArray();

			for (int i = 0; i < props.Length; i++)
			{
				string name;
				var columnName = (ExcelColumnAttribute) Attribute.GetCustomAttribute(propsInfo[i], typeof(ExcelColumnAttribute));

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

		private static void FillColumnValues(ExcelWorksheet sheet, ITableCollection tables, string[] props, Type type)
		{
			for (int row = 0; row < tables.Count(); row++)
			{
				var table = tables[row];

				for (int column = 0; column < props.Count(); column++)
				{
					var name = props[column];
					var value = type.GetProperty(name).GetValue(table);
					sheet.Cells[row + 2, column + 1].Value = value?.ToString();
				}
			}
		}
	}
}
