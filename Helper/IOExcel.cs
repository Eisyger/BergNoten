using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Globalization;
using System.Reflection;

namespace AusbilderAppViews.Model
{
    class IOExcel
    {
        #region Export
        private static void ExportCreateHeaderRow(ISheet sheet, List<PropertyInfo> properties)
        {
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < properties.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(properties[i].Name);
            }
        }

        private static void ExportInsertData(ISheet sheet, List<IExportable> items, List<PropertyInfo> properties, ICellStyle dateCellStyle)
        {
            int row = 1;
            foreach (var item in items)
            {
                IRow dataRow = sheet.CreateRow(row);
                for (int i = 0; i < properties.Count; i++)
                {
                    object? valueObject = properties[i].GetValue(item);
                    string? value = valueObject != null ? valueObject.ToString() : string.Empty;
                    if (DateTime.TryParseExact(value, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out DateTime dateTime))
                    {
                        var test = dateTime.ToString();

                        // Hier die Formatierung der Zelle auf Datum gesetzt
                        ICell cell = dataRow.CreateCell(i);
                        cell.CellStyle = dateCellStyle;
                        cell.SetCellValue(dateTime);
                    }
                    else if (double.TryParse(value, out double number))
                    {
                        // Wert ist eine Zahl
                        dataRow.CreateCell(i).SetCellValue(number);
                    }
                    else
                    {
                        // Wert ist Text
                        dataRow.CreateCell(i).SetCellValue(value);
                    }
                }
                row++;
            }
        }

        public static void ExportToExcel(List<List<IExportable>> save_sheets, List<string> sheetNames, string filePath)
        {
            var workbook = new HSSFWorkbook();

            for (int i = 0; i < save_sheets.Count; i++)
            {
                var sheet = workbook.CreateSheet(sheetNames[i]);
                var items = save_sheets[i];
                if (items.Count == 0)
                    continue;

                // Definieren des Datumsformats für die Datumszeile
                ICellStyle dateCellStyle = workbook.CreateCellStyle();
                short dateFormat = workbook.CreateDataFormat().GetFormat("dd.MM.yyyy");
                dateCellStyle.DataFormat = dateFormat;

                var properties = items[0].GetProperties();

                ExportCreateHeaderRow(sheet, properties);
                ExportInsertData(sheet, items, properties, dateCellStyle);
            }

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            workbook.Write(fs);
        }
        #endregion

        #region Import
        private static object? ImportParseCellValue(ICell cell, Type targetType)
        {
            if (cell != null)
            {
                string? value = cell.ToString();
                if (targetType == typeof(string) && value != null)
                {
                    return value;
                }
                else if (targetType == typeof(int))
                {
                    if (int.TryParse(value, out int intValue))
                    {
                        return intValue;
                    }
                }
            }
            return null;
        }

        private static int? ImportGetColumnIndex(IRow headerRow, string columnName)
        {
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                if (headerRow.GetCell(i).ToString() == columnName)
                {
                    return i;
                }
            }
            return null;
        }

        private static T ImportItem<T>(IRow row, IRow headerRow, List<PropertyInfo> properties) where T : IExportable, new()
        {
            var item = new T();
            for (int j = 0; j < properties.Count; j++)
            {
                string columnName = properties[j].Name;
                int? columnIndex = ImportGetColumnIndex(headerRow, columnName);

                if (columnIndex.HasValue)
                {
                    ICell cell = row.GetCell(columnIndex.Value);
                    object? value = ImportParseCellValue(cell, properties[j].PropertyType);
                    properties[j].SetValue(item, value);
                }
            }
            return item;
        }

        public static List<List<IExportable>> ImportFromExcel(string filePath)
        {
            var result = new List<List<IExportable>>();
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var workbook = new HSSFWorkbook(fs);

                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);
                    IRow headerRow = sheet.GetRow(0);
                    if (headerRow == null) continue;

                    var sheetItems = new List<IExportable>();
                    var firstRowType = DetermineRowType(headerRow);

                    for (int j = 1; j <= sheet.LastRowNum; j++)
                    {
                        IRow row = sheet.GetRow(j);
                        if (row != null)
                        {
                            if (firstRowType == typeof(Participant))
                            {
                                var participant = ImportItem<Participant>(row, headerRow, new Participant().GetProperties());
                                sheetItems.Add(participant);
                            }
                            else if (firstRowType == typeof(Exam))
                            {
                                var exam = ImportItem<Exam>(row, headerRow, new Exam().GetProperties());
                                sheetItems.Add(exam);
                            }

                            else if (firstRowType == typeof(Type))
                            {
                                // no match with any IExportable
                                break;
                            }

                            /*Ein Laden der Noten aus einer final erzeugten excel ist nicht vorgesehen
                            else if (firstRowType == typeof(Grade))
                            {
                                var gradeModel = ImportItem<Grade>(row, headerRow, new Grade().GetProperties());
                                sheetItems.Add(gradeModel);
                            }*/
                        }
                    }
                    if (sheetItems.Count > 0)
                    {
                        result.Add(sheetItems);
                    }
                }

                // Application.Current?.MainPage?.DisplayAlert("Erfolg",
                // "Importieren der Daten aus .xls erfolgreich: ", "OK");
            }
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlert("Fehler",
                    "Fehler beim Importieren der Daten aus .xls: " + ex.Message, "OK");
            }
            return result;
        }

        private static Type DetermineRowType(IRow headerRow)
        {
            // Annahme: Wir bestimmen den Typ anhand der Spaltennamen der Kopfzeile
            var participantProperties = new Participant().GetProperties().Select(p => p.Name).ToList();
            var examProperties = new Exam().GetProperties().Select(p => p.Name).ToList();

            var headerColumns = new List<string>();
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                string cell = headerRow.GetCell(i).ToString() ?? "Invalid Column Name";
                headerColumns.Add(cell);
            }

            if (participantProperties.All(p => headerColumns.Contains(p)) && participantProperties.Count == headerColumns.Count)
            {
                return typeof(Participant);
            }
            else if (examProperties.All(p => headerColumns.Contains(p)) && examProperties.Count == headerColumns.Count)
            {
                return typeof(Exam);
            }
            //return a default type if the rows don't match any IExportable
            return typeof(Type);

            /*Ein Laden der Noten aus einer final erzeugten excel ist nicht vorgesehen
            else if (gradeProperties.All(p => headerColumns.Contains(p)))
            {
                return typeof(Grade);
            }*/

            throw new Exception($"Unbekannter Typ in der Excel-Datei. {string.Join(", ", headerColumns)}, {headerColumns.Count}, {participantProperties.Count}");
        }
        #endregion
    }
}
