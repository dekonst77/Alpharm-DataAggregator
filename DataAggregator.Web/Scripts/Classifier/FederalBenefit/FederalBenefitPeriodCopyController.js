angular
    .module('DataAggregatorModule')
    .controller('FederalBenefitPeriodCopyController', ['$scope', '$uibModalInstance', '$http', 'messageBoxService', 'data', 'uiGridCustomService', 'formatConstants', FederalBenefitPeriodCopyController]);

function FederalBenefitPeriodCopyController($scope, $modalInstance, $http, messageBoxService, data, uiGridCustomService, formatConstants) {

    //Задаем свойства грида
    $scope.gridPeriodOptions = uiGridCustomService.createOptions('FederalBenefit_PeriodCopyGrid');

    var gridPeriodOptions = {
        customEnableRowSelection: true,
        enableColumnResizing: true,
        enableGridMenu: true,
        enableSorting: true,
        enableFiltering: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        selectionRowHeaderWidth: 20,
        rowHeight: 30,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: false,
        noUnselect: false,
        columnDefs: [
                        { name: 'Id', field: 'Id', width: 200, type: 'number', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Название', field: 'Name', filter: { condition: uiGridCustomService.condition } },
                        { name: 'Дата с', field: 'DateStart', type: 'date', cellFilter: formatConstants.FILTER_PERIOD_DATE, filter: { condition: uiGridCustomService.condition } },
                        { name: 'Дата по', field: 'DateEnd', type: 'date', cellFilter: formatConstants.FILTER_PERIOD_DATE, filter: { condition: uiGridCustomService.condition } }
        ]
    };

    angular.extend($scope.gridPeriodOptions, gridPeriodOptions);

    $scope.gridPeriodOptions.data = data.PeriodList.filter(function (i) { return i.Id != data.SelectPeriod.Id});


    $scope.gridPeriodOptions.onRegisterApi = function (gridApi) {

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.Item.PeriodIdFrom = row.entity.Id;
        });
    };

    $scope.Item = {
        PeriodName: data.SelectPeriod.Name,
        PeriodIdFrom: null,
        WithRewrite: false
    }

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.ok = function () {
        $modalInstance.close($scope.classifierFilter);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };

    $scope.clear = function () {
        $scope.classifierFilter = new Object();
    };

    $scope.CanCopyPeriod = function () {
        return $scope.Item.PeriodIdFrom != null;
    }
    
    $scope.CopyPeriod = function () {
        
        var json = JSON.stringify({ PeriodIdFrom: $scope.Item.PeriodIdFrom, PeriodIdTo: data.SelectPeriod.Id });

        $scope.classifierLoading =
            $http({
                method: "POST",
                url: "/FederalBenefit/CopyPeriod/",
                data: json
            }).then(function (response) {
                var responseData = response.data;

                if (responseData.Success) {
                    $modalInstance.close();
                } else {
                    messageBoxService.showError(responseData.Message);
                }

            }, function () {
                $scope.message = "Unexpected Error";
                messageBoxService.showError("Произошла ошибка");
            });
      
    }


    
}