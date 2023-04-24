angular
    .module('DataAggregatorModule')
    .controller('GoodsClassifierReportController', ['$scope', '$http', '$q', '$uibModal', '$cacheFactory', '$timeout', 'userService', 'uiGridCustomService', 'errorHandlerService', 'messageBoxService', 'uiGridConstants', 'formatConstants', GoodsClassifierReportController]);

function GoodsClassifierReportController($scope, $http, $q, $uibModal, $cacheFactory, $timeout, userService, uiGridCustomService, errorHandlerService, messageBoxService, uiGridConstants, formatConstants) {
    $scope.Title = "Отчёт по классификатору доп. ассортимента";
    $scope.user = userService.getUser();
    var PermanentColumns = [];

    console.log('Отчет по классификатору доп. ассортимента');

    // ================================== Категории ==================================
    $scope.goodsCategory = null;
    $scope.ClassifierId = null;

    var getGoodsCategoryList = function () {
        $scope.goodsCategory = null;
        $scope.loading = $http.post("/DOPMonitoringDatabase/GetGoodsCategoryList/")
            .then(function (response) {
                $scope.goodsCategoryList = response.data.Data;
            }, function () {
                messageBoxService.showError("Не удалось загрузить список категорий!");
            });
    };
    getGoodsCategoryList();
    // ================================== Категории ==================================

    // ============>
    // Блок панелей
    $scope.PanelCategoryListIsShow = true;

    $scope.PanelCategoryListToogle = function () {
        $scope.PanelCategoryListIsShow = !$scope.PanelCategoryListIsShow;
        setTimeout(function () { if (!$scope.$$phase) $scope.$apply(); });
    }
    // Блок панелей
    // ============<

    // ================= Инициализация таблиц =================->
    $scope.selectedRows = [];

    $scope.GoodsClassifierReport_Init = function () {

        $scope.message = 'Пожалуйста, ожидайте... Загрузка';

        $scope.GoodsClassifierReport = uiGridCustomService.createGridClassMod($scope, 'GoodsClassifierReport');

        PermanentColumns =
            [
                { headerTooltip: true, name: 'GoodsCategoryId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsCategoryId', type: 'number', visible: false, nullable: false },
                { headerTooltip: true, name: 'GoodsCategoryName', displayName: 'Категория', enableCellEdit: false, width: 200, cellTooltip: true, field: 'GoodsCategoryName', visible: false },

                { headerTooltip: true, name: 'ClassifierId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'ClassifierId', type: 'number', visible: true, nullable: false, filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },

                { headerTooltip: true, name: 'GoodsId', displayName: 'Код препарата', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsId', type: 'number', visible: true, nullable: false },
                { headerTooltip: true, name: 'GoodsTradeNameId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'GoodsTradeNameId', type: 'number', visible: false, nullable: false },
                { headerTooltip: true, name: 'GoodsTradeName', displayName: 'Торговое наименование', enableCellEdit: false, width: 300, cellTooltip: true, field: 'GoodsTradeName', visible: true },
                { headerTooltip: true, name: 'GoodsDescription', displayName: 'ФВ + Ф + Д', enableCellEdit: false, width: 300, cellTooltip: true, field: 'GoodsDescription', visible: true },

                { headerTooltip: true, name: 'OwnerTradeMarkId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'OwnerTradeMarkId', type: 'number', visible: false, nullable: false },
                { headerTooltip: true, name: 'OwnerTradeMark', displayName: 'Правообладатель', enableCellEdit: false, width: 200, cellTooltip: true, field: 'OwnerTradeMark', visible: true },

                { headerTooltip: true, name: 'PackerId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'PackerId', type: 'number', visible: false, nullable: false },
                { headerTooltip: true, name: 'Packer', displayName: 'Упаковщик', enableCellEdit: false, width: 200, cellTooltip: true, field: 'Packer', visible: true },

                { headerTooltip: true, name: 'BrandId', enableCellEdit: false, width: 100, cellTooltip: true, field: 'BrandId', type: 'number', visible: false, nullable: false },
                { headerTooltip: true, name: 'Brand', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Brand', visible: true }
            ];
        $scope.GoodsClassifierReport.Options.columnDefs = PermanentColumns;

        $scope.GoodsClassifierReport.SetDefaults();

        $scope.GoodsClassifierReport.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;

            // Что-то выделили
            gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                $scope.selectedRows = $scope.gridApi.selection.getSelectedRows();
            });

            // Что-то выделили
            gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
                $scope.selectedRows = $scope.gridApi.selection.getSelectedRows();
            });
        };

    }
    // ================= Инициализация таблиц =================<-

    $scope.GoodsClassifierReport_Refresh = function () {
        $scope.message = 'Пожалуйста, ожидайте... Загрузка';

        if ($scope.goodsCategory === null) {
            messageBoxService.showError("Не выбрана категория!");
            return;
        }

        var columnsAddTemp = $scope.getAddColumnGrid();

        columnsAddTemp.then((columns) => {
            console.log(columns);

            $scope.GoodsClassifierReport.Options.columnDefs = PermanentColumns;

            if (columns.length > 0) {
                let columnsAdd = [];
                Array.prototype.push.apply(columnsAdd, columns.map(function (obj) {
                    return { field: obj.field, headerCellClass: 'yellowColor' };
                }));
                $scope.GoodsClassifierReport.Options.columnDefs = PermanentColumns.concat(columnsAdd);
            }

            $scope.dataLoading = $http({
                method: 'POST',
                url: '/GoodsClassifierReport/Init/',
                data: JSON.stringify({ GoodsCategoryId: $scope.goodsCategory.Id })
            }).then(function (response) {
                var data = response.data;

                if (data.Success) {
                    $scope.GoodsClassifierReport.SetData(data.Data.GoodsClassifierReport);
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }

            }, function (response) {
                console.error(response.data.message);
                messageBoxService.showError(response.data.message);
            }).catch(
                error => {
                    console.error(error);
                    alert(error)
                }
            )
        });
    }

    // возвращает массив доп. полей для выбранной категории <$scope.goodsCategory>
    // test: $scope.getAddColumnGrid = [{ field: 'company', enableSorting: false }, { field: 'Gender', enableSorting: false }];
    $scope.getAddColumnGrid = function () {
        return $http({
            method: 'POST',
            url: '/GoodsClassifierReport/getAddColumnGrid/',
            data: JSON.stringify({ GoodsCategoryId: $scope.goodsCategory !== null ? $scope.goodsCategory.Id : 0 })
        }).then(function (response) {
            console.log(response);

            var data = response.data;
            if (data.Success)
                return response.data.Data.columns
            else
                return [];
        }, function () {
            return [];
        })
    }

}
