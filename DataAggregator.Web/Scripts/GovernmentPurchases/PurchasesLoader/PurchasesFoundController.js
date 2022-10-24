angular
   .module('DataAggregatorModule')
   .controller('PurchasesFoundController', ['$scope', '$http', 'uiGridCustomService', PurchasesFoundController]);

function PurchasesFoundController($scope, $http, uiGridCustomService) {


    //Грид
    $scope.purchasesGrid = uiGridCustomService.createGridClass($scope, 'PurchasesLoader_PurchasesFound_Grid');
    $scope.purchasesGrid.Options.showGridFooter = true;
    $scope.purchasesGrid.Options.enableSorting = true;
    $scope.purchasesGrid.Options.multiSelect = true;
    $scope.purchasesGrid.Options.modifierKeysToMultiSelect = true;
    $scope.purchasesGrid.Options.noUnselect = true;
    $scope.purchasesGrid.Options.columnDefs =
    [
        { displayName: 'COMMON_GRID.TYPE_FL', field: 'FzNumber', filter: { condition: uiGridCustomService.condition } },
        { name: 'Номер Закупки', field: 'Number', type: 'number', filter: { condition: uiGridCustomService.conditionSpace } },
        { displayName: 'COMMON_GRID.PURCHASE_NAME', field: 'Name', filter: { condition: uiGridCustomService.condition } },
        {
            displayName: '', field: 'Url', width: 16, enableSorting: false, enableFiltering: false,
            cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()" style="overflow:  hidden"><a ng-href="{{COL_FIELD}}" target="_blank" class="glyphicon glyphicon-eye-open green"></a></div>'
        },
        { displayName: 'PURCHASE_FOUND.CUSTOMER', field: 'Customer', filter: { condition: uiGridCustomService.condition } },

        { displayName: 'PURCHASE_FOUND.METHOD', field: 'Method', filter: { condition: uiGridCustomService.condition } },
        { displayName: 'PURCHASE_FOUND.STAGE', field: 'Stage', filter: { condition: uiGridCustomService.condition } },
        { displayName: 'PURCHASE_FOUND.SUM', field: 'Sum', filter: { condition: uiGridCustomService.condition } },
        { displayName: 'PURCHASE_FOUND.PUBLISH_DATE', field: 'PublishDate', filter: { condition: uiGridCustomService.condition }, cellFilter: 'date:\'yyyy-MM-dd HH:mm\'' },
        { displayName: 'PURCHASE_FOUND.UPDATE_DATE', field: 'UpdateDate', filter: { condition: uiGridCustomService.condition }, cellFilter: 'date:\'yyyy-MM-dd HH:mm\'' },


        { displayName: 'COMMON_GRID.PURCHASE_CLASS', field: 'PurchaseClass', filter: { condition: uiGridCustomService.condition } },
        { displayName: 'COMMON_GRID.FIO_CLASS', field: 'PurchaseClassUser', filter: { condition: uiGridCustomService.condition } },
        { displayName: 'COMMON_GRID.DATE_CLASS', field: 'PurchaseClassDate', filter: { condition: uiGridCustomService.condition }, cellFilter: 'date:\'yyyy-MM-dd HH:mm\'' },
        { displayName: 'PURCHASE_FOUND.SEARCH_DATE', field: 'SearchDate', filter: { condition: uiGridCustomService.condition }, cellFilter: 'date:\'yyyy-MM-dd HH:mm\'' },
        { displayName: 'PURCHASE_FOUND.PURCHASE_STAGE', field: 'PurchaseStage', filter: { condition: uiGridCustomService.condition }, cellFilter: 'date:\'yyyy-MM-dd HH:mm\'' },
        { displayName: 'PURCHASE_FOUND.ERROR_MESSAGE', field: 'ErrorMessage', filter: { condition: uiGridCustomService.condition }, cellFilter: 'date:\'yyyy-MM-dd HH:mm\'' },
    ];

    //События изменения грида
    $scope.purchasesGrid.Options.onRegisterApi = function (gridApi) {
        $scope.purchasesGridApi = gridApi;

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.filter.selectedRows = $scope.purchasesGridApi.selection.getSelectedRows();
        });

        //Что-то выделили
        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.filter.selectedRows = $scope.purchasesGridApi.selection.getSelectedRows();
        });
    };

    //Класс Фильтр
    var filterClass = function (loadData) {

        this.canSetPurchaseClass = canSetPurchaseClass,
        this.selectedRows = null,
            this.DateBegin_Start = new dateClass();
        this.DateBegin_End = new dateClass();
        this.purchaseClass = new dictClass();
        this.isAssignedToUser = false;
        this.isCheckTZ = false;
        this.user = new dictTypeHead();

        this.clear = function () {
            this.DateBegin_Start.setNull();
            this.DateBegin_End.setNull();
            this.purchaseClass.selected = this.purchaseClass.DictionaryData[0];
            this.user.clear();
        }

        this.search = function () {

            loadData(this);
        }

        this.getJson = function () {
            var filterPurchases = {
                DateBegin_Start: this.DateBegin_Start.Value,
                DateBegin_End: this.DateBegin_End.Value,
                PurchaseClassId: this.purchaseClass.selected != null ? this.purchaseClass.selected.Id : null,
                PurchaseClassUserId: this.user.selected != null ? this.user.selected.Id : null
            }

            return JSON.stringify({ filterPurchases: filterPurchases });
        }

        function canSetPurchaseClass() {
            return (this.selectedRows != null && this.selectedRows.length > 0) &&
                this.purchaseClass.selected != null;
        }

    }

    //Объект фильтр

    $scope.filter = new filterClass(loadData);

    //Методы

    //Загрузка 
    function loadClass() {
        $scope.loading = $http({
            method: "POST",
            url: "/PurchasesFound/GetPurchaseClasses"
        }).then(function (response) {
            //$scope.dictionary.PurchaseClasses = response.data;
            $scope.filter.purchaseClass.DictionaryData = response.data;
            $scope.filter.purchaseClass.selected = $scope.filter.purchaseClass.DictionaryData[0];
        }, function () {
            $scope.dictionary.PurchaseClasses = [];
        });
    }

    //Загрузка пользователей
    function loadUsers() {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/PurchasesFound/GetUsers/"
        }).then(function (response) {
            $scope.filter.user.DictionaryData = response.data;
        },
            function () {
                $scope.message = "Unexpected Error";
            });
    }

    //Загрузка данных по фильтру
    function loadData() {
        $scope.loading = $http({
            method: "POST",
            url: "/PurchasesFound/GetData/",
            data: $scope.filter.getJson()
        }).then(function (response) {
            $scope.purchasesGrid.Options.data = response.data;
        }, function () {
            $scope.purchasesGrid.Options.data = [];
            alert('Ошибка');
        });
    }

    //Установить класс
    $scope.setPurchaseClass = function () {
        var ids = $scope.filter.selectedRows.map(function (r) { return r.Id });
        var datajson = JSON.stringify({ ids: ids, purchaseClassId: $scope.filter.purchaseClass.selected.Id });

        $scope.loading = $http({
            method: "POST",
            data: datajson,
            url: "/PurchasesFound/SetPurchaseClass/"
        }).then(function (response) {
            //Меняем показатели
            $scope.filter.selectedRows.forEach(function (item, i, arr) {
                var currentId = item.Id;
                $scope.purchasesGrid.Options.data.forEach(function (item, i, arr) {
                    if (item.Id == currentId) {
                        item.PurchaseClass = $scope.filter.purchaseClass.selected.Name;
                        item.PurchaseClassUser = response.data.UserFullName;
                        item.PurchaseClassDate = response.data.DateUpdate;
                    }
                });
            });
        }, function () {
        });
    }

    //Инициализация

    loadClass();
    loadUsers();

}