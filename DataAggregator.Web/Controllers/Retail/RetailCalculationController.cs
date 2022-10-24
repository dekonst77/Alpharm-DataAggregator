using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.RetailCalculation;
using DataAggregator.Web.Models.RetailCalculation;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public class RetailCalculationController : BaseController
    {
        public static DateTime? CalculationPeriod { get; set; }

        private readonly RetailCalculationContext _context;
        private static readonly object LockObject = new object();

        static RetailCalculationController()
        {

            InitiCalculationPeriod();

        }

        public RetailCalculationController()
        {
            _context = new RetailCalculationContext();
        }

        ~RetailCalculationController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult Init()
        {

            int? year = null;
            int? month = null;


            if (CalculationPeriod != null)
            {
                year = CalculationPeriod.Value.Year;
                month = CalculationPeriod.Value.Month;
            }

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new
                {
                    Year = year,
                    Month = month,
                }
            };

            return jsonNetResult;
        }


        [HttpPost]
        public ActionResult GetLauncher(int year, int month)
        {
            var data = _context.ProcessLauncher.Where(p => p.Year == year && p.Month == month).OrderBy(p => p.ProcessId).ToList();

            List<LauncherModel> launcherModel = data.Select(LauncherModel.Create).ToList();

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = launcherModel
            };

            return jsonNetResult;
        }


        //Начать
        [HttpPost]
        public ActionResult StartPeriod(int year, int month)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {

                try
                {
                    //Запускаем транзакцию на изменения

                    lock (LockObject)
                    {
                        if (CalculationPeriod.HasValue)
                        {
                            throw new ApplicationException("Период уже запущен");
                        }

                        //Запишем текущий выпускаемый период
                        CalculationPeriod = new DateTime(year, month, 15);
                        _context.ProcessInfo.First(p => p.Param == "CalculationPeriod").Value =
                            CalculationPeriod.ToString();
                        _context.SaveChanges();
                    }

                    //Если в выпускаемом месяце нету ни одного процесса то нужно их добавить
                    if (!_context.ProcessLauncher.Any(pl => pl.Year == year && pl.Month == month))
                    {
                        foreach (var processProcess in _context.ProcessProcess.OrderBy(p => p.Id))
                        {
                            _context.ProcessLauncher.Add(new Launcher()
                            {
                                Year = year,
                                Month = month,
                                ProcessId = processProcess.Id,
                                StatusId = 0,
                                UserId = new Guid(User.Identity.GetUserId()),
                                StartTime = DateTime.Now
                            });
                        }

                        _context.SaveChanges();
                    }

                    //foreach (var processLauncher in _context.ProcessLauncher.Where(pl => pl.Year == year && pl.Month == month && pl.ProcessId >= 8))
                    //{
                    //    processLauncher.StatusId = 0;
                    //    processLauncher.Comment = null;                       
                    //}

                    //_context.SaveChanges();

                    transaction.Commit();

                }
                catch (Exception e)
                {

                    transaction.Rollback();
                    throw new ApplicationException("Ошибка", e);
                }
                finally
                {
                    InitiCalculationPeriod();
                }
            }



            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = true
            };

            return jsonNetResult;

        }

        private static void InitiCalculationPeriod()
        {
            string calculationPeriodParam;

            using (RetailCalculationContext rc = new RetailCalculationContext())
            {
                calculationPeriodParam = rc.ProcessInfo.FirstOrDefault(p => p.Param == "CalculationPeriod")?.Value;
            }

            if (!string.IsNullOrEmpty(calculationPeriodParam))
            {
                CalculationPeriod = Convert.ToDateTime(calculationPeriodParam);
            }
            else
            {
                CalculationPeriod = null;
            }
        }

        //Закончить
        [HttpPost]
        public ActionResult EndPeriod(int year, int month)
        {
            lock (LockObject)
            {
                CheckCurrentPeriod(year, month);

                CalculationPeriod = null;

                _context.ProcessInfo.First(p => p.Param == "CalculationPeriod").Value = null;
                _context.SaveChanges();
            }

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = true
            };

            return jsonNetResult;

        }
        [HttpPost]
        public ActionResult StopProcess(List<int> processList, int year, int month)
        {
            CheckCurrentPeriod(year, month);
            using (var transaction = _context.Database.BeginTransaction())
            {
                foreach (var i in processList)
                {
                    foreach (var processLauncher in _context.ProcessLauncher.Where(p =>
                        p.ProcessId == i && p.Year == year && p.Month == month && p.StatusId == 1))
                    {
                        processLauncher.StatusId = 4;
                    }
                }

                _context.SaveChanges();
                transaction.Commit();
            }

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = true
            };

            return jsonNetResult;
        }


        //Запустить
        [HttpPost]
        public ActionResult RunProcess(List<int> processList, int year, int month)
        {
            CheckCurrentPeriod(year, month);

            //Проверим каждый процесс на возможность запускаs
            //Если в списке идут зависимые процессы, то будем считать, что предыдущий завершился успешно
            bool check = true;
            string message = string.Empty;

            foreach (var i in processList)
            {
                if (!CanRun(i, year, month, processList, ref message))
                    check = false;
            }

            if (!check)
                throw new ApplicationException(
                    String.Format("Процессы не могут быть запущены, не все условия выполенны:{0}", message));

            using (var transaction = _context.Database.BeginTransaction())
            {
                foreach (var i in processList)
                {
                    var pl = _context.ProcessLauncher.First(p => p.ProcessId == i && p.Year == year && p.Month == month);
                    pl.StartTime = DateTime.Now;
                    pl.StatusId = 1;
                    pl.Comment = null;
                    pl.EndTime = null;
                    pl.UserId = new Guid(User.Identity.GetUserId());
                }

                _context.SaveChanges();

                transaction.Commit();
            }

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = true
            };

            return jsonNetResult;
        }

        private static void CheckCurrentPeriod(int year, int month)
        {
            //Проверим что текущий месяц является выпускаемым
            lock (LockObject)
            {
                if (!CalculationPeriod.HasValue)
                {
                    throw new ApplicationException("Период еще запущен");
                }

                if (CalculationPeriod.Value.Year != year || CalculationPeriod.Value.Month != month)
                {
                    throw new ApplicationException("Запущен другой период");
                }
            }
        }

        /// <summary>
        /// Проверяем может ли быть запущен текущий процесс
        /// </summary>
        /// <param name="ProcessId">Процесс</param>
        /// <param name="month">Месяц</param>
        /// <param name="year">Год</param>
        /// <param name="processList">Список всех запущенных процессов</param>
        /// <param name="message">Возвращает описание ошибки</param>
        /// <returns></returns>
        private bool CanRun(int ProcessId, int year, int month, List<int> processList, ref string message)
        {
            string message1 = " - уже выполняется. \r\t";
            string message2 = " - не все предыдущие процессы выполнены или поставлены на выполенение. \r\t";


            #region Список базовых процессов

            //1   Отвязать аптеки
            //2   Привязать аптеки
            //3   Расчитать цены
            //4   Отдать суммы в ГС
            //5(3)   Отдать на классификацию
            //6(4)   Забрать классификатор
            //7(5)   Забрать классификацию
            //8(6)   Проверить ГС
            //9(7)   Разбриковка ОФД

            #endregion


            List<int> canRunStatus = new List<int>() {0,3,4,5};

            List<int> goodStatusId = new List<int>() {1,2,3};

            List<int> baseProcess = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };


            if (ProcessId < 10)
            {
                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == ProcessId && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));

                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }
            }


            if (baseProcess.Contains(ProcessId))
                return true;

            //Подготовить к расчету БД
            if (ProcessId == 10)
            {

                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;
                //Должны быть выполнены или запущены 9 предыдущих этапов
                var flag = true;

                //Проверяем зависимости
                foreach (var i in baseProcess)
                {
                    if (!processList.Contains(i))
                    {
                        var processFlag = _context.ProcessLauncher.Any(p =>
                            p.ProcessId == i && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId));

                        flag = flag && processFlag;
                    }
                }

                if (!flag)
                {
                    message += $"\"{processName}\" {message2}";
                }

                //Проверяем текущий статус
                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 10 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));

                if(!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                }

                flag = flag && currentStatusCanRun;

                return flag;
            }

            //Рассчитать БД продаж
            if (ProcessId == 11)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 11 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));

                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }

                if (processList.Contains(10))
                    return true;

                if (_context.ProcessLauncher.Any(p =>
                    p.ProcessId == 10 && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId)))
                    return true;

                message += $"\"{processName}\" {message2}";
                return false;
            }

            //Рассчитать БД закупок
            if (ProcessId == 12)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 12 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));


                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }

                if (processList.Contains(11))
                    return true;

                if (_context.ProcessLauncher.Any(p =>
                    p.ProcessId == 11 && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId)))
                    return true;

                message += $"\"{processName}\" {message2}";
                return false;
            }

            //Рассчитать БД доп ассортимента
            if (ProcessId == 13)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 13 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));


                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }

                if (processList.Contains(11))
                    return true;

                if (_context.ProcessLauncher.Any(p =>
                    p.ProcessId == 11 && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId)))
                    return true;

                message += $"\"{processName}\" {message2}";
                return false;
            }


            //Объединить БД доп ассортимента
            if (ProcessId == 14)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 14 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));


                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }

                if (processList.Contains(13))
                    return true;

                if (_context.ProcessLauncher.Any(p =>
                    p.ProcessId == 13 && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId)))
                    return true;

                message += $"\"{processName}\" {message2}";
                return false;
            }

            //Применить правила закупок
            if (ProcessId == 15)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 15 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));

                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }

                if (processList.Contains(12))
                    return true;

                if (_context.ProcessLauncher.Any(p =>
                    p.ProcessId == 12 && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId)))
                    return true;


                message += $"\"{processName}\" {message2}";
                return false;
            }

            //Применить правила продаж
            if (ProcessId == 16)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 16 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));

                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }

                if (processList.Contains(11))
                    return true;

                if (_context.ProcessLauncher.Any(p =>
                    p.ProcessId == 11 && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId)))
                    return true;

                message += $"\"{processName}\" {message2}";
                return false;
            }

            //Создать аггрегаты
            if (ProcessId == 17)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 17 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));

                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }

                var purchaseH = processList.Contains(13) || _context.ProcessLauncher.Any(p =>
                    (p.ProcessId == 15) && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId));

                var purchaseS = processList.Contains(14) || _context.ProcessLauncher.Any(p =>
                    (p.ProcessId == 16) && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId));

                if (purchaseH && purchaseS)
                {
                    return true;
                }

                message += $"\"{processName}\" {message2}";
                return false;
              
            }

            if (ProcessId == 18)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                //Проверим, что мы можем запустить текущий процесс
                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 18 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));

                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }


                var check = processList.Contains(15) || _context.ProcessLauncher.Any(p =>
                    (p.ProcessId == 15) && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId));


                if (check)
                {
                    return true;
                }

                message += $"\"{processName}\" {message2}";
                return false;

            }

            if (ProcessId == 19)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                //Проверим, что мы можем запустить текущий процесс
                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 19 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));

                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }


                var check = processList.Contains(16) || _context.ProcessLauncher.Any(p =>
                    (p.ProcessId == 16) && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId));


                if (check)
                {
                    return true;
                }

                message += $"\"{processName}\" {message2}";
                return false;

            }

            if (ProcessId == 20)
            {
                var processName = _context.ProcessProcess.First(p => p.Id == ProcessId).Name;

                //Проверим, что мы можем запустить текущий процесс
                var currentStatusCanRun = _context.ProcessLauncher.Any(p =>
                    p.ProcessId == 20 && p.Year == year && p.Month == month && canRunStatus.Contains(p.StatusId));

                if (!currentStatusCanRun)
                {
                    message += $"\"{processName}\" {message1}";
                    return false;
                }


                var check = processList.Contains(17) || _context.ProcessLauncher.Any(p =>
                    (p.ProcessId == 17) && p.Year == year && p.Month == month && goodStatusId.Contains(p.StatusId));


                if (check)
                {
                    return true;
                }

                message += $"\"{processName}\" {message2}";
                return false;

            }

            return false;
        }
    }

}
