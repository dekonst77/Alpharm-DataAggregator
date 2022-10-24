angular.module('DataAggregatorModule')
    .controller('MultiSelectController',
        ['$scope', '$uibModalInstance', 'uiGridCustomService', '$q', '$timeout',
            function ($scope, $uibModalInstance, uiGridCustomService, $q, $timeout) {

                $scope.multiSelectFilter = undefined;

                $scope.sourceGrid = {
                    options: uiGridCustomService.createOptions('MultiSelect_SourceGrid')
                };

                $scope.sourceGrid.options.columnDefs = [
                    {
                        displayName: 'COMMON_GRID.NAME', field: 'displayValue', filterCellFiltered: true
                    }
                ];

                var options = {
                    customEnableRowSelection: true,
                    enableColumnResizing: true,
                    enableGridMenu: true,
                    enableSorting: true,
                    enableFiltering: true,
                    enableSelectAll: true,
                    enableRowSelection: true,
                    enableRowHeaderSelection: true,
                    enableCellEdit: false,
                    appScopeProvider: $scope,
                    enableFullRowSelection: true,
                    enableSelectionBatchEvent: true,
                    enableHighlighting: true,
                    multiSelect: true,
                    excessRows: 100
                };

                angular.extend($scope.sourceGrid.options, options);

                var gridApi = undefined;

                $scope.sourceGrid.options.onRegisterApi = function (api) {

                    gridApi = api;

                    gridApi.selection.on.rowSelectionChanged($scope, function (row, event) {
                        rowSelectionChangedBatch([row], event);
                    });

                    gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows, event) {
                        rowSelectionChangedBatch(rows, event);
                    });

                    $timeout(function () {
                        if ($scope.ngModel.availableItems)
                            showAvailableAndSelectedItems();
                        else
                            showOnlySelectedItems();
                    });
                };

                if ($scope.ngModel.search) {
                    
                    $scope.$watch(
                        function () { return $scope.multiSelectFilter; },
                        function (newValue, oldValue) {
                            if (newValue === oldValue)
                                return;

                            if (!$scope.searchPromise)
                                search();
                            else
                                $scope.searchPromise.then(function () { search(); });
                        });
                }


                function search() {
                    var searchValue = $scope.multiSelectFilter;

                    if (!!searchValue && !!(searchValue).trim())
                        startSearch(searchValue);
                    else
                        showOnlySelectedItems();
                }

                function startSearch(searchValue) {
                    var promise = $scope.ngModel.search(searchValue.trim());

                    promise.then(function (data) {
                            gridApi.selection.clearSelectedRows();
                            var hashSet = createHashSetSelectedItems();

                            $scope.sourceGrid.options.data = data;
                            gridApi.grid.modifyRows(data);

                            for (var i = 0; i < data.length; i++) {
                                if (hashSet.hasOwnProperty(data[i].displayValue))
                                    gridApi.selection.selectRow(data[i]);
                            }
                        }
                    );

                    $scope.searchPromise = promise;
                }

                function showAvailableAndSelectedItems() {

                    gridApi.selection.clearSelectedRows();
                    var hashSet = createHashSetSelectedItems();

                    var data = $scope.ngModel.availableItems.map(function (item) { return angular.copy(item); });

                    $scope.sourceGrid.options.data = data;
                    gridApi.grid.modifyRows(data);

                    for (var i = 0; i < data.length; i++) {
                        if (hashSet.hasOwnProperty(data[i].displayValue))
                            gridApi.selection.selectRow(data[i]);
                    }
                }

                function showOnlySelectedItems() {
                    var promise = $q(function (resolve) {
                        gridApi.selection.clearSelectedRows();

                        var newData = $scope.ngModel.selectedItems.map(function (item) { return angular.copy(item); });

                        $scope.sourceGrid.options.data = newData;
                        if (newData.length > 0) {
                            gridApi.grid.modifyRows(newData);
                            gridApi.selection.selectAllRows();
                        }

                        resolve();
                    });

                    $scope.searchPromise = promise;
                }

                function rowSelectionChangedBatch(rows, event) {
                    if (!event)
                        return;

                    var hashSet = createHashSetSelectedItems();
                    var newSelectedItems = [];
                    var hashSetUnselectedItems = {};

                    var i;
                    for (i = 0; i < rows.length; i++) {
                        var row = rows[i];
                        var item = row.entity;

                        if (row.isSelected) {
                            if (!hashSet.hasOwnProperty(item.displayValue)) {
                                var newItem = angular.copy(item);
                                delete newItem.$$hashKey;
                                newSelectedItems.push(newItem);
                            }
                        } else {
                            if (hashSet.hasOwnProperty(item.displayValue)) {
                                hashSetUnselectedItems[item.displayValue] = true;
                            }
                        }
                    }

                    var selectedItems = $scope.ngModel.selectedItems;
                    var updatedOldSelectedItems = [];

                    for(i=0;i<selectedItems.length;i++)
                        if (!hashSetUnselectedItems.hasOwnProperty(selectedItems[i].displayValue))
                            updatedOldSelectedItems.push(selectedItems[i]);

                    selectedItems = updatedOldSelectedItems.concat(newSelectedItems);

                    $scope.ngModel.displayValue = selectedItems.map(function (s) { return s.displayValue; }).join('; ');

                    $scope.ngModel.selectedItems = selectedItems;
                }

                function createHashSetSelectedItems() {

                    var selectedItems = $scope.ngModel.selectedItems;

                    var hashSet = {};
                    for (var i = 0; i < selectedItems.length; i++)
                        hashSet[selectedItems[i].displayValue] = true;

                    return hashSet;
                }

            }
        ]);