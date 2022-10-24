angular
    .module('DataAggregatorModule')
    .controller('GoodsParametersEditorController', ['$scope', '$http', '$uibModal', 'messageBoxService', 'errorHandlerService', 'uiGridCustomService', 'formatConstants', '$timeout', GoodsParametersEditorController]);

function GoodsParametersEditorController($scope, $http, $uibModal, messageBoxService, errorHandlerService, uiGridCustomService, formatConstants, $timeout) {
    
    $scope.goodsCategory = null;
    $scope.selectedParameterGroup = null;
    $scope.selectedParameterLevel1 = null;
    $scope.selectedParameterLevel2 = null;
    
    // ================================== Категории ==================================

    var getGoodsCategoryList = function () {
        $scope.goodsCategory = null;
        $scope.loading = $http.post("/GoodsParametersEditor/GetGoodsCategoryList/")
            .then(function(response) {
                $scope.goodsCategoryList = response.data.Data;
            }, function() {
                messageBoxService.showError("Не удалось загрузить список категорий!");
            });
    };

    getGoodsCategoryList();
    
    // ======================== Свойства (ParameterGroup) =======================

    $scope.getParameterGroups = function (selectedItemId) {
        $scope.gridParameterLevel1.Options.data = [];
        $scope.gridParameterLevel2.Options.data = [];

        $scope.loading = $http.post('/GoodsParametersEditor/GetParameterGroups/', { goodsCategoryId: $scope.goodsCategory.Id })
            .then(function(response) {
                $scope.gridParameterGroup.Options.data = response.data.Data;

                var selectedItemIndex = selectedItemId == null ? 0 : response.data.Data.map(function (d) { return d.Id; }).indexOf(selectedItemId);;

                $scope.gridParameterGroupApi.grid.modifyRows($scope.gridParameterGroup.Options.data);
                $scope.gridParameterGroupApi.selection.selectRow($scope.gridParameterGroup.Options.data[selectedItemIndex]);

                $timeout(function () {
                    $scope.gridParameterGroupApi.core.scrollTo($scope.selectedParameterGroup);
                }, 100);
            }, function(response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.gridParameterGroup = uiGridCustomService.createGridClass($scope, 'ObjectsToObjectsReady_LogsGrid');
    $scope.gridParameterGroup.Options.multiSelect = false;
    $scope.gridParameterGroup.Options.noUnselect = true;
    $scope.gridParameterGroup.Options.columnDefs = [
        { name: 'Свойство', field: 'Name', filter: { condition: uiGridCustomService.condition }, enableCellEdit: true }
    ];
    
    $scope.gridParameterGroup.Options.onRegisterApi = function (gridApi) {
        $scope.gridParameterGroupApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, selectParameterGroup);
        gridApi.edit.on.afterCellEdit($scope, renameParameterGroup);
    };

    function selectParameterGroup(row) {
        if (row.isSelected) {
            $scope.selectedParameterGroup = row.entity;
        } else {
            $scope.selectedParameterGroup = null;
        }
        $scope.selectedParameterLevel1 = null;
        $scope.selectedParameterLevel2 = null;
        $scope.getParametersLevel1();
    }

    function renameParameterGroup(rowEntity, colDef, newValue, oldValue) {
        var data = JSON.stringify({ id: rowEntity.Id, newValue: newValue });
        $scope.sectionsLoading = $http.post('/GoodsParametersEditor/RenameParameterGroup/', data)
          .then(function (response) {
              var data = response.data;
              if (data.Success) {
                  return true;
              } else {
                  messageBoxService.showError(data.ErrorMessage);
                  rowEntity[colDef.field] = oldValue;
                  $scope.$apply();
                  return false;
              }
          }, function (response) {
              errorHandlerService.showResponseError(response);
              rowEntity[colDef.field] = oldValue;
              $scope.$apply();
              return false;
          });
    }

    $scope.addParameterGroup = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/GoodsParametersEditor/_AddParameterView.html',
            controller: 'AddParameterViewController',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function (newValue) {
            $scope.loading = $http.post("/GoodsParametersEditor/AddParameterGroup/", { newValue: newValue, goodsCategoryId: $scope.goodsCategory.Id })
                .then(function (response) {
                    var selectedItemId = response.data.Data.Id;
                    $scope.getParameterGroups(selectedItemId);
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }, function () {
        });
    }


    $scope.removeParameterGroupAsk = function () {

        if ($scope.selectedParameterGroup == null) {
            return;
        }

        messageBoxService.showConfirm('Вы уверены, что хотите удалить запись "' + $scope.selectedParameterGroup.Name + '"?', 'Удаление')
            .then(
                function() {
                    removeParameterGroup();
                },
                function() {
                });
    };

    function removeParameterGroup() {
        $scope.loading = $http.post('/GoodsParametersEditor/RemoveParameterGroup/', { parameterGroupId: $scope.selectedParameterGroup.Id })
            .then(function() {
                var selectedItemIndex = $scope.gridParameterGroup.Options.data.indexOf($scope.selectedParameterGroup);
                $scope.gridParameterGroup.Options.data.splice(selectedItemIndex, 1);
                $scope.selectedParameterGroup = null;
                $scope.selectedParameterLevel1 = null;
                $scope.selectedParameterLevel2 = null;
                $scope.gridParameterLevel1.Options.data = [];
                $scope.gridParameterLevel2.Options.data = [];
            }, function(response) {
                errorHandlerService.showResponseError(response);
            });
    }

    // ======================== Подуровень 1 =======================
    
    $scope.getParametersLevel1 = function (selectedItemId) {
        $scope.gridParameterLevel2.Options.data = [];

        $scope.loading = $http.post('/GoodsParametersEditor/GetParameters/', { parentId: $scope.selectedParameterGroup.Id, level: 1 })
            .then(function (response) {
                $scope.gridParameterLevel1.Options.data = response.data.Data;

                var selectedItemIndex = selectedItemId == null ? 0 : response.data.Data.map(function (d) { return d.Id; }).indexOf(selectedItemId);;

                $scope.gridParameterLevel1Api.grid.modifyRows($scope.gridParameterLevel1.Options.data);
                $scope.gridParameterLevel1Api.selection.selectRow($scope.gridParameterLevel1.Options.data[selectedItemIndex]);

                $timeout(function () {
                    $scope.gridParameterLevel1Api.core.scrollTo($scope.selectedParameterLevel1);
                }, 100);
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.gridParameterLevel1 = uiGridCustomService.createGridClass($scope, 'GoodsParametersEditor_ParameterLevel1Grid');
    $scope.gridParameterLevel1.Options.multiSelect = false;
    $scope.gridParameterLevel1.Options.noUnselect = true;
    $scope.gridParameterLevel1.Options.columnDefs = [
        { name: 'Подуровень 1', field: 'Value', filter: { condition: uiGridCustomService.condition }, enableCellEdit: true }
    ];

    $scope.gridParameterLevel1.Options.onRegisterApi = function (gridApi) {
        $scope.gridParameterLevel1Api = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, selectParameterLevel1);
        gridApi.edit.on.afterCellEdit($scope, renameParameter);
    };

    function selectParameterLevel1(row) {
        if (row.isSelected) {
            $scope.selectedParameterLevel1 = row.entity;
        } else {
            $scope.selectedParameterLevel1 = null;
        }
        $scope.selectedParameterLevel2 = null;
        $scope.getParametersLevel2();
    }

    function renameParameter(rowEntity, colDef, newValue, oldValue) {
        var data = JSON.stringify({ id: rowEntity.Id, newValue: newValue });
        $scope.sectionsLoading = $http.post('/GoodsParametersEditor/RenameParameter/', data)
          .then(function (response) {
              var data = response.data;
              if (data.Success) {
                  return true;
              } else {
                  messageBoxService.showError(data.ErrorMessage);
                  rowEntity[colDef.field] = oldValue;
                  $scope.$apply();
                  return false;
              }
          }, function (response) {
              errorHandlerService.showResponseError(response);
              rowEntity[colDef.field] = oldValue;
              $scope.$apply();
              return false;
          });
    }

    $scope.addParameter = function (level) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/GoodsParametersEditor/_AddParameterView.html',
            controller: 'AddParameterViewController',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function(newValue) {
            $scope.loading = $http.post(
                    "/GoodsParametersEditor/AddParameter/",
                    {
                        newValue: newValue,
                        parameterGroupId: $scope.selectedParameterGroup.Id,
                        parentId: level === 1 ? null : $scope.selectedParameterLevel1.Id
                    })
                .then(function(response) {
                    var selectedItemId = response.data.Data.Id;
                    if (level === 1) {
                        $scope.getParametersLevel1(selectedItemId);
                    } else {
                        $scope.getParametersLevel2(selectedItemId);
                    }
                }, function(response) {
                    errorHandlerService.showResponseError(response);
                });
        }, function() {
        });
    }

    $scope.removeParameterAsk = function(level) {

        if ((level === 1 && $scope.selectedParameterLevel1 == null) || (level === 2 && $scope.selectedParameterLevel2 == null)) {
            return;
        }

        messageBoxService.showConfirm('Вы уверены, что хотите удалить запись "' + (level === 1 ? $scope.selectedParameterLevel1.Value : $scope.selectedParameterLevel2.Value) + '"?', 'Удаление')
            .then(
                function() {
                    removeParameter(level);
                },
                function() {
                });
    };

    function removeParameter(level) {
        $scope.loading = $http.post('/GoodsParametersEditor/RemoveParameter/', { parameterId: level === 1 ? $scope.selectedParameterLevel1.Id : $scope.selectedParameterLevel2.Id })
            .then(function () {
                var selectedItemIndex;
                if (level === 1) {
                    selectedItemIndex = $scope.gridParameterLevel1.Options.data.indexOf($scope.selectedParameterLevel1);
                    $scope.gridParameterLevel1.Options.data.splice(selectedItemIndex, 1);
                    $scope.selectedParameterLevel1 = null;
                    $scope.selectedParameterLevel2 = null;
                    $scope.gridParameterLevel2.Options.data = [];
                } else {
                    selectedItemIndex = $scope.gridParameterLevel2.Options.data.indexOf($scope.selectedParameterLevel2);
                    $scope.gridParameterLevel2.Options.data.splice(selectedItemIndex, 1);
                    $scope.selectedParameterLevel2 = null;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    // ======================== Подуровень 2 =======================

    $scope.getParametersLevel2 = function (selectedItemId) {
        $scope.loading = $http.post('/GoodsParametersEditor/GetParameters/', { parentId: $scope.selectedParameterLevel1.Id, level: 2 })
            .then(function (response) {
                $scope.gridParameterLevel2.Options.data = response.data.Data;

                var selectedItemIndex = selectedItemId == null ? 0 : response.data.Data.map(function (d) { return d.Id; }).indexOf(selectedItemId);;

                $scope.gridParameterLevel2Api.grid.modifyRows($scope.gridParameterLevel2.Options.data);
                $scope.gridParameterLevel2Api.selection.selectRow($scope.gridParameterLevel2.Options.data[selectedItemIndex]);

                $timeout(function () {
                    $scope.gridParameterLevel2Api.core.scrollTo($scope.selectedParameterLevel2);
                }, 100);
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.gridParameterLevel2 = uiGridCustomService.createGridClass($scope, 'GoodsParametersEditor_ParameterLevel2Grid');
    $scope.gridParameterLevel2.Options.multiSelect = false;
    $scope.gridParameterLevel2.Options.noUnselect = true;
    $scope.gridParameterLevel2.Options.columnDefs = [
        { name: 'Подуровень 2', field: 'Value', filter: { condition: uiGridCustomService.condition }, enableCellEdit: true }
    ];

    $scope.gridParameterLevel2.Options.onRegisterApi = function (gridApi) {
        $scope.gridParameterLevel2Api = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, selectParameterLevel2);
        gridApi.edit.on.afterCellEdit($scope, renameParameter);
    };

    function selectParameterLevel2(row) {
        if (row.isSelected) {
            $scope.selectedParameterLevel2 = row.entity;
        } else {
            $scope.selectedParameterLevel2 = null;
        }
    }
}