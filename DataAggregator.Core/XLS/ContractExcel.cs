
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataAggregator.Core.Models.GovernmentPurchases.GovernmentPurchases;

using DataAggregator.Domain.Model.GovernmentPurchases;
using ExcelDataReader;
using LinqToExcel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace DataAggregator.Core.XLS
{
    public class ContractExcel : IDisposable
    {
        IWorkbook workbook;
        ISheet sheet;

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
            //Наименование, Ед.изм, Кол-во, Цена, Сумма
            var contractObjects = objects == null ? new List<ContractObjectReadyJson>() : objects.ToList();

            workbook = new XSSFWorkbook();
            sheet = workbook.CreateSheet("Шаблон ТЗ");

            var header = sheet.CreateRow(0);

            header.CreateCell(0).SetCellValue("Наименование объекта закупки");
            header.CreateCell(1).SetCellValue("Единица измерения");
            header.CreateCell(2).SetCellValue("Количество");
            header.CreateCell(3).SetCellValue("Цена за единицу");            
            header.CreateCell(4).SetCellValue("Сумма");
            

            for (var i = 0; i < contractObjects.Count(); i++)
            {
                var o = contractObjects[i];
                var row = sheet.CreateRow(i + 1);

                row.CreateCell(0).SetCellValue(o.Name);
                row.CreateCell(1).SetCellValue(o.Unit);
                row.CreateCell(2).SetCellType(CellType.Numeric);
                row.CreateCell(2).SetCellValue(o.Amount.ToString());

                row.CreateCell(3).SetCellType(CellType.Numeric);
                row.CreateCell(3).SetCellValue(o.Price.ToString());

                row.CreateCell(4).SetCellType(CellType.Numeric);
                row.CreateCell(4).SetCellValue(o.Sum.ToString());
                
            }
            byte[] bytes = null;

            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);

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
                    bool findSheet = false;
                    if (reader != null) 
                    { 
                        for (int sheet = 0; sheet < reader.ResultsCount; sheet++)
                        {
                            if (!findSheet && reader.Name == worksheet)
                                findSheet = true;

                            reader.NextResult();
                        }
                    }
                    if (!findSheet)
                    {
                        throw new ApplicationException(String.Format(" В шаблоне отсутствует лист: {0}", worksheet));
                    }
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
                throw new ApplicationException(e.Message);
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
