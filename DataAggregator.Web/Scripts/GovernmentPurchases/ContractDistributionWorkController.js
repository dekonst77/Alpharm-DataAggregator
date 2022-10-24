angular
    .module('DataAggregatorModule')
    .controller('ContractDistributionWorkController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'formatConstants', ContractDistributionWorkController]);

function ContractDistributionWorkController(messageBoxService, $scope, $http, uiGridCustomService, formatConstants) {


    $(window).keydown(function(event) {
        if (event.keyCode == 16) {
            SetIsShift(true);
        }

    });

    $(window).keyup(function(event) {
        if (event.keyCode == 16) {
            SetIsShift(false);
        }
    });


    function SetIsShift(value) {
        $scope.IsShift = value;
        if (value)
            $('#content').addClass('disableSelection');
        else {
            $('#content').removeClass('disableSelection');
        }
    };

    //Методы
    $scope.setAssignedToUser = function() {
        var purchasesId = $scope.purchase.getSelected();
        var userId = $scope.filter.user.selected == null ? null : $scope.filter.user.selected.Id;
        var user = $scope.filter.user.selected == null ? "" : $scope.filter.user.selected.FullName;

        var json = JSON.stringify({ purchasesId: purchasesId, userId: userId });

        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/ContractDistributionWork/SetAssignedContractToUser/",
            data: json
        }).then(function(response) {
                purchasesId.forEach(function(item, i, arr) {
                    var currentId = item;
                    $scope.purchase.Options.data.forEach(function(item, i, arr) {
                        if (item.Id == currentId) {
                            item.ContractAssignedToUserId = userId;
                            item.ContractAssignedToUser = user;
                        }
                    });
                });
                //$scope.filter.user.DictionaryData = response.data;
            },
            function() {
                $scope.message = "Unexpected Error";
            });
    }

    $scope.removeKK = function() {
        var purchasesId = $scope.purchase.getSelected();

        if ((purchasesId !== null) && (purchasesId.length > 0) && confirm('Обнулить КК?')) {
            var json = JSON.stringify({ purchasesId: purchasesId });

            $scope.loadingDictionary = $http({
                method: "POST",
                url: "/ContractDistributionWork/RemoveKK/",
                data: json
            }).then(function(response) {
                    purchasesId.forEach(function(item, i, arr) {
                        var currentId = item;
                        $scope.purchase.Options.data.forEach(function(item, i, arr) {
                            if (item.Id === currentId) {
                                item.ContractKKCount = 0;
                            }
                        });
                    });
                },
                function() {
                    $scope.message = "Unexpected Error";
                });
        }
    }

    //Загрузка справочников

    loadPurchaseClass();
    loadUsers();

    function loadPurchaseClass() {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/ContractDistributionWork/GetPurchaseClass/"
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
            url: "/ContractDistributionWork/GetUsers/"
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
            url: "/ContractDistributionWork/GetPurchases/",
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
    $scope.purchase = uiGridCustomService.createGridClass($scope, 'ContractDistributionWork_Grid');
    $scope.purchase.Options.showGridFooter = true;
    $scope.purchase.Options.noUnselect = true;
    $scope.purchase.Options.columnDefs =
    [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Номер закупки', field: 'Number', filter: { condition: uiGridCustomService.conditionSpace }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/GovernmentPurchases/GovernmentPurchases?PurchaseNumber={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Наименование объекта закупки', field: 'Name', enableHiding: false },
        { name: 'Дата объявления', field: 'DateBegin', type: 'date', cellFilter: formatConstants.FILTER_DATE },
        { name: 'Дата аукциона', field: 'DateEnd', type: 'date', cellFilter: formatConstants.FILTER_DATE },
        { name: 'Раздел', field: 'PurchaseClass', filter: { condition: uiGridCustomService.condition } },
        { name: 'ФИО ("Раздел")', field: 'PurchaseClassUser', filter: { condition: uiGridCustomService.condition } },
        { name: 'ФИО ("На обработке")', field: 'ContractAssignedToUser', filter: { condition: uiGridCustomService.condition } },
        { name: 'ФИО ("в работе")', field: "UserInWork", filter: { condition: uiGridCustomService.condition } },
        { name: 'Сумма контракта', field: 'Sum', type: 'number', cellFilter: formatConstants.FILTER_PRICE, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Кол-во необр. контрактов', field: 'ContractCount', type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Кол-во КК контрактов', field: 'ContractKKCount', type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'Кол-во PDF контрактов', field: 'ContractPDFCount', type: 'number', cellFilter: formatConstants.FILTER_INT_COUNT, filter: { condition: uiGridCustomService.numberCondition } },
        {
            name: '',
            field: 'URL',
            width: 16,
           
            enableSorting: false,
            enableFiltering: false,
            cellTemplate:
                '<div class="ngCellText" ng-class="col.colIndex()" style="overflow:  hidden"><a ng-href="{{COL_FIELD}}" target="_blank" class="glyphicon glyphicon-eye-open green"></a></div>'
        },
        { name: 'Характер', field: "NatureName" },
        { name: 'Победитель', field: "Supplier_Winner" }
        //{ name: 'Сложность', field: '' },
        //{ name: 'Переход в "Обработка закупки"', field: '' }
    ];

    //Фильтр
    var filterClass = function(loadFunction) {

        this.DateBegin_Start = new dateClass();
        this.DateBegin_End = new dateClass();
        this.purchaseClass = new dictClass();
        this.isAssignedToUser = false;
        this.isCheckTZ = false;
        this.user = new dictTypeHead();

        this.clear = function() {
            this.DateBegin_Start.setNull();
            this.DateBegin_End.setNull();
            this.purchaseClass.selected = this.purchaseClass.DictionaryData[0];
            this.user.clear();
        }

        this.search = function() {
            if (!this.validate()) {
                return;
            }

            loadFunction(this);
        }

        this.validate = function() {
            return true;
        }

        this.getJson = function() {
            var filterPurchases = {
                DateBegin_Start: this.DateBegin_Start.Value,
                DateBegin_End: this.DateBegin_End.Value,
                PurchaseClassId: this.purchaseClass.selected != null ? this.purchaseClass.selected.Id : null,
                IsAssignedToUser: this.isAssignedToUser,
                AssignedToUserId: this.isAssignedToUser && this.user.selected != null ? this.user.selected.Id : null,
                isCheckTZ: this.isCheckTZ
            }

            return JSON.stringify({ filterPurchases: filterPurchases });
        }
    }


    //Объект фильтр

    $scope.filter = new filterClass(loadPurchases);

}



