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
            { enableCellEdit: true,headerTooltip: true, name: 'Value', field: 'Value', filter: { condition: uiGridCustomService.condition } },
            {
                enableCellEdit: false, name: 'Обработка очереди',
                cellTemplate: '<div class="ui-grid-cell-contents"><span class="text-warning" ng-if="row.entity.DtStart != null && row.entity.DtEnd == null" title="Дата начала: {{row.entity.DtStart|date:\'dd.MM.yyyy HH:mm:ss\'}}">{{row.entity.StatusDescription}}&nbsp;</span>' +
                    '<span class="text-success" ng-if="row.entity.DtStart != null && row.entity.DtEnd != null" title="Дата начала: {{row.entity.DtStart|date:\'dd.MM.yyyy HH:mm:ss\'}} Дата завершения: {{row.entity.DtEnd|date:\'dd.MM.yyyy HH:mm:ss\'}}">{{row.entity.StatusDescription}}&nbsp;</span>' +
                    '<span class="text-danger" ng-if="row.entity.Queuing_Order != null" title="Номер в очереди: {{ row.entity.Queuing_Order }}">{{row.entity.StatusQueue}}</small></div>',
                filter: { condition: uiGridCustomService.condition }
            },
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
                    
                    if ($scope.Grid_PrioritetWords.gridApi.grid.columns[1].filters[0].term !== undefined) {
                        var source = $scope.Source.filter(function (item, index) { return item.Name == $scope.Grid_PrioritetWords.gridApi.grid.columns[1].filters[0].term })[0];
                        $scope.SetFilter(source.Id, source.Name);
                    }
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
        var sourceId = $scope.Source.filter(function (item, index) { return item.Name == $scope.Grid_PrioritetWords.gridApi.grid.columns[1].filters[0].term })[0].Id;
        var data = $scope.Grid_PrioritetWords.Options.data.filter(function (item, index) { return item.SourceId == sourceId });
        if (data.length > 0) {
            $scope.Grid_PrioritetWords.gridApi.grid.modifyRows(data);
            $timeout(function () {
                $scope.Grid_PrioritetWords.gridApi.selection.selectRow($scope.Grid_PrioritetWords.gridApi.core.getVisibleRows()[0].entity);
            });
        }
        else {
            $scope.CurrentRow = {};
        }
    };

}