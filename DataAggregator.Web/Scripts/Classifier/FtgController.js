angular
    .module('DataAggregatorModule')
    .controller('FtgController', ['$scope', '$http', 'messageBoxService', 'uiGridCustomService', FtgController]);

function FtgController($scope, $http, messageBoxService, uiGridCustomService) {

    //Значение текстбокса ФТГ
    $scope.ftg = { Id : 0 , Value : ''};

    //Задаем свойства грида
    $scope.gridOptions = uiGridCustomService.createOptions('Ftg_Grid');

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
                        { name: 'ФТГ', field: 'Value', filter: { condition: uiGridCustomService.condition } }
                    ]
    };

    angular.extend($scope.gridOptions, gridOptions);


    $scope.gridOptions.onRegisterApi = function (gridApi) {
       
        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.ftg.Id = row.entity.Id;
            $scope.ftg.Value = row.entity.Value;
        });
    
       
    };

    //Загружаем все ФТГ
    function Load() {


        $scope.ftgLoading = 
        $http({
            method: "POST",
            url: "/FTG/Load/"
        }).then(function (response) {
            $scope.gridOptions.data = response.data;
            
        }, function() {
            $scope.message = "Unexpected Error";
        });
    }

    //Загружаем
    Load();

    //Добавить новый ФТГ
    $scope.add = function () {
        $scope.ftgLoading =
        $http({
            method: "POST",
            url: "/FTG/Add/",
            data: JSON.stringify({ value: $scope.ftg.Value })
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
                $scope.gridOptions.data.push(data.Ftg);
            } else {
                messageBoxService.showError(data.Message);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    //Удалить ФТГ
    $scope.delete = function () {
        $scope.ftgLoading =
         $http({
            method: "POST",
            url: "/FTG/Delete/",
            data: JSON.stringify({ value: $scope.ftg })
         }).then(function (response) {
            var data = response.data;
            if (data.Success) {
                $scope.gridOptions.data.removeitem(data.Ftg);
            } else {
                messageBoxService.showError(data.Message);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    //Изменить ФТГ
    $scope.change = function () {
        $scope.ftgLoading =
        $http({
            method: "POST",
            url: "/FTG/Change/",
            data: JSON.stringify({ value: $scope.ftg })
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
                var searchftg = $scope.gridOptions.data.filter(function (item) { return item.Id === data.Ftg.Id })[0];
                searchftg.Value = data.Ftg.Value;
            } else {
                messageBoxService.showError(data.Message);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

}