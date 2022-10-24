angular
    .module('DataAggregatorModule')
    .controller('VEDPeriodController', ['$scope', '$http', '$uibModalInstance', '$uibModal', 'messageBoxService', 'uiGridCustomService', 'formatConstants', VEDPeriodController]);


function VEDPeriodController($scope, $http, $modalInstance, $uibModal, messageBoxService, uiGridCustomService, formatConstants) {

    //Задаем свойства грида
    $scope.gridOptions = uiGridCustomService.createOptions('Ved_PeriodGrid');

    var gridOptions = {
        customEnableRowSelection: true,
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

    angular.extend($scope.gridOptions, gridOptions);


    $scope.gridOptions.onRegisterApi = function (gridApi) {

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.Period.Id = row.entity.Id;
            $scope.Period.Name = row.entity.Name;
            $scope.Period.DateStart = row.entity.DateStart; //new Date(row.entity.YearStart, row.entity.MonthStart-1);
            $scope.Period.DateEnd = row.entity.DateEnd; //new Date(row.entity.YearEnd, row.entity.MonthEnd-1);
        });
    };

    $scope.Period = {
        Name: '',
        DateStart: null,
        DateEnd: null,

    }
  
    $scope.openDateStart = function () {
        $scope.popupStart.opened = true;
    };

    $scope.openDateEnd = function () {
        $scope.popupEnd.opened = true;
    };

    $scope.popupStart = {
        opened: false
    };

    $scope.popupEnd = {
        opened: false
    };

    function mapperiodload(value) {
        value.DateStart = new Date(value.YearStart, value.MonthStart-1,15);
        value.DateEnd = new Date(value.YearEnd, value.MonthEnd-1,15);
    }

    function LoadGridData(data) {
        data.forEach(mapperiodload);
        $scope.gridOptions.data = data;
    }

    $scope.CanDelete = function() {
        return $scope.Period != null && $scope.Period.Id != null;
    }

    $scope.CanChange = function() {
        return  $scope.Period != null &&
                $scope.Period.Name != null &&
                $scope.Period.Name.length > 0 &&
                $scope.Period.DateStart != null &&
                $scope.Period.DateStart instanceof Date &&
                $scope.Period.DateEnd != null &&
                $scope.Period.DateEnd instanceof Date;
    }



    //Добавить
    $scope.Add = function () {

        SetYearMont();

        $scope.periodLoading = $http({
            method: "POST",
            url: "/VED/PeriodAdd/",
            data: JSON.stringify({ period: $scope.Period })
        }).then(function (response) {
            var responseData = response.data;
            if (!responseData.Success) {
                messageBoxService.showError(responseData.Message);
            } else {
                LoadGridData(responseData.Data);
            }
        }, function () {
            
            $scope.message = "Unexpected Error";
        });
    };

    //Изменить
    $scope.Change = function () {

        SetYearMont();

        $scope.periodLoading = $http({
            method: "POST",
            url: "/VED/PeriodChange/",
            data: JSON.stringify({ period: $scope.Period })
        }).then(function (response) {
            var responseData = response.data;
            if (!responseData.Success) {
                messageBoxService.showError(responseData.Message);
            } else {
                LoadGridData(responseData.Data);
            }

        }, function () {

            $scope.message = "Unexpected Error";
        });
    }

    //Удалить
    $scope.Remove = function () {
        $scope.periodLoading = $http({
            method: "POST",
            url: "/VED/PeriodRemove/",
            data: JSON.stringify({ period: $scope.Period })
        }).then(function (response) {
            var responseData = response.data;
            if (!responseData.Success) {
                messageBoxService.showError(responseData.Message);
            } else {
                LoadGridData(responseData.Data);
            }
        }, function () {
          
            $scope.message = "Unexpected Error";
        });
    };

    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.CanCopy = function() {
        return $scope.Period.Id;
    }
    

    // загрузить
    function Load() {
        $scope.periodLoading = $scope.filterLoading = $http({
            method: "POST",
            url: "/VED/PeriodLoad/"
        }).then(function (response) {
            var responseData = response.data;
            if (!responseData.Success) {
                messageBoxService.showError(responseData.Message);
            } else {
                LoadGridData(responseData.Data);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    Load();


    function SetYearMont() {
            $scope.Period.YearStart = $scope.Period.DateStart.getFullYear();
            $scope.Period.MonthStart = $scope.Period.DateStart.getMonth() + 1;
            $scope.Period.YearEnd = $scope.Period.DateEnd.getFullYear();
            $scope.Period.MonthEnd = $scope.Period.DateEnd.getMonth() + 1;
    }


    $scope.CopyVED = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/VED/_PeriodCopy.html',
            controller: 'VEDPeriodCopyController',
            size: 'giant',
            backdrop: 'static',
            resolve: {
                data: {
                    PeriodList: $scope.gridOptions.data,
                    SelectPeriod: $scope.Period
                }
            }});

        modalInstance.result.then(function () {
            //Load();
        }, function () {
            //Load();
        });
    }
   

}