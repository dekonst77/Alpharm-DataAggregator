using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DataAggregator.Core.Models.GovernmentPurchases.GovernmentPurchases;
using DataAggregator.Domain.Model.GovernmentPurchases;

using ExcelDataReader;
using ExcelLibrary.SpreadSheet;

namespace DataAggregator.Web.GovernmentPurchasesExcel
{
    public class ContractExcel : IDisposable
    {
        Workbook workbook;
        Worksheet sheet;

        private byte[] GetByte(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public byte[] GetExcel(IEnumerable<ContractObjectReadyJson> objects)
        {
            var contractObjects = objects == null ? new List<ContractObjectReadyJson>() : objects.ToList();

            workbook = new Workbook();
            sheet = new Worksheet("Шаблон ТЗ");

            sheet.Cells[0, 0] = new Cell("Наименование объекта закупки");
            sheet.Cells[0, 1] = new Cell("Единица измерения");
            sheet.Cells[0, 2] = new Cell("Количество");
            sheet.Cells[0, 3] = new Cell("Цена за единицу");
            sheet.Cells[0, 4] = new Cell("Сумма");

            for (var i = 0; i < contractObjects.Count(); i++)
            {
                var o = contractObjects[i];

                sheet.Cells[i + 1, 0] = new Cell(o.Name);
                sheet.Cells[i + 1, 1] = new Cell(o.Unit);
                sheet.Cells[i + 1, 2] = new Cell((decimal)o.Amount);
                sheet.Cells[i + 1, 3] = new Cell((decimal)o.Price);
                sheet.Cells[i + 1, 4] = new Cell((decimal)o.Sum);
            }

            workbook.Worksheets.Add(sheet);

            byte[] bytes = null;

            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveToStream(stream);

                bytes = stream.ToArray();
            }

            return bytes;
            
        }

        public IEnumerable<ContractObjectReady> GetContractObjectReady(Stream data)
        {
            const string worksheet = "Шаблон ТЗ";

            // поля Excel:
            const string Unit_FieldName = "Единица измерения";
            const string Amount_FieldName = "Количество";
            const string Price_FieldName = "Цена за единицу";
            const string Sum_FieldName = "Сумма";

            List<string> reservedNamed = new List<string>()
            {
                Unit_FieldName,
                Amount_FieldName,
                Price_FieldName,
                Sum_FieldName
            };

            try
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(data))
                {
                    #region проверка на существование листа <Шаблон ТЗ>
                    while (reader != null)
                    {
                        if (reader.Name == worksheet)
                            break;
                        else
                            reader.NextResult();
                    }

                    if (reader == null)
                        throw new ApplicationException(String.Format(" В шаблоне отсутствует лист: {0}", reader.Name));
                    #endregion

                    #region Список всех колонок
                    reader.Read();

                    Dictionary<int, string> columns = new Dictionary<int, string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add(i, reader.GetValue(i).ToString());
                    }
                    #endregion

                    #region Колонки, которых нет среди обязательных именованных reservedNamed
                    var missedColumn = reservedNamed.Except(columns.Values).ToList();

                    if (missedColumn.Any())
                    {
                        StringBuilder missedColumList = new StringBuilder();

                        missedColumn.ForEach(c => missedColumList.Append(c));

                        throw new ApplicationException(String.Format(" В шаблоне отсутствуют следующие обязательные поля: {0}", missedColumList));
                    }
                    #endregion

                    #region Колонки Excel среди обязательных в формате <Имя столбца, порядковый номер>
                    Dictionary<string, int> ExistColumns = columns.Where(t => reservedNamed.Contains(t.Value)).ToDictionary(k => k.Value, k => k.Key);
                    #endregion

                    #region Колонки для поля <Наименование объекта закупки>: все остальные в Excel кроме обязательных именованных reservedNamed
                    var nameColumns = columns.Where(c => !reservedNamed.Contains(c.Value)).ToList();

                    if (!nameColumns.Any())
                        throw new ApplicationException(String.Format(" В шаблоне отсутствуют поля, определяющие наименование объекта закупки"));
                    #endregion

                    #region считываем все данные из excel в список rows
                    List<ArrayList> rows = new List<ArrayList>();
                    while (reader.Read())
                    {
                        var row = new ArrayList();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetValue(i));
                        }

                        rows.Add(row);
                    }
                    #endregion

                    #region загружаем данные в dataClass по обязательным полям из списка reservedNamed
                    List<ExcelObject> dataClass = rows.Select(d => new ExcelObject()
                    {
                        Unit = d[ExistColumns[Unit_FieldName]].ToString(),
                        Amount = d[ExistColumns[Amount_FieldName]].ToString(),
                        Price = d[ExistColumns[Price_FieldName]].ToString(),
                        Sum = d[ExistColumns[Sum_FieldName]].ToString()
                    }).ToList();
                    #endregion

                    #region Заполняем <наименование объекта закупки>
                    for (int i = 0; i < rows.Count; i++)
                    {
                        var row = rows[i];
                        StringBuilder objectName = new StringBuilder();

                        foreach (var column in nameColumns)
                        {
                            objectName.Append(" ");
                            objectName.Append(row[column.Key]);
                        }

                        dataClass[i].Name = objectName.ToString().Trim();
                    }
                    #endregion

                    dataClass.ForEach(clearString);

                    return dataClass.Where(d => !(string.IsNullOrEmpty(d.Amount) &&
                                                string.IsNullOrEmpty(d.Price) &&
                                                string.IsNullOrEmpty(d.Sum) &&
                                                string.IsNullOrEmpty(d.Unit) &&
                                                string.IsNullOrEmpty(d.Name))).Select(d => d.GetContractObjectReady());
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Ошибка анализа файла", e);
            }
        }

        private void clearString(ExcelObject obj)
        {
            if (!string.IsNullOrEmpty(obj.Name))
                obj.Name = obj.Name.Trim();

            if (!string.IsNullOrEmpty(obj.Price))
                obj.Price = obj.Price.Trim();

            if (!string.IsNullOrEmpty(obj.Sum))
                obj.Sum = obj.Sum.Trim();

            if (!string.IsNullOrEmpty(obj.Unit))
                obj.Unit = obj.Unit.Trim();

            if (!string.IsNullOrEmpty(obj.Name))
                obj.Name = obj.Name.Trim();
        }

        public void Dispose()
        {
            if (sheet != null) sheet = null;
            if (workbook != null) workbook = null;
        }
    }

}
