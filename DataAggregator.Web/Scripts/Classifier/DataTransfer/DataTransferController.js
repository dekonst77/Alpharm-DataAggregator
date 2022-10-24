angular
    .module('DataAggregatorModule')
    .controller('DataTransferController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', DataTransferController]);

function DataTransferController($scope, $http, $uibModal, uiGridCustomService, formatConstants) {
    $scope.leftClassifierGrid = uiGridCustomService.createGridClass($scope, 'DataTransfer_LeftClassifierGrid');
    $scope.rightClassifierGrid = uiGridCustomService.createGridClass($scope, 'DataTransfer_RightClassifierGrid');
    $scope.transferedDataGrid = uiGridCustomService.createGridClass($scope, 'DataTransfer_TransferedDataGrid');

    $scope.leftClassifierGrid.Options.multiSelect = true;
    $scope.leftClassifierGrid.Options.noUnselect = false;
    $scope.leftClassifierGrid.Options.showGridFooter = false;
    $scope.leftClassifierGrid.Options.onRegisterApi = function (gridApi) {
        $scope.leftClassifierGridApi = gridApi;
    };

    $scope.rightClassifierGrid.Options.multiSelect = false;
    $scope.rightClassifierGrid.Options.noUnselect = false;
    $scope.rightClassifierGrid.Options.showGridFooter = false;
    $scope.rightClassifierGrid.Options.onRegisterApi = function (gridApi) {
        $scope.rightClassifierGridApi = gridApi;
    };

    $scope.transferedDataGrid.Options.multiSelect = true;
    $scope.transferedDataGrid.Options.noUnselect = false;
    $scope.transferedDataGrid.Options.showGridFooter = false;
    $scope.transferedDataGrid.Options.onRegisterApi = function (gridApi) {
        $scope.transferedDataGridApi = gridApi;
    };

    $scope.leftClassifierGrid.Options.columnDefs = [
        { name: 'DrugId', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'ТН', field: 'TradeName' },
        { name: 'ФВ, Ф, Д', field: 'DrugDescription' },
        { name: 'PackerId', field: 'PackerId', type: 'number' },
        { name: 'Упаковщик', field: 'Packer' },
        { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number' },
        { name: 'Производитель', field: 'OwnerTradeMark' }
    ];

    $scope.rightClassifierGrid.Options.columnDefs = [
        { name: 'DrugId', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'ТН', field: 'TradeName' },
        { name: 'ФВ, Ф, Д', field: 'DrugDescription' },
        { name: 'PackerId', field: 'PackerId', type: 'number' },
        { name: 'Упаковщик', field: 'Packer' },
        { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number' },
        { name: 'Производитель', field: 'OwnerTradeMark' }
    ];

    $scope.transferedDataGrid.Options.columnDefs = [
        { name: 'DrugId с', field: 'DrugIdFrom' },
        { name: 'ТН с', field: 'TradeNameFrom' },
        { name: 'ФВ, Ф, Д с', field: 'DrugDescriptionFrom' },
        { name: 'PackerId с', field: 'PackerIdFrom', type: 'number' },
        { name: 'Упаковщик с', field: 'PackerFrom' },
        { name: 'OwnerTradeMarkId с', field: 'OwnerTradeMarkIdFrom', type: 'number' },
        { name: 'Производитель с', field: 'OwnerTradeMarkFrom' },
        { name: 'DrugId на', field: 'DrugIdTo', type: 'number' },
        { name: 'ТН на', field: 'TradeNameTo' },
        { name: 'ФВ, Ф, Д на', field: 'DrugDescriptionTo' },
        { name: 'PackerId на', field: 'PackerIdTo', type: 'number' },
        { name: 'Упаковщик на', field: 'PackerTo' },
        { name: 'OwnerTradeMarkId на', field: 'OwnerTradeMarkIdTo', type: 'number' },
        { name: 'Производитель на', field: 'OwnerTradeMarkTo' },
        { name: 'Дата с', field: 'DateFrom', type: 'date', cellFilter: formatConstants.FILTER_DATE },
        { name: 'Дата по', field: 'DateTo', type: 'date', cellFilter: formatConstants.FILTER_DATE }
    ];

    var nowDate = new Date();
    var previousMonthDate = new Date(nowDate.getFullYear(), nowDate.getMonth() - 1, 1);

    $scope.dateFromUnlimited = false;
    $scope.dateToUnlimited = false;

    $scope.transferOptions = {
        TransferDrugId: true,
        TransferPackerId: true,
        TransferOwnerTradeMarkId: true,
        DateFrom: previousMonthDate,
        DateTo: previousMonthDate
    };

    $scope.leftClassifierFilter = {
        DrugId: null,
        TradeName: null,
        PackerId: null,
        Packer: null,
        OwnerTradeMarkId: null,
        OwnerTradeMark: null
    };

    $scope.rightClassifierFilter = {
        DrugId: null,
        TradeName: null,
        PackerId: null,
        Packer: null,
        OwnerTradeMarkId: null,
        OwnerTradeMark: null
    };

    $scope.openRightClassifierFilter = function () {
        var rightClassifierFilterDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/DataTransfer/_RightClassifierFilter.html',
            controller: 'RightClassifierFilterController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogData: $scope.rightClassifierFilter
            }
        });

        rightClassifierFilterDialog.result.then(
            // ok
            function (data) {
                $scope.rightClassifierFilter = data;

                $scope.loading = $http({
                    method: 'POST',
                    url: '/DataTransfer/GetRightClassifier',
                    data: JSON.stringify({ filter: $scope.rightClassifierFilter })
                }).then(function (response) {
                    $scope.rightClassifierGrid.Options.data = response.data;
                }, function () {
                    alert('error');
                });
            },
            // cancel
            function (reason) {
            }
        );
    };

    $scope.openLeftClassifierFilter = function () {
        var leftClassifierFilterDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/DataTransfer/_LeftClassifierFilter.html',
            controller: 'LeftClassifierFilterController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogData: $scope.leftClassifierFilter
            }
        });

        leftClassifierFilterDialog.result.then(
            // ok
            function (data) {
                $scope.leftClassifierFilter = data;

                $scope.loading = $http({
                    method: 'POST',
                    url: '/DataTransfer/GetLeftClassifier',
                    data: JSON.stringify({ filter: $scope.leftClassifierFilter })
                }).then(function (response) {
                    $scope.leftClassifierGrid.Options.data = response.data;
                }, function () {
                    alert('error');
                });
            },
            // cancel
            function (reason) {
            }
        );
    };

    $scope.transfer = function() {
        var selectedLeftClassifierIds = $scope.leftClassifierGridApi.selection.getSelectedRows().map(function (value) {
            return value.ClassifierId;
        });

        var selectedRightClassifierId = $scope.rightClassifierGridApi.selection.getSelectedRows().map(function (value) {
            return value.ClassifierId;
        })[0];

        $scope.loading = $http({
            method: 'POST',
            url: '/DataTransfer/Transfer',
            data: JSON.stringify({
                classifierIdsFrom: selectedLeftClassifierIds,
                classifierIdTo: selectedRightClassifierId,
                options: {
                    TransferDrugId: $scope.transferOptions.TransferDrugId,
                    TransferPackerId: $scope.transferOptions.TransferPackerId,
                    TransferOwnerTradeMarkId: $scope.transferOptions.TransferOwnerTradeMarkId,
                    MonthFrom: $scope.dateFromUnlimited ? null : $scope.transferOptions.DateFrom.getMonth() + 1,
                    MonthTo: $scope.dateToUnlimited ? null : $scope.transferOptions.DateTo.getMonth() + 1,
                    YearFrom: $scope.dateFromUnlimited ? null : $scope.transferOptions.DateFrom.getFullYear(),
                    YearTo: $scope.dateToUnlimited ? null : $scope.transferOptions.DateTo.getFullYear()
                }
            })
        }).then(function () {
            $scope.getTransferedData();
        }, function (data) {
            alert(data);
        });
    };

    $scope.getTransferedData = function() {
        $scope.loading = $http({
            method: 'POST',
            url: '/DataTransfer/GetTransferedData'
        }).then(function (response) {
            $scope.transferedDataGrid.Options.data = response.data;
        }, function () {
            alert('error');
        });
    };

    $scope.deleteTransfer = function() {
        var selectedTransfers = $scope.transferedDataGridApi.selection.getSelectedRows().map(function(value) {
            return value.Id;
        });

        var modalDeleteDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/DataTransfer/_DeleteTransferDialog.html',
            controller: 'DeleteTransferController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: function() {
                    return selectedTransfers;
                }
            }
        });

        modalDeleteDialog.result.then(
            // ok
            function(data) {
                $scope.doDeleteTransfers(selectedTransfers);
            },
            // cancel
            function(reason) {
            }
        );
    }

    $scope.editTransfer = function () {
        var selectedTransfers = $scope.transferedDataGridApi.selection.getSelectedRows().map(function (value) {
            return value.Id;
        });

        $scope.doEditTransfer(selectedTransfers);
    }

    $scope.doEditTransfer = function (transfers) {
        $scope.loading = $http({
            method: 'POST',
            url: '/DataTransfer/Edit',
            data: JSON.stringify({
                transfersToEdit: transfers,
                options: {
                    TransferDrugId: $scope.transferOptions.TransferDrugId,
                    TransferPackerId: $scope.transferOptions.TransferPackerId,
                    TransferOwnerTradeMarkId: $scope.transferOptions.TransferOwnerTradeMarkId,
                    MonthFrom: $scope.dateFromUnlimited ? null : $scope.transferOptions.DateFrom.getMonth() + 1,
                    MonthTo: $scope.dateToUnlimited ? null : $scope.transferOptions.DateTo.getMonth() + 1,
                    YearFrom: $scope.dateFromUnlimited ? null : $scope.transferOptions.DateFrom.getFullYear(),
                    YearTo: $scope.dateToUnlimited ? null : $scope.transferOptions.DateTo.getFullYear()
                }
            })
        }).then(function () {
            $scope.getTransferedData();
        }, function (data) {
            alert(data);
        });
    }

    $scope.doDeleteTransfers = function (transfers) {
        $scope.loading = $http({
            method: 'POST',
            url: '/DataTransfer/DeleteTransfers',
            data: JSON.stringify({
                transfersToDelete: transfers
            })
        }).then(function () {
            $scope.getTransferedData();
        }, function () {
            alert('Ошибка удаления');
        });
    }

    $scope.getTransferedData();


    $scope.$watch(function() {
            return {
                topPartSize: $scope.topPartSize,
                midPartSize: $scope.midPartSize
            };
        },
        function (newValue, oldValue) {
            if (newValue === oldValue && newValue === undefined)
                return;

            if (!newValue.topPartSize || !newValue.midPartSize)
                return;

            $scope.gridStyle = {
                'height': 'calc((100% - 20px - ' + newValue.topPartSize.height + ' - ' + newValue.midPartSize.height +')/2)'
            };

        },
        true);

}