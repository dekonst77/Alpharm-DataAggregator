angular.module('DataAggregatorModule').config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    $locationProvider.hashPrefix('');

    setupMainRouting();
    setupManagementRouting();
    setupSystematizationRouting();
    setupClassifierRouting();
    setupRetailRouting();
    setupGovernmentPurchasesRouting();
    setupOFDRouting();
    setupClietsRouting();
    setupGSRouting();
    setupDistrRepRouting();
    setupLPURouting();


    function setupOFDRouting() {
        //
        $routeProvider
            // окно ОФД
            .when('/OFD/Logs',
                {
                    templateUrl: 'Views/OFD/logs.html',
                    title: 'Логи действий по ОФД',
                    authorize: true,
                    isEditor: true
                })
            // окно ОФД
            .when('/OFD/Action',
                {
                    templateUrl: 'Views/OFD/Action.html',
                    title: 'Действия по ОФД',
                    authorize: true,
                    isEditor: true
                })
            // окно ОФД
            .when('/OFD/Aggregated_All_Edit',
                {
                    templateUrl: 'Views/OFD/Aggregated_All_Edit.html',
                    title: 'Редактор Агрегатов',
                    authorize: true,
                    isEditor: true
                })
            .when('/OFD/4S_Data_All_Edit',
                {
                    templateUrl: 'Views/OFD/4S_Data_All_Edit.html',
                    title: 'Редактор 4CC',
                    authorize: true,
                    isEditor: true
                })
            .when('/OFD/4SC_Agreement_Edit',
                {
                    templateUrl: 'Views/OFD/4SC_Agreement_Edit.html',
                    title: 'Редактор 4CC',
                    authorize: true,
                    isEditor: true
                })
            .when('/OFD/GS_ToOFD_Edit',
                {
                    templateUrl: 'Views/OFD/GS_ToOFD_Edit.html',
                    title: 'Фиксация точек',
                    authorize: true,
                    isEditor: true
                })
            .when('/OFD/Files',
                {
                    templateUrl: 'Views/OFD/Files.html',
                    title: 'Файлообмен ОФД',
                    authorize: true,
                    isEditor: true
                })
            // окно ОФД
            .when('/OFD/Periods',
                {
                    templateUrl: 'Views/OFD/Periods.html',
                    title: 'Периоды Дней',
                    authorize: true,
                    isEditor: true
                })
            // окно ОФД
            .when('/OFD/Periods_4SC',
                {
                    templateUrl: 'Views/OFD/Periods_4SC.html',
                    title: 'Периоды Дней 4СС',
                    authorize: true,
                    isEditor: true
                })
            .when('/OFD/PeriodsWK',
                {
                    templateUrl: 'Views/OFD/PeriodsWK.html',
                    title: 'Периоды Недель',
                    authorize: true,
                    isEditor: true
                })
            // окно ОФД
            .when('/OFD/Report',
                {
                    templateUrl: 'Views/OFD/Report.html',
                    title: 'Отчёты по ОФД',
                    authorize: true,
                    isEditor: true
                })
            // окно ОФД
            .when('/OFD/PriceCurrent',
                {
                    templateUrl: 'Views/OFD/PriceCurrent/Index.html',
                    title: 'OFD.PRICE_ETALON.TITLE',
                    authorize: true,
                    isEditor: true
                })
            // окно ОФД
            .when('/OFD/PriceCurrent_v2',
                {
                    templateUrl: 'Views/OFD/PriceCurrent/Index_v2.html',
                    title: 'OFD.PRICE_ETALON.TITLE',
                    authorize: true,
                    isEditor: true
                })
            // окно ОФД
            .when('/OFD/PriceEtalon',
                {
                    templateUrl: 'Views/OFD/PriceEtalon/Index.html',
                    title: 'OFD.PRICE_HISTORY.TITLE',
                    authorize: true
                })

            // Справочник подразделений
            .when('/DepartmentDictionary',
                {
                    templateUrl: 'Views/Management/DepartmentDictionary.html',
                    title: 'MAIN.DEPARTMENT_DICTIONARY.TITLE',
                    authorize: true
                })
            .when('/Projects',
                {
                    templateUrl: 'Views/Management/Projects.html',
                    title: 'MAIN.DEPARTMENT_DICTIONARY.TITLE',
                    authorize: true
                })
            .when('/Global',
                {
                    templateUrl: 'Views/Common/Table.html',
                    title: 'Таблица',
                    authorize: true
                })
            .when('/GlobalQuery',
                {
                    templateUrl: 'Views/Common/query.html',
                    title: 'Редактор отчетов',
                    authorize: true
                });


    }
    // Management
    function setupDistrRepRouting() {
        $routeProvider
            .when('/DistrRep/DataFiles',
                {
                    templateUrl: 'Views/DistrRep/DataFiles.html',
                    title: 'DataFiles',
                    authorize: true
                })
            .when('/DistrRep/EditSource',
                {
                    templateUrl: 'Views/DistrRep/EditSource.html',
                    title: 'EditSource',
                    authorize: true
                })
            .when('/Check/CheckReload',
                {
                    templateUrl: 'Views/DistrRep/CheckReload.html',
                    title: 'CheckReload',
                    authorize: true
                })
            .when('/DistrRep/HistoryGS',
                {
                    templateUrl: 'Views/DistrRep/HistoryGS.html',
                    title: 'HistoryGS',
                    authorize: true
                })
            .when('/DistrRep/TovarActions',
                {
                    templateUrl: 'Views/DistrRep/TovarActions.html',
                    title: 'TovarActions',
                    authorize: true
                })
            .when('/DistrRep/CompanyPeriod',
                {
                    templateUrl: 'Views/DistrRep/CompanyPeriod.html',
                    title: 'Периоды для публикации',
                    authorize: true
                })
            .when('/DistrRep/Rules',
                {
                    templateUrl: 'Views/DistrRep/Rules.html',
                    title: 'Rules',
                    authorize: true
                })
            .when('/DistrRep/RawData',
                {
                    templateUrl: 'Views/DistrRep/RawData.html',
                    title: 'Посчитанные данные',
                    authorize: true
                })
            ;
    }
    // Main
    function setupMainRouting() {
        $routeProvider

            // Главная страница
            .when('/',
                {
                    templateUrl: 'Views/Main/Index.html',
                    title: 'MAIN.TITLE',
                    authorize: true
                })

            // Логин
            .when('/Login',
                {
                    templateUrl: 'Views/Account/Login.html',
                    title: 'MAIN.TITLE'
                })

            // redirect если ничего не найдено
            .otherwise('/')

            ;
    }

    // Management
    function setupManagementRouting() {
        //$routeProvider


        //;
    }

    // Систематизация
    function setupSystematizationRouting() {
        $routeProvider

            // Обработка доп. ассортимента
            .when('/Systematization/Raspredelenie',
                {
                    templateUrl: 'Views/Classifier/Raspredelenie.html',
                    title: 'Распределение',
                    authorize: true
                })
            // Обработка доп. ассортимента
            .when('/Systematization/GoodsSystematization',
                {
                    templateUrl: 'Views/GoodsSystematization/Index.html',
                    title: 'SYSTEMATIZATION.GOODS_SYSTEMATIZATION.TITLE',
                    authorize: true
                })
            // Обработка данных
            .when('/Systematization/DrugGoodClassifier',
                {
                    templateUrl: 'Views/Systematization/DrugGoodClassifier.html',
                    title: 'SYSTEMATIZATION.SYSTEMATIZATION.TITLE',
                    authorize: true
                })
            // Настройка
            .when('/Systematization/PeriodsSettings',
                {
                    templateUrl: 'Views/Systematization/PeriodsSettings/Index.html',
                    title: 'SYSTEMATIZATION.PERIODS_SETTINGS.TITLE',
                    authorize: true
                })
            .when('/Systematization/PrioritetWords',
                {
                    templateUrl: 'Views/Systematization/PrioritetWords.html',
                    title: 'Приоритетные слова',
                    authorize: true
                })
            ;
    }

    // Clients
    function setupClietsRouting() {
        $routeProvider  // Клиенты-компании
            .when('/Clients/Companies',
                {
                    templateUrl: 'Views/Clients/Companies.html',
                    title: 'Компании',
                    authorize: true,
                    isEditor: true
                })
            // Клиенты-Сотрудники
            .when('/Clients/Workers',
                {
                    templateUrl: 'Views/Clients/Workers.html',
                    title: 'Сотрудники компаний',
                    authorize: true,
                    isEditor: true
                })
            // клиенты-Отчёты
            .when('/Clients/Reports',
                {
                    templateUrl: 'Views/Clients/Reports.html',
                    title: 'Рассылаемые отчёты',
                    authorize: true,
                    isEditor: true
                })
        // Запуск работ
    }
    // GS
    function setupGSRouting() {
        // Запуск работ
        $routeProvider
            .when('/Classifier/Licenses',
                {
                    templateUrl: 'Views/Classifier/ClassifierRelease/Index.html',
                    title: 'GS.Licenses.TITLE',
                    authorize: true,
                    isEditor: true
                })
            // Редактор ГС реестр
            .when('/GS/GS',
                {
                    templateUrl: 'Views/GS/GS.html',
                    title: 'Генеральная совокупность',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/Bricks',
                {
                    templateUrl: 'Views/GS/Bricks.html',
                    title: 'Брики',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/Bricks_Region',
                {
                    templateUrl: 'Views/GS/Bricks_Region.html',
                    title: 'Брики Регионы',
                    authorize: true,
                    isEditor: true
                })
            // Редактор Лицензий реестр
            .when('/GS/Licenses',
                {
                    templateUrl: 'Views/GS/Licenses.html',
                    title: 'Лицензии',
                    authorize: true,
                    isEditor: true
                })
            // Редактор Адреса для базы
            .when('/GS/base_address',
                {
                    templateUrl: 'Views/GS/base_address.html',
                    title: 'Адреса для базы',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/SummsAlphaBit',
                {
                    templateUrl: 'Views/GS/SummsAlphaBit.html',
                    title: 'Суммы АльфаБит',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/SummsOFD',
                {
                    templateUrl: 'Views/GS/SummsOFD.html',
                    title: 'Суммы ОФД',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/SummsRegion',
                {
                    templateUrl: 'Views/GS/SummsRegion.html',
                    title: 'Суммы Регинов',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/SummsNetwork',
                {
                    templateUrl: 'Views/GS/SummsNetwork.html',
                    title: 'Сети Тотал',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/Network',
                {
                    templateUrl: 'Views/GS/Network.html',
                    title: 'Карточки АС',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/SummsAnket',
                {
                    templateUrl: 'Views/GS/SummsAnket.html',
                    title: 'Суммы Анкет',
                    authorize: true,
                    isEditor: true
                })
            // Редактор Адреса для базы
            .when('/GS/SummsPeriod',
                {
                    templateUrl: 'Views/GS/SummsPeriod.html',
                    title: 'Основной Расчёт',
                    authorize: true,
                    isEditor: true
                })
            // Обработка
            .when('/GS/History',
                {
                    templateUrl: 'Views/GS/History.html',
                    title: 'Мэчинг точек',
                    authorize: true,
                    isEditor: true
                })
            // Обработка
            .when('/GS/Point',
                {
                    templateUrl: 'Views/GS/Point.html',
                    title: 'Точки',
                    authorize: true,
                    isEditor: true
                })
            // Обработка
            .when('/GS/DistributorBranch',
                {
                    templateUrl: 'Views/GS/DistributorBranch.html',
                    title: 'Дистрибьюторы',
                    authorize: true,
                    isEditor: true
                })
            // Обработка
            .when('/GS/OperationMode',
                {
                    templateUrl: 'Views/GS/OperationMode.html',
                    title: 'Режим Работы',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/NetworkBrand',
                {
                    templateUrl: 'Views/GS/NetworkBrand.html',
                    title: 'Бренды Сетей',
                    authorize: true,
                    isEditor: true
                })
            // Редактор Адреса для базы
            .when('/GS/Organization',
                {
                    templateUrl: 'Views/GS/Organization.html',
                    title: 'Организации',
                    authorize: true,
                    isEditor: true
                })
            // Организации без ИНН Для ЛПУ
            .when('/GS/Organization_without_INN',
                {
                    templateUrl: 'Views/GS/Organization_without_INN.html',
                    title: 'Организации без ИНН',
                    authorize: true,
                    isEditor: true
                })
            .when('/GS/BookOfChange',
             {
                 templateUrl: 'Views/GS/BookOfChange.html',
                 title: 'Книга перемен',
                 authorize: true,
                 isEditor: true
             });
    }
    //ЛПУ
    function setupLPURouting() {
        $routeProvider
            // ЛПУ
            .when('/LPU/LPU',
                {
                    templateUrl: 'Views/LPU/lpu.html',
                    title: 'ЛПУ',
                    authorize: true
                })
            .when('/LPU/LPUPoint',
                {
                    templateUrl: 'Views/LPU/lpuPoint.html',
                    title: 'ЛПУ Адреса',
                    authorize: true
                })
            .when('/LPU/LPULicenses',
                {
                    templateUrl: 'Views/LPU/LpuLicenses.html',
                    title: 'ЛПУ Лицензии',
                    authorize: true
                })
    }


    // Классификация
    function setupClassifierRouting() {
        $routeProvider
            // Маска
            .when('/Classifier/Mask',
                {
                    templateUrl: 'Views/Classifier/Mask.html',
                    title: 'Распределение',
                    authorize: true
                })
            // Редактор классификатора
            .when('/Classifier/ClassifierEditor',
                {
                    templateUrl: 'Views/Classifier/ClassifierEditor/Index.html',
                    title: 'CLASSIFIER.CLASSIFIER_EDITOR.TITLE',
                    authorize: true,
                    isEditor: false
                })

            // Редактор классификатора - изменение
            .when('/Classifier/ClassifierEditor/Edit',
                {
                    templateUrl: 'Views/Classifier/ClassifierEditor/Index.html',
                    title: 'CLASSIFIER.CLASSIFIER_EDITOR_CHANGE.TITLE',
                    authorize: true,
                    isEditor: true
                })
            // Редактор классификатора - изменение справочника
            .when('/Classifier/SPR',
                {
                    templateUrl: 'Views/Classifier/SPR.html',
                    title: 'Справочник',
                    authorize: true,
                    isEditor: true
                })
            .when('/Classifier/DataChangeExcel',
                {
                    templateUrl: 'Views/Classifier/DataChangeExcel.html',
                    title: 'Обмен через Excel',
                    authorize: true,
                    isEditor: true
                })
            .when('/Classifier/Checkeds',
                {
                    templateUrl: 'Views/Classifier/Checkeds.html',
                    title: 'Бессмертные',
                    authorize: true,
                    isEditor: true
                })
            .when('/Classifier/Certificates',
                {
                    templateUrl: 'Views/Classifier/Certificate/Certificates.html',
                    title: 'Сертификаты',
                    authorize: true,
                    isEditor: true
                })
            .when('/Classifier/Certificate',
                {
                    templateUrl: 'Views/Classifier/Certificate/Certificate.html',
                    title: 'Сертификат',
                    authorize: true,
                    isEditor: true
                })
            .when('/Classifier/Subtance',
                {
                    templateUrl: 'Views/Classifier/Certificate/Subtance.html',
                    title: 'Субстанции',
                    authorize: true,
                    isEditor: true
                })
            .when('/Classifier/ESKLP',
                {
                    templateUrl: 'Views/Classifier/Certificate/ESKLP.html',
                    title: 'ЕСКЛП',
                    authorize: true,
                    isEditor: true
                })
            .when('/Classifier/MnfWay',
                {
                    templateUrl: 'Views/Classifier/Certificate/MnfWay.html',
                    title: 'Стадии Производства',
                    authorize: true,
                    isEditor: true
                })
            // Редактор производителей - изменение
            .when('/Classifier/Manufacturer/Edit',
                {
                    templateUrl: 'Views/Classifier/Manufacturer/Index.html',
                    title: 'CLASSIFIER.Manufacturer_EDITOR_CHANGE.TITLE',
                    authorize: true,
                    isEditor: true
                })

            // Редактор доп. ассортимента
            .when('/Classifier/GoodsClassifierEditor',
                {
                    templateUrl: 'Views/Classifier/GoodsClassifierEditor/Index.html',
                    title: 'CLASSIFIER.GOODS_CLASSIFIER_EDITOR.TITLE',
                    authorize: true,
                    isEditor: false
                })

            // Редактор доп. ассортимента - изменение
            .when('/Classifier/GoodsClassifierEditor/Edit',
                {
                    templateUrl: 'Views/Classifier/GoodsClassifierEditor/Index.html',
                    title: 'CLASSIFIER.GOODS_CLASSIFIER_EDITOR_CHANGE.TITLE',
                    authorize: true,
                    isEditor: true
                })

            // Отчет по классификатору доп. ассортимента
            .when('/Classifier/GoodsClassifierReport',
                {
                    templateUrl: 'Views/Classifier/GoodsClassifierReport/GoodsClassifierReport.html',
                    title: 'CLASSIFIER.GOODS_CLASSIFIER_REPORT.TITLE',
                    authorize: true
                })

            // Редактор категорий дополнительного ассортимента
            .when('/Classifier/GoodsCategoryEditor',
                {
                    templateUrl: 'Views/Classifier/GoodsCategoryEditor/Index.html',
                    title: 'CLASSIFIER.GOODS_CATEGORY_EDITOR.TITLE',
                    authorize: true
                })

            // Редактор свойств дополнительного ассортимента
            .when('/Classifier/GoodsParametersEditor',
                {
                    templateUrl: 'Views/Classifier/GoodsParametersEditor/Index.html',
                    title: 'CLASSIFIER.GOODS_PARAMETERS_EDITOR.TITLE'
                })

            // Редактор ФТГ
            .when('/Classifier/FTG',
                {
                    templateUrl: 'Views/Classifier/FTG/Index.html',
                    title: 'CLASSIFIER.FTG.TITLE',
                    authorize: true
                })

            // Редактор ATCWho
            .when('/Classifier/ATCWho',
                {
                    templateUrl: 'Views/Classifier/ATCWho/Index.html',
                    title: 'CLASSIFIER.ATC_WHO.TITLE',
                    authorize: true
                })

            // Редактор ATCEphmra
            .when('/Classifier/ATCEphmra',
                {
                    templateUrl: 'Views/Classifier/ATCEphmra/Index.html',
                    title: 'CLASSIFIER.ATC_EPHMRA.TITLE',
                    authorize: true
                })

            // Редактор ATC Бад
            .when('/Classifier/ATCBAA',
                {
                    templateUrl: 'Views/Classifier/ATCBAA/Index.html',
                    title: 'CLASSIFIER.ATC_BAA.TITLE',
                    authorize: true
                })

            // Редактор SQA
            .when('/Classifier/SQA',
                {
                    templateUrl: 'Views/Classifier/SQA/Index.html',
                    title: 'CLASSIFIER.SQA.TITLE',
                    authorize: true
                })

            // Редактор Дженерик
            .when('/Classifier/GENERIC',
                {
                    templateUrl: 'Views/Classifier/Generic/Index.html',
                    title: 'CLASSIFIER.GENERIC.TITLE',
                    authorize: true
                })


            // Редактор NFC
            .when('/Classifier/NFC',
                {
                    templateUrl: 'Views/Classifier/NFC/Index.html',
                    title: 'CLASSIFIER.NFC.TITLE',
                    authorize: true
                })
            .when('/Classifier/DDD_Norma',
                {
                    templateUrl: 'Views/Classifier/DDD/DDD_Norma.html',
                    title: 'DDD Норма',
                    authorize: true
                })
            .when('/Classifier/DDD',
                {
                    templateUrl: 'Views/Classifier/DDD/DDD.html',
                    title: 'DDD',
                    authorize: true
                })
            .when('/Classifier/StandardUnits',
                {
                    templateUrl: 'Views/Classifier/DDD/StandardUnits.html',
                    title: 'StandardUnits',
                    authorize: true
                })
            // ЖНВЛП
            .when('/Classifier/VED',
                {
                    templateUrl: 'Views/Classifier/VED/Index.html',
                    title: 'CLASSIFIER.VED.TITLE',
                    authorize: true
                })

            // Федеральная льгота
            .when('/Classifier/FederalBenefit',
                {
                    templateUrl: 'Views/Classifier/FederalBenefit/Index.html',
                    title: 'CLASSIFIER.FEDERAL_BENEFIT.TITLE',
                    authorize: true
                })

            // Выпуск классификатор
            .when('/Classifier/ClassifierRelease', {
                templateUrl: 'Views/Classifier/ClassifierRelease/Index.html',
                title: 'CLASSIFIER.TITLE',
                authorize: true
            })

            // Переброс данных
            .when('/Classifier/DataTransfer',
                {
                    templateUrl: 'Views/Classifier/DataTransfer/Index.html',
                    title: 'CLASSIFIER.DATA_TRANSFER.TITLE',
                    authorize: true
                })

            // Бионика Медиа
            .when('/Classifier/BionicaMediaReport',
                {
                    templateUrl: 'Views/Classifier/Reports/BionicaMediaReport/Index.html',
                    title: 'CLASSIFIER.REPORT_BIONICA_MEDIA.TITLE',
                    authorize: true
                })

            // Выпуск классификатор
            .when('/Classifier/ClassifierRelease', {
                templateUrl: 'Views/Classifier/ClassifierRelease/Index.html',
                title: 'CLASSIFIER.CLASSIFIER_RELEASE.TITLE',
                authorize: true
            })

            // Блок «блистеровка»
            .when('/Classifier/BlisterBlock', {
                templateUrl: 'Views/Classifier/BlisterBlock/BlisterBlock.html',
                title: 'CLASSIFIER.BLISTERBLOCK.TITLE',
                authorize: true
            })

            // Отчет проверки классификатора
            .when('/Classifier/CheckReport',
                {
                    templateUrl: 'Views/Classifier/Reports/CheckReport/CheckReport.html',
                    title: 'CLASSIFIER.CHECK_REPORT.TITLE',
                    authorize: true
                })

            // Модуль для добавления ДОП ассортимента в БД мониторинг, разработка #11668
            .when('/Classifier/AddingDOPMonitoringDatabase', {
                templateUrl: 'Views/Classifier/AddingDOPMonitoringDatabase/AddingDOPMonitoringDatabase.html',
                title: 'CLASSIFIER.AddingDOPtotheMonitoringDatabase.TITLE',
                authorize: true
            });
    }

    // Розница
    function setupRetailRouting() {
        $routeProvider

            // Редактор источников и шаблонов
            .when('/Retail/RetailTemplates', {
                templateUrl: 'Views/Retail/RetailTemplates/Index.html',
                title: 'RETAIL.RETAIL_TEMPLATES_TITLE',
                authorize: true
            })

            // Загрузка файлов
            .when('/Retail/FileInfo', {
                templateUrl: 'Views/Retail/FileInfo/Index.html',
                title: 'RETAIL.FILE_INFO_TITLE',
                authorize: true
            })

            // Справочник аптек
            .when('/Retail/SourcePharmaciesEditor', {
                templateUrl: 'Views/Retail/SourcePharmaciesEditor/Index.html',
                title: 'RETAIL.SOURCE_PHARMACIES_EDITOR_TITLE',
                authorize: true
            })

            // Черный список аптека-бренд
            .when('/Retail/PharmacyBrandBlackList', {
                templateUrl: 'Views/Retail/PharmacyBrandBlackList/Index.html',
                title: 'RETAIL.PHARMACY_BRAND_BLACK_LIST_TITLE',
                authorize: true
            })

            // Черный список аптек
            .when('/Retail/PharmacyWithoutAverage', {
                templateUrl: 'Views/Retail/PharmacyWithoutAverage/Index.html',
                title: 'RETAIL.PHARMACY_WITHOUT_AVERAGE_LIST_TITLE',
                authorize: true
            })

            // Черный список брендов
            .when('/Retail/SourceBrandBlackList', {
                templateUrl: 'Views/Retail/SourceBrandBlackList/Index.html',
                title: 'RETAIL.SOURCE_BRAND_BLACK_LIST_TITLE',
                authorize: true
            })

            // Редактор цен
            .when('/Retail/PriceLimitsEditor', {
                templateUrl: 'Views/Retail/PriceLimitsEditor/Index.html',
                title: 'RETAIL.PRICE_LIMITS_EDITOR_TITLE',
                authorize: true
            })

            // Редактор количеств
            .when('/Retail/CountCheck', {
                templateUrl: 'Views/Retail/CountCheck/Index.html',
                title: 'RETAIL.COUNT_CHECK_TITLE',
                authorize: true
            })

            // Корректировка цен на итоговых данных
            .when('/Retail/PriceRuleEditor', {
                templateUrl: 'Views/Retail/PriceRuleEditor/Index.html',
                title: 'RETAIL.PRICE_RULE_EDITOR_TITLE',
                authorize: true
            })

            // Корректировка количеств на итоговых данных
            .when('/Retail/CountRuleEditor', {
                templateUrl: 'Views/Retail/CountRuleEditor/Index.html',
                title: 'RETAIL.COUNT_RULE_EDITOR.TITLE',
                authorize: true
            })

            // Корректировка наценок по умолчанию
            .when('/Retail/MarkupDefaultEditor', {
                templateUrl: 'Views/Retail/MarkupDefaultEditor/Index.html',
                title: 'RETAIL.MARKUP_DEFAULT_EDITOR_TITLE',
                authorize: true
            })

            // Корректировка цен доп. ассортимента на итоговых данных
            .when('/Retail/GoodsPriceRuleEditor', {
                templateUrl: 'Views/Retail/GoodsPriceRuleEditor/Index.html',
                title: 'RETAIL.GOODS_PRICE_RULE_EDITOR.TITLE',
                authorize: true
            })

            // Корректировка количеств доп. ассортимента на итоговых данных
            .when('/Retail/GoodsCountRuleEditor', {
                templateUrl: 'Views/Retail/GoodsCountRuleEditor/Index.html',
                title: 'RETAIL.GOODS_COUNT_RULE_EDITOR.TITLE',
                authorize: true
            })

            // Корректировка количеств доп. ассортимента на итоговых данных - 100%
            .when('/Retail/GoodsCountRuleFullVolumeEditor', {
                templateUrl: 'Views/Retail/GoodsCountRuleFullVolumeEditor/Index.html',
                title: 'RETAIL.GOODS_COUNT_RULE_EDITOR.FULL_VOLUME_TITLE',
                authorize: true
            })

            // Поиск в исходных данных по тексту
            .when('/Retail/SearchRawDataByDrugClear', {
                templateUrl: 'Views/Retail/SearchRawDataByDrugClear/Index.html',
                title: 'RETAIL.SEARCH_RAW_DATA_BY_DRUG_CLEAR.TITLE',
                authorize: true
            })

            // Поиск в исходных данных по классификатору
            .when('/Retail/SearchRawDataByClassifier', {
                templateUrl: 'Views/Retail/SearchRawDataByClassifier/Index.html',
                title: 'RETAIL.SEARCH_RAW_DATA_BY_CLASSIFIER.TITLE',
                authorize: true
            })

            // Поиск в исходных данных по классификатору
            .when('/Retail/SearchRawDataByGoodsClassifier', {
                templateUrl: 'Views/Retail/SearchRawDataByGoodsClassifier/Index.html',
                title: 'RETAIL.SEARCH_RAW_DATA_BY_CLASSIFIER.GOODS_TITLE',
                authorize: true
            })
            // Выпуск ретейла
            .when('/Retail/RetailReport', {
                templateUrl: 'Views/Retail/RetailReport/Index.html',
                title: 'RETAIL.REPORT.TITLE',
                authorize: true
            })
            // Выпуск Ecom
            .when('/Retail/Ecom_Coefficient', {
                templateUrl: 'Views/Retail/Ecom/Coefficients.html',
                title: 'Ecom Коэфициенты',
                authorize: true
            })
            // Выпуск ретейла
            .when('/Retail/RetailCalculation', {
                templateUrl: 'Views/Retail/RetailCalculation/Index.html',
                title: 'RETAIL.RETAIL_CALCULATION.TITLE',
                authorize: true
            })
            .when('/Retail/CTM', {
                templateUrl: 'Views/Retail/CTM/CTM.html',
                title: 'RETAIL.RETAIL_CTM.TITLE',
                authorize: true
            })
            // Блок продаж СКЮ по субъектам федерации
            .when('/Retail/SalesSKUbySF', {
                templateUrl: 'Views/Retail/SalesSKUbySF/SalesSKUBySF.html',
                title: 'RETAIL.RETAIL_SALESSKUBYSF.TITLE',
                authorize: true
            })
            //Фиксация правил
            .when('/Retail/RulesCommit', {
                templateUrl: 'Views/Retail/RulesCommit/Index.html',
                title: 'RETAIL.RULES_COMMIT.TITLE',
                authorize: true
            })
            ;
    }

    // Госзакупки
    function setupGovernmentPurchasesRouting() {
        $routeProvider

            // Добавление закупок на скачивание
            .when('/GovernmentPurchases/PurchaseLink',
                {
                    templateUrl: 'Views/GovernmentPurchases/PurchaseLink/Index.html',
                    title: 'GOVERNMENT_PURCHASES.PURCHASE_LINK.TITLE',
                    authorize: true
                })

            // Ключевые слова для распределения
            .when('/GovernmentPurchases/DistributionKeyWords',
                {
                    templateUrl: 'Views/GovernmentPurchases/DistributionKeyWords/Index.html',
                    title: 'GOVERNMENT_PURCHASES.DISTRIBUTION_KEY_WORDS.TITLE',
                    authorize: true
                })

            // Распределение в работу
            .when('/GovernmentPurchases/DistributionWork',
                {
                    templateUrl: 'Views/GovernmentPurchases/DistributionWork/Index.html',
                    title: 'GOVERNMENT_PURCHASES.DISTRIBUTION_WORK.TITLE',
                    authorize: true
                })

            // Распределение контрактов в работу
            .when('/GovernmentPurchases/ContractDistributionWork',
                {
                    templateUrl: 'Views/GovernmentPurchases/ContractDistributionWork/Index.html',
                    title: 'GOVERNMENT_PURCHASES.CONTRACT_DISTRIBUTION_WORK.TITLE',
                    authorize: true
                })
            // Проверка по сайту контракты
            .when('/GovernmentPurchases/Check/Contract',
                {
                    templateUrl: 'Views/GovernmentPurchases/Check/Contract/Index.html',
                    title: 'Проверка по сайту контракты',
                    authorize: true
                })
            // Дополнительная скачка и сравнение КБК по сайту http://s-dev1:8080/redmine/issues/6240#change-18690
            .when('/GovernmentPurchases/Check/ContractPaymentStage',
                {
                    templateUrl: 'Views/GovernmentPurchases/Check/ContractPaymentStage.html',
                    title: 'Дополнительная скачка и сравнение КБК по сайту',
                    authorize: true
                })
            // Редактирование закупок
            .when('/GovernmentPurchases/GovernmentPurchases',
                {
                    templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/Index.html',
                    title: 'GOVERNMENT_PURCHASES.GOVERNMENT_PURCHASES.TITLE',
                    authorize: true
                })

            // Редактирование организаций
            .when('/GovernmentPurchases/OrganizationsEditor',
                {
                    templateUrl: 'Views/GovernmentPurchases/OrganizationsEditor/Index.html',
                    title: 'GOVERNMENT_PURCHASES.ORGANIZATIONS_EDITOR.TITLE',
                    authorize: true
                })
            .when('/GovernmentPurchases/OrganizationRaw',
                {
                    templateUrl: 'Views/GovernmentPurchases/OrganizationRaw.html',
                    title: 'Организации Обработка',
                    authorize: true
                })
            // Редактирование поставщиков
            .when('/GovernmentPurchases/Suppliers',
                {
                    templateUrl: 'Views/GovernmentPurchases/Suppliers/Index.html',
                    title: 'GOVERNMENT_PURCHASES.SUPPLIERS.TITLE',
                    authorize: true
                })

            // Блок массовой замены
            .when('/GovernmentPurchases/MassFixesData',
                {
                    templateUrl: 'Views/GovernmentPurchases/MassFixesData/Index.html',
                    title: 'GOVERNMENT_PURCHASES.MASS_FIXES_DATA.TITLE',
                    authorize: true
                })
            // Блок массовой замены
            .when('/GovernmentPurchases/DeliveryTime',
                {
                    templateUrl: 'Views/GovernmentPurchases/MassFixesData/DeliveryTime.html',
                    title: 'Пустые периоды поставки',
                    authorize: true
                })
            // Перенос данных по форме выпуска
            .when('/GovernmentPurchases/ObjectsToObjectsReady',
                {
                    templateUrl: 'Views/GovernmentPurchases/ObjectsToObjectsReady/Index.html',
                    title: 'GOVERNMENT_PURCHASES.OBJECTS_TO_OBJECTS_READY.TITLE',
                    authorize: true
                })

            // Запуск расчета
            .when('/GovernmentPurchases/CalcRunner',
                {
                    templateUrl: 'Views/GovernmentPurchases/CalcRunner/Index.html',
                    title: 'GOVERNMENT_PURCHASES.CALC_RUNNER.TITLE',
                    authorize: true
                })

            // Редактирование расчетных данных
            .when('/GovernmentPurchases/CalculatedDataEditor',
                {
                    templateUrl: 'Views/GovernmentPurchases/CalculatedDataEditor/Index.html',
                    title: 'GOVERNMENT_PURCHASES.CALCULATED_DATA_EDITOR.TITLE',
                    authorize: true
                })
            // Редактирование расчетных данных
            .when('/GovernmentPurchases/HandMadePosition',
                {
                    templateUrl: 'Views/GovernmentPurchases/HandMadePosition.html',
                    title: 'Ручной ввод данных - вакцины',
                    authorize: true
                })
            .when('/GovernmentPurchases/Reports',
                {
                    templateUrl: 'Views/GovernmentPurchases/Reports/Reports.html',
                    title: 'Отчёты',
                    authorize: true
                })
            // Drug id с min/max ценой
            .when('/GovernmentPurchases/DrugIdWithMinMaxPriceReport',
                {
                    templateUrl: 'Views/GovernmentPurchases/Reports/DrugIdWithMinMaxPriceReport/Index.html',
                    title: 'GOVERNMENT_PURCHASES.REPORT_DRUG_ID_WITH_MIN_MAX_PRICE.TITLE',
                    authorize: true
                })
            .when('/GovernmentPurchases/KBK',
                {
                    templateUrl: 'Views/GovernmentPurchases/KBK.html',
                    title: 'KBK',
                    authorize: true
                })
            .when('/GovernmentPurchases/Budjet',
                {
                    templateUrl: 'Views/GovernmentPurchases/Budjet.html',
                    title: 'Бюджет',
                    authorize: true
                })
            // Неправильные цены
            .when('/GovernmentPurchases/WrongPricesReport',
                {
                    templateUrl: 'Views/GovernmentPurchases/Reports/WrongPricesReport/Index.html',
                    title: 'GOVERNMENT_PURCHASES.REPORT_WRONG_PRICES.TITLE',
                    authorize: true
                })

            // Не вылитые в External базу закупки
            .when('/GovernmentPurchases/NotExportedToExternalPurchasesReport',
                {
                    templateUrl: 'Views/GovernmentPurchases/Reports/NotExportedToExternalPurchasesReport/Index.html',
                    title: 'GOVERNMENT_PURCHASES.REPORT_NOT_EXPORTED_TO_EXTERNAL_PURCHASES.TITLE',
                    authorize: true
                })

            // Исполнение прекращено
            .when('/GovernmentPurchases/ExecutionTerminatedContractReport',
                {
                    templateUrl: 'Views/GovernmentPurchases/Reports/ExecutionTerminatedContractReport/Index.html',
                    title: 'GOVERNMENT_PURCHASES.REPORT_EXECUTION_TERMINATED_CONTRACT.TITLE',
                    authorize: true
                })

            // Статистика по закупкам/контрактам
            .when('/GovernmentPurchases/PurchasesAndContractsStatistics',
                {
                    templateUrl: 'Views/GovernmentPurchases/Statistics/PurchasesAndContractsStatistics/Index.html',
                    title: 'GOVERNMENT_PURCHASES.STATISTICS_PURCHASES_AND_CONTRACTS.TITLE',
                    authorize: true
                })
            .when('/GovernmentPurchases/AutoNature',
                {
                    templateUrl: 'Views/GovernmentPurchases/AutoNature.html',
                    title: 'Авто - Характер',
                    authorize: true
                })
            .when('/GovernmentPurchases/PurchasesFound',
                {
                    templateUrl: 'Views/GovernmentPurchases/PurchasesLoader/PurchasesFound/Index.html',
                    title: 'GOVERNMENT_PURCHASES.PURCHASES_FOUND.TITLE',
                    authorize: true
                })
            // Редактор ключевых слов для клиентских фильтров
            .when('/GovernmentPurchases/ClientKeywords',
                {
                    templateUrl: 'Views/GovernmentPurchases/ClientKeywords/Index.html',
                    title: 'GOVERNMENT_PURCHASES.CLIENT_KEYWORDS.TITLE',
                    authorize: true
                })

            ;
    }
}]);

angular.module('DataAggregatorModule').run(['$rootScope', '$window', '$translate', function ($rootScope, $window, $translate) {

    var authorizationResolver =
        [
            'userService', '$location', '$q', function (userService, $location, $q) {

                function checkUser() {
                    var user = userService.getUser();
                    return user && user.IsAuthenticated;
                }

                // Check without loading
                if (checkUser())
                    return true;

                return userService.loadUser().then(function (response) {
                    // Check after loading
                    if (checkUser())
                        return response;

                    var url = $location.url();
                    $location.path('/Login').search({ returnUrl: url });

                    return $q.reject();
                });
            }
        ];

    $rootScope.$on('$routeChangeStart', function (event, next, current) {
        if (!next.hasOwnProperty('$$route'))
            return;

        if (next.authorize) {
            next.resolve = next.resolve || {};
            if (!next.resolve.authorizationResolver)
                next.resolve.authorizationResolver = authorizationResolver;
        }
    });


    $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
        if (!current.hasOwnProperty('$$route'))
            return;
        $window.document.title = $translate.instant(current.$$route.title);
    });
}]);