using AutoMapper;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Web.Mapper.Classifier;
using DataAggregator.Web.Mapper.LPU;
using DataAggregator.Web.Mapper.OFD;
using DataAggregator.Web.Mapper.Retail;
using DataAggregator.Web.Mapper.RetailCalculation;
using DataAggregator.Web.Models.RetailCalculation;

namespace DataAggregator.Web
{
    internal static class ModelMapper
    {
        private static bool _isInitiated;

        public static IMapper Mapper { get; private set; }

        public static void Init()
        {
            if (_isInitiated)
                return;

            //новая версия нет этого метода 20200428
            //AutoMapper.Mapper.Initialize(s => { });

            var config = new MapperConfiguration(cfg =>
            {
                //Retail
                cfg.AddProfile<CountRuleProfile>();
                cfg.AddProfile<CountRuleFullVolumeProfile>();
                cfg.AddProfile<LauncherProfile>();
                //User
                cfg.AddProfile<DepartmentProfile>();
                //Goods
                cfg.AddProfile<GoodsCountRuleProfile>();
                cfg.AddProfile<GoodsCountRuleFullVolumeProfile>();
                //Репорт
                cfg.AddProfile<ReportLauncherProfile>();//-------------throw  new Exception("обновили и теперь надо найти проблему");
                //ОФД
                cfg.AddProfile<PriceEtalonProfile>();
                cfg.AddProfile<PriceCurrentProfile>();
                cfg.AddProfile<ClassifierHistoryProfile>();
                //Classifier
                cfg.AddProfile<ClassificationGenericModelProfile>();
                cfg.AddProfile<LPUModelProfile>();
                cfg.AddProfile<RetailProfile>();
            });

            Mapper = config.CreateMapper();
            config.AssertConfigurationIsValid();

            _isInitiated = true;
        }
    }
}