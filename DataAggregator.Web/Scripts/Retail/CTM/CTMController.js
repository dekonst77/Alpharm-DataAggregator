angular
    .module('DataAggregatorModule')
    .controller('CTMController', ['$scope', '$http', '$q', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'messageBoxService', 'uiGridConstants', 'formatConstants', CTMController]);

function CTMController($scope, $http, $q, $timeout, uiGridCustomService, errorHandlerService, messageBoxService, uiGridConstants, formatConstants) {
    $scope.Title = "СТМ";

    $scope.periods = [];
    $scope.currentperiod = null;

    $scope.Networks = [];
    $scope.NetworksLabel = [];

    var selectedRows = null;
    $scope.selectedCount = null;

    //******** Grid ******** ->

    $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'CTM_Grid');
    $scope.Grid.Options.showGridFooter = true;
    $scope.Grid.Options.multiSelect = true;
    $scope.Grid.Options.enableSelectAll = true;
    $scope.Grid.Options.modifierKeysToMultiSelect = true;
    $scope.Grid.Options.columnDefs = [
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Id', field: 'Id', type: 'number', visible: false, nullable: true, filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 130, name: 'Classifier Id', field: 'ClassifierID', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'TradeName Id', enableHiding: true, visible: false, field: 'TradeNameId', type: 'number', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 170, name: 'Наименование ТН', field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 150, name: 'Описание ТН', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'OwnerTradeMark Id', field: 'OwnerTradeMarkId', type: 'number', visible: false, filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 170, name: 'Правообладатель', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Id сети', field: 'NetworkID', visible: false, nullable: true },

        {
            headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 300, name: 'Наименование сети',
            field: 'NetworkName',
            headerCellClass: 'Edit',
            cellTooltip: 'Наименование аптечной сети',
            filters: [
                {
                    condition: uiGridConstants.filter.STARTS_WITH,
                    type: uiGridConstants.filter.INPUT,
                    selectOptions: $scope.Networks /* формат [ { value: 1, label: 'male' }, { value: 2, label: 'female' } ] */
                }],
            editableCellTemplate: 'ui-grid/dropdownEditor',
            editDropdownOptionsArray: $scope.NetworksLabel,
            editDropdownIdLabel: 'value', editDropdownValueLabel: 'NetworksLabel'
        },

        {
            headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'DeBrikingState', displayName: 'Разбриковка', field: 'DeBrikingState', type: 'boolean', visible: false, nullable: true
        },
        {
            headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'DeBrikingStateTemplate', displayName: '1/0', field: 'DeBrikingState', type: 'boolean', visible: true, nullable: true,
            cellTemplate:
                '<div class="btn-group">' +
                '<button type="button" disabled class="btn-sm" ng-class="{ \'btn-info\' : row.entity.DeBrikingState==null}">X</button >' +
                '<button type="button" disabled class="btn-sm" ng-class="{ \'btn-danger\' : row.entity.DeBrikingState==0}">0</button >' +
                '<button type="button" disabled class="btn-sm" ng-class="{ \'btn-success\' : row.entity.DeBrikingState==1}">1</button >' +
                '</div>',
            filter: { condition: uiGridCustomService.booleanConditionX }
        },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Тип данных', field: 'DeBrikingStateName', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Период', field: 'Period', visible: false, nullable: true, filter: { condition: uiGridCustomService.numberCondition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: true, width: 300, name: 'Комментарий', field: 'Comment', filter: { condition: uiGridCustomService.condition } }
    ];
    $scope.Grid.SetDefaults();

    $scope.Grid.Options.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;

        // Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            selectedRows = $scope.gridApi.selection.getSelectedRows();
            $scope.selectedCount = selectedRows.length;
        });

        // Что-то выделили
        gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            selectedRows = $scope.gridApi.selection.getSelectedRows();
            $scope.selectedCount = selectedRows.length;
        });

        gridApi.edit.on.afterCellEdit($scope, editRowDataSource);
    };

    // редактируемые поля: NetworkName, Comment
    function editRowDataSource(rowEntity, colDef, newValue, oldValue) {
        const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
        const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];
        const day = 15;
        const period = new Date(Date.UTC(year, month - 1, day));

        console.log('editRowDataSource() -> year = ' + year);
        console.log('editRowDataSource() -> month = ' + month);
        console.log('editRowDataSource() -> period = ' + period);

        // проверка на изменение
        if (newValue === oldValue || newValue === undefined)
            return;

        $scope.dataLoading = $http({
            method: "POST",
            url: "/CTM/Edit/",
            data: { period: period, record: rowEntity, fieldname: colDef.field }
        }).then(function (response) {

            var data = response.data.Data;

            if (data.Success) {
                let CTMRecord = data.Data.CTMRecord[0];
                rowEntity['NetworkID'] = CTMRecord.NetworkID;
                rowEntity['DeBrikingState'] = CTMRecord.DeBrikingState;
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }
            return true;
        }, function (response) {
            rowEntity[colDef.field] = oldValue;
            errorHandlerService.showResponseError(response);
            return false;
        });

        return;
    }

    //******** Grid ******** <-

    // загрузка периодов
    function GS_periods() {
        var deferred = $q.defer();

        $scope.dataLoading = $http({
            method: 'GET',
            url: '/CTM/GS_periodsAsync/',
            data: JSON.stringify({})
        }).then(function (response) {
            console.log('GS_periods() -> успешно');

            var periods = response.data;

            Array.prototype.push.apply($scope.periods, periods);
            $scope.currentperiod = periods[0];

            console.log('GS_periods -> $scope.periods = ' + $scope.periods);
            console.log('GS_periods -> $scope.currentperiod = ' + $scope.currentperiod);

            deferred.resolve();
        }, function (response) {
            console.log('GS_periods() -> error');

            errorHandlerService.showResponseError(response);

            deferred.reject(response.status);
        });

        return deferred.promise;
    };

    // загрузка аптечных сетей
    function NetworksLoading() {
        const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
        const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

        console.log('NetworksLoading() -> year = ' + year);
        console.log('NetworksLoading() -> month = ' + month);

        var deferred = $q.defer();

        $scope.dataLoading = $http({
            method: "POST",
            url: "/CTM/GetAllNetworks/",
            data: JSON.stringify({ year: year, month: month })
        }).then(function (response) {
            console.log('$scope.NetworksLoading -> успешно');

            $scope.Networks.length = 0;
            $scope.NetworksLabel.length = 0;

            Array.prototype.push.apply($scope.NetworksLabel, response.data.map(function (obj) {
                var rObj = { 'value': obj.NetworkName, 'NetworksLabel': obj.NetworkName == null ? '-' : obj.NetworkName };
                return rObj;
            }));

            deferred.resolve();
        }, function (response) {
            console.log('$scope.NetworksLoading -> error');

            var data = response.data;
            messageBoxService.showError('Произошла ошибка при загрузке списка аптечных сетей.<br/>' + data.message);

            deferred.reject(response.status);
        });

        return deferred.promise;
    };

    $scope.Search = function () {
        const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
        const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

        console.log('$scope.Search -> year = ' + year);
        console.log('$scope.Search -> month = ' + month);

        var promiseObj = NetworksLoading();

        promiseObj.then(function (value) {

            console.log('$scope.Search -> $scope.NetworksLoading() -> success');

            $scope.dataLoading = $http({
                method: 'POST',
                url: '/CTM/LoadCTM/',
                data: JSON.stringify({ year: year, month: month })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.Options.data = data.Data.CTM;
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
        }, function (response) {
            console.log('$scope.Search -> $scope.NetworksLoading() -> error');

            $scope.Grid.Options.data = [];
        });
    }

    $scope.Init = function () {
        var promiseObj = GS_periods();

        promiseObj.then(function (value) {
            var reggie = /(\d{4})-(\d{2})/;
            var dateArray = reggie.exec($scope.currentperiod);
            var year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
            var month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];

            console.log('$scope.Init -> $scope.periods = ' + $scope.periods);
            console.log('$scope.Init -> $scope.currentperiod = ' + $scope.currentperiod);
            console.log('$scope.Init -> dateArray = ' + dateArray);
            console.log('$scope.Init -> year = ' + year);
            console.log('$scope.Init -> month = ' + month);

            $scope.Search();
        });
    }

    $scope.Network_FromExcel = function (files) {
        if (files && files.length) {
            var formData = new FormData();
            files.forEach(function (item, i, arr) {
                formData.append('uploads', item);
            });
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/CTM/Network_FromExcelAsync/',
                data: formData,
                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            }).then(function (response) {
                $scope.Search();
            }, function (response) {
                console.log(response);
                var responseData = response.data;

                $scope.Grid.Options.data = [];
                messageBoxService.showError(JSON.stringify(responseData.message));
            });
        }
    };

    $scope.ClearComments = function () {
        console.log(selectedRows);

        if (selectedRows === null) {
            return;
        }

        selectedRows.forEach(function (item, i, arr) {
            //console.log(item);
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/CTM/ClearComment',
                data: { ID: item.Id }
            }).then(function () {
                $scope.Search();
            }, function (response) {
                console.log(response);
                var responseData = response.data;

                $scope.Grid.Options.data = [];

                messageBoxService.showError('Ошибка! Не удалось сохранить.');
            });
        });

    };

}
