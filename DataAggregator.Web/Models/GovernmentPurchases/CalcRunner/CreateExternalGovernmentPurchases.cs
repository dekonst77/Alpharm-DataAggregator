using System;
using DataAggregator.Web.DatabaseManagerService;


namespace DataAggregator.Web.Models.GovernmentPurchases.CalcRunner
{
    public class CreateExternalGovernmentPurchases
    {

        public bool IsRunning { get; set; }

        public string Message { get; set; }

        private static volatile CreateExternalGovernmentPurchases _instance;

        private static object syncRoot = new Object();

        private CreateExternalGovernmentPurchases()
        {
            
        }

        public void Start()
        {
            //Если уже запущен - выходим
            if(IsRunning)
                return;
            
            try
            {
                IsRunning = true;
                Message = "Запущен";
                using (DatabaseManagerServiceClient client = new DatabaseManagerServiceClient())
                {
                    client.CreateGovernmentPurchases();
                }
            }
            catch (Exception e)
            {
                Message = "Произошла ошибка " + e.Message;
                throw;
            }
            finally
            {
                Message = "Не запущен";
                IsRunning = false;
            }
        }


        public void StartCreateExternalShipment()
        {
            //Если уже запущен - выходим
            if (IsRunning)
                return;

            try
            {
                IsRunning = true;
                Message = "Запущен";
                using (DatabaseManagerServiceClient client = new DatabaseManagerServiceClient())
                {
                    client.CreateExternalShipment();
                }
            }
            catch (Exception e)
            {
                Message = "Произошла ошибка " + e.Message;
                throw;
            }
            finally
            {
                Message = "Не запущен";
                IsRunning = false;
            }
        }

        public void StartCalcAveragePrice()
        {
            //Если уже запущен - выходим
            if (IsRunning)
                return;

            try
            {
                IsRunning = true;
                Message = "Запущен";
                using (DatabaseManagerServiceClient client = new DatabaseManagerServiceClient())
                {
                    client.CalcAveragePrice();
                }
            }
            catch (Exception e)
            {
                Message = "Произошла ошибка " + e.Message;
                throw;
            }
            finally
            {
                Message = "Не запущен";
                IsRunning = false;
            }
        }


        public void StartRunGovernmentSegmentShipmentJob()
        {
            //Если уже запущен - выходим
            if (IsRunning)
                return;

            try
            {
                IsRunning = true;
                Message = "Запущен";
                using (DatabaseManagerServiceClient client = new DatabaseManagerServiceClient())
                {
                    client.RunGovernmentSegmentShipmentJob();
                }
            }
            catch (Exception e)
            {
                Message = "Произошла ошибка " + e.Message;
                throw;
            }
            finally
            {
                Message = "Не запущен";
                IsRunning = false;
            }
        }

        public static CreateExternalGovernmentPurchases Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new CreateExternalGovernmentPurchases();
                    }
                }
                return _instance;
            }
        }
    }
}