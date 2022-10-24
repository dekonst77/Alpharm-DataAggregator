angular
    .module('DataAggregatorModule')
    .controller('DistributionWorkController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'formatConstants', DistributionWorkController]);

function DistributionWorkController(messageBoxService, $scope, $http, uiGridCustomService, formatConstants) {
    //Методы
    $scope.setAssignedToUser = function() {
        var purchasesId = $scope.purchase.getSelected("Id");
        var userId = $scope.filter.user.selected === null ? null : $scope.filter.user.selected.Id;
        var user = $scope.filter.user.selected === null ? "" : $scope.filter.user.selected.FullName;

        var json = JSON.stringify({ purchasesId: purchasesId, userId: userId });

        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/DistributionWork/SetAssignedToUser/",
            data: json
        }).then(function(response) {
                purchasesId.forEach(function(item, i, arr) {
                    var currentId = item;
                    $scope.purchase.Options.data.forEach(function(item, i, arr) {
                        if (item.Id === currentId) {
                            item.AssignedToUserId = userId;
                            item.AssignedToUser = user;
                        }
                    });
                });
                //$scope.filter.user.DictionaryData = response.data;
            },
            function() {
                $scope.message = "Unexpected Error";
            });
    }

    $scope.setPurchaseClass = function() {
        if ($scope.filter.purchaseClass === null || $scope.filter.purchaseClass.selected === null)
            return;

        if ($scope.purchase.selectedRows() === null || $scope.purchase.selectedRows().length === 0)
            return;

        var purchasesId = $scope.purchase.getSelected("Id");
        var purchaseClassId = $scope.filter.purchaseClass.selected.Id;
        var purchaseClass = $scope.filter.purchaseClass.selected.Name;

        var json = JSON.stringify({ purchasesId: purchasesId, purchaseClassId: purchaseClassId });

        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/DistributionWork/SetPurchaseClass/",
            data: json
        }).then(function(response) {
                purchasesId.forEach(function(item, i, arr) {
                    var currentId = item;
                    $scope.purchase.Options.data.forEach(function(item, i, arr) {
                        if (item.Id === currentId) {
                            item.PurchaseClassId = purchaseClassId;
                            item.PurchaseClass = purchaseClass;
                            item.PurchaseClassUser = response.data.UserFullName;
                        }
                    });
                });
                //$scope.filter.user.DictionaryData = response.data;
            },
            function() {
                $scope.message = "Unexpected Error";
            });
    }


    //Загрузка справочников

    loadPurchaseClass();
    loadUsers();

    function loadPurchaseClass() {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/DistributionWork/GetPurchaseClass/"
        }).then(function(response) {
                $scope.filter.purchaseClass.DictionaryData = response.data;
                $scope.filter.purchaseClass.selected = $scope.filter.purchaseClass.DictionaryData[0];
            },
            function() {
                $scope.message = "Unexpected Error";
            });
    }

    function loadUsers() {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/DistributionWork/GetUsers/"
        }).then(function(response) {
                $scope.filter.user.DictionaryData = response.data;
            },
            function() {
                $scope.message = "Unexpected Error";
            });
    }

    // Загрузка данных
    function loadPurchases(filter) {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/DistributionWork/GetPurchases/",
            data: filter.getJson()
        }).then(function(response) {
                $scope.purchase.Options.data = response.data;
            },
            function() {
                $scope.message = "Unexpected Error";
                messageBoxService.showError("Очень важная");
                //common.showError($scope.message);
            });

    }


    //Закупка

    $scope.purchase = uiGridCustomService.createGridClassMod($scope, 'DistributionWork_Grid');
    $scope.purchase.Options.showGridFooter = true;
    $scope.purchase.Options.noUnselect = true;
    $scope.purchase.Options.columnDefs =
    [
        { name: 'PurchaseId', field: 'Id', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Номер закупки', field: 'Number', filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?PurchaseNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Наименование объекта закупки', field: 'Name', enableHiding: false, filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата объявления', field: 'DateBegin', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата аукциона', field: 'DateEnd', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } },
        { name: 'Раздел', field: 'PurchaseClass', filter: { condition: uiGridCustomService.condition } },
        { name: 'Закупку осуществляет', field: 'WhoIsPurchasing', filter: { condition: uiGridCustomService.condition } },
        { name: 'Тип организации, осущ.зак.', field: 'OrganizationType', filter: { condition: uiGridCustomService.condition } },
        { name: 'ФИО ("Раздел")', field: 'PurchaseClassUser', filter: { condition: uiGridCustomService.condition } },
        { name: 'ФИО ("На обработке")', field: 'AssignedToUser', filter: { condition: uiGridCustomService.condition } },
        { name: 'ФИО ("в работе")', field: "UserInWork", filter: { condition: uiGridCustomService.condition } },
        { name: 'Сумма Лота', field: 'LotSum', type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Кол-во лотов', field: 'LotCount', type: 'number' },
        { name: 'Кол-во открытых лотов', field: 'LotCountOpen', type: 'number' },
        {
            name: '',
            field: 'URL',
            width: 16,
           
            enableSorting: false,
            enableFiltering: false,
            cellTemplate:
                '<div class="ngCellText" ng-class="col.colIndex()" style="overflow:  hidden"><a ng-href="{{COL_FIELD}}" target="_blank" class="glyphicon glyphicon-eye-open green"></a></div>',
            filter: { condition: uiGridCustomService.condition }
        },
        { name: 'Тип ФЗ', field: 'LawTypeName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Этап закупки', field: 'StageName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Способ опр постав', field: 'MethodName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Основание для заключения контракта', field: 'ConclusionReason', filter: { condition: uiGridCustomService.condition } },
        { name: 'Id орг осущ зак', field: 'CustomerId', type: 'number', filter: { condition: uiGridCustomService.condition } },
        { name: 'Наим орг осущ зак', field: 'CustomerShortName', filter: { condition: uiGridCustomService.condition } },
        { name: 'Регион', field: 'CustomerRegionFederationSubject', filter: { condition: uiGridCustomService.condition } },
        { name: 'Сложность', field: '', filter: { condition: uiGridCustomService.condition } },
        { name: 'Переход в "Обработка закупки"', field: '', filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата создания закупки', field: 'PurchaseDateCreate', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, filter: { condition: uiGridCustomService.condition } },
        { name: 'ФИО ("Последнее изменение")', field: 'LastChangedUser', filter: { condition: uiGridCustomService.condition } },
        { name: 'Дата последнего изменения Описания закупки', field: 'LastChangedDate', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, filter: { condition: uiGridCustomService.condition } },
        { name: 'Характер', field: 'ExistsNature', type:"boolean"},
        { name: 'Период поставки', field: 'ExistsDeliveryTimeInfo', type:"boolean"},
        { name: 'Финансирование', field: 'ExistsLotFunding', type:"boolean"},
        { name: 'Тех.задание', field: 'ExistsPurchaseObject', type: "boolean" },
        { name: 'Протокол', field: 'ToProtokol', type: "boolean" },
        { name: 'П.На проверку', field: 'ForCheck', type: "boolean" },
        { name: 'Сайт источник', field: 'SiteURL', filter: { condition: uiGridCustomService.condition }, cellTemplate: formatConstants.cellTemplateURL }
    ];
    $scope.purchase.SetDefaults();
    //Фильтр
    var filterClass = function (loadFunction) {

        this.DateBegin_Start = new dateClass();
        this.DateBegin_End = new dateClass();
        this.DateEnd_Start = new dateClass();
        this.DateEnd_End = new dateClass();
        this.PurchaseDateCreate_Start = new dateClass();
        this.PurchaseDateCreate_End = new dateClass();
        this.purchaseClass = new dictClass();
        this.isAssignedToUser = false;
        this.user = new dictTypeHead();
        this.isCheckTZ = false;
        this.withPtotokol = true;
        this.clear = function () {
            this.DateBegin_Start.setNull();
            this.DateBegin_End.setNull();
            this.DateEnd_Start.setNull();
            this.DateEnd_End.setNull();
            this.PurchaseDateCreate_Start.setNull();
            this.PurchaseDateCreate_End.setNull();
            this.purchaseClass.selected = this.purchaseClass.DictionaryData[0];
            this.user.clear();
        };

        this.search = function () {
            if (!this.validate()) {
                return;
            }

            loadFunction(this);
        };

        this.validate = function () {
            return true;
        };

        this.getJson = function () {
            var filterPurchases = {
                DateBegin_Start: this.getFormatedDateString(this.DateBegin_Start.Value),
                DateBegin_End: this.getFormatedDateString(this.DateBegin_End.Value),
                DateEnd_Start: this.getFormatedDateString(this.DateEnd_Start.Value),
                DateEnd_End: this.getFormatedDateString(this.DateEnd_End.Value),
                PurchaseDateCreate_Start: this.getFormatedDateString(this.PurchaseDateCreate_Start.Value),
                PurchaseDateCreate_End: this.getFormatedDateString(this.PurchaseDateCreate_End.Value),
                PurchaseClassId: this.purchaseClass.selected != null ? this.purchaseClass.selected.Id : null,
                IsAssignedToUser: this.isAssignedToUser,
                AssignedToUserId: this.isAssignedToUser && this.user.selected != null ? this.user.selected.Id : null, withPtotokol: this.withPtotokol
            };
            return JSON.stringify({ filterPurchases: filterPurchases });
        };

        this.getFormatedDateString = function (date) {
            if (date === null) {
                return null;
            }

            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var day = date.getDate();
            return year + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;
        };
    };

    //Объект фильтр

    $scope.filter = new filterClass(loadPurchases);

    $(window).keydown(function (event) {
        if (event.keyCode === 16) {
            SetIsShift(true);
        }
    });

    $(window).keyup(function (event) {
        if (event.keyCode === 16) {
            SetIsShift(false);
        }
    });

    function SetIsShift(value) {
        if (value)
            $('#grid-block').addClass('disableSelection');
        else {
            $('#grid-block').removeClass('disableSelection');
        }
    };
}