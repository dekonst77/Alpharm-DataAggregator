angular
    .module('DataAggregatorModule')
    .controller('PrioritetWordsController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', PrioritetWordsController]);

function PrioritetWordsController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.Title = "Приоритетные слова";
    $scope.user = userService.getUser();

    $scope.PrioritetWords_Init = function () {
        $scope.CurrentRow;
        $scope.Source = [];
        $scope.CurrentSourceId = 0;
        $scope.Grid_PrioritetWords = uiGridCustomService.createGridClassMod($scope, "Grid_PrioritetWords");
        $scope.Grid_PrioritetWords.onSelectionChanged = onSelectionChanged;
        $scope.Grid_PrioritetWords.Options.columnDefs = [
            { name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: true, width: 100, name: 'Источник', field: 'SourceId', filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.Source,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name'
            },
            { enableCellEdit: true,name: 'Name', field: 'Name', filter: { condition: uiGridCustomService.condition } },
            { enableCellEdit: true,headerTooltip: true, name: 'Value', field: 'Value', filter: { condition: uiGridCustomService.condition } }
 
        ];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/PrioritetWords/Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            var data = response.data;

            Array.prototype.push.apply($scope.Source, response.data.Data.Source);

            $scope.Search();
            return response.data;
        });
    };

    function onSelectionChanged(row) {
        if (row !== undefined) {
            $scope.CurrentRow = row;
        }
    }
    textChange = function () {
        if ($scope.CurrentRow !== undefined) {
            $scope.Grid_PrioritetWords.GridCellsMod($scope.CurrentRow, "@modify", true);
        }
    };
    $scope.Search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/PrioritetWords/PrioritetWords_search/',
                data: JSON.stringify({})
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid_PrioritetWords.Options.data = data.Data.PrioritetWords;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Search = function () {
        if ($scope.Grid_PrioritetWords.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Search_AC();
        }

    };
    $scope.Save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/PrioritetWords/PrioritetWords_save/',
                data: JSON.stringify({
                    array_PrioritetWords: $scope.Grid_PrioritetWords.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.Search_AC();
                    }
                    else {
                        
                        $scope.Grid_PrioritetWords.ClearModify();
                        $scope.Search_AC();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Add = function () {
        var new_element = {
            Id : -1,
            SourceId : $scope.CurrentSourceId,
            Value : "",
            Name : ""
        };
        $scope.Grid_PrioritetWords.Options.data.push(new_element);
    };
    $scope.SetFilter = function (Id,Name) {
        $scope.CurrentSourceId = Id;
        $scope.Grid_PrioritetWords.gridApi.grid.columns[1].filters[0].term = Name;

    };

}