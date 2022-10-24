angular
    .module('DataAggregatorModule')
    .controller('GoodsCategoryEditorController', ['$scope', '$http', '$uibModal', 'messageBoxService', 'uiGridCustomService', GoodsCategoryEditorController]);

function GoodsCategoryEditorController($scope, $http, $uibModal, messageBoxService, uiGridCustomService) {
 
    $scope.selectedSection = null;
    $scope.selectedCategory = null;
    $scope.selectedKeyword = null;
    $scope.sectionsLoading = undefined;
    $scope.categoriesLoading = undefined;
    $scope.keywordsLoading = undefined;

    // ================================== Разделы ==================================

    $scope.gridSectionsOptions = uiGridCustomService.createOptions('GoodsCategoryEditor_SectionsGrid');

    var gridSectionsOptions = {
        customEnableRowSelection: true,
        multiSelect: false,
        enableFullRowSelection: true,
        enableRowHeaderSelection: false,
        appScopeProvider: $scope,
        enableRowSelection: true,
        showGridFooter: false,
        noUnselect: true,
        columnDefs: [
            { name: 'Разделы', field: 'Name', enableCellEdit: true }
        ]
    };

    angular.extend($scope.gridSectionsOptions, gridSectionsOptions);

    $scope.gridSectionsOptions.onRegisterApi = function (gridApi) {
        $scope.gridSectionsApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, selectSection);
        gridApi.edit.on.afterCellEdit($scope, renameSection);
    };

    // Выбрали раздел
    function selectSection(row) {
        if (row.isSelected) {
            $scope.selectedSection = row.entity;
            $scope.selectedCategory = null;
            $scope.selectedKeyword = null;
            loadCategoriesThenKeywords();
        }
    }

    // Загрузить разделы
    function loadSections() {
        $scope.gridSectionsOptions.data = [];

        $scope.sectionsLoading = $http.post('/GoodsCategoryEditor/GetSections/')
        .then(function (response) {
            var data = response.data;
            if (data.Success) {
                $scope.gridSectionsOptions.data = data.Data;
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }
        }, function () {
            messageBoxService.showError("Ошибка при загрузке списка разделов!");
        });
    }

    // Редактируем раздел
    function renameSection(rowEntity, colDef, newValue, oldValue) {
        var data = JSON.stringify({ id: rowEntity.Id, value: newValue });
        $scope.sectionsLoading = $http.post('/GoodsCategoryEditor/RenameSection/', data)
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
          }, function () {
              messageBoxService.showError("Ошибка при редактировании раздела!");
              rowEntity[colDef.field] = oldValue;
              $scope.$apply();
              return false;
          });
    }

    //Добавляем раздел
    $scope.addSection = function () {
        $scope.sectionsLoading = $http.post('/GoodsCategoryEditor/AddSection/')
            .then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.gridSectionsOptions.data.push(data.Data);

                    $scope.gridSectionsApi.grid.modifyRows($scope.gridSectionsOptions.data);

                    var index = $scope.gridSectionsOptions.data.indexOf(data.Data);
                    $scope.gridSectionsApi.selection.selectRow($scope.gridSectionsOptions.data[index]);
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function () {
                messageBoxService.showError("Ошибка при добавлении раздела!");
            });
    }

    //Удаляем раздел
    $scope.removeSection = function () {

        if ($scope.selectedSection == null)
            return;

        var data = JSON.stringify({ id: $scope.selectedSection.Id });
        $scope.sectionsLoading = $http.post('/GoodsCategoryEditor/RemoveSection/', data)
            .then(function (response) {
                var data = response.data;
                if (data.Success) {
                    var index = $scope.gridSectionsOptions.data.indexOf($scope.selectedSection);
                    $scope.gridSectionsOptions.data.splice(index, 1);
                    $scope.selectedSection = null;
                    $scope.selectedCategory = null;
                    $scope.selectedKeyword = null;
                    loadCategoriesThenKeywords();
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function () {
                messageBoxService.showError("Ошибка при удалении раздела");
            });
    }

    // ================================== Категории ==================================

    $scope.gridCategoriesOptions = uiGridCustomService.createOptions('GoodsCategoryEditor_CategoriesGrid');

    var gridCategoriesOptions = {
        customEnableRowSelection: true,
        multiSelect: false,
        enableFullRowSelection: true,
        enableRowHeaderSelection: false,
        appScopeProvider: $scope,
        enableRowSelection: true,
        showGridFooter: false,
        noUnselect: true,
        columnDefs: [
            { name: 'Категории', field: 'Name', enableCellEdit: true }
        ]
    };

    angular.extend($scope.gridCategoriesOptions, gridCategoriesOptions);

    $scope.gridCategoriesOptions.onRegisterApi = function (gridApi) {
        $scope.gridCategoriesApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, selectCategory);
        gridApi.edit.on.afterCellEdit($scope, renameCategory);
    };

    // Выбрали Категорию
    function selectCategory(row) {
        if (row.isSelected) {
            $scope.selectedCategory = row.entity;
            $scope.selectedKeyword = null;
            loadKeywords();
        }
    }

    // Загрузить категории
    function loadCategoriesThenKeywords() {
        $scope.gridCategoriesOptions.data = [];

        $scope.categoriesLoading = $http.post('/GoodsCategoryEditor/GetCategories/', JSON.stringify({ id: $scope.selectedSection.Id }))
            .then(function(response) {
                var data = response.data;
                if (data.Success) {
                    $scope.gridCategoriesOptions.data = data.Data;
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function() {
                messageBoxService.showError("Ошибка при загрузке списка категорий!");
            })
            .then(function() {
                loadKeywords();
            }, function() {
                messageBoxService.showError("Ошибка при загрузке списка категорий!");
            });
    }
    
    // Редактируем категорию
    function renameCategory(rowEntity, colDef, newValue, oldValue) {

        var data = JSON.stringify({ id: rowEntity.Id, value: newValue });
        $scope.categoriesLoading = $http.post('/GoodsCategoryEditor/RenameCategory/', data)
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
        }, function () {
            messageBoxService.showError("Ошибка при редактировании категории!");
            rowEntity[colDef.field] = oldValue;
            $scope.$apply();
            return false;
        });
    }

    //Добавляем категорию
    $scope.addCategory = function () {
        var data = JSON.stringify({ sectionId: $scope.selectedSection.Id });

        $scope.categoriesLoading = $http.post('/GoodsCategoryEditor/AddCategory/', data)
            .then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.gridCategoriesOptions.data.push(data.Data);

                    $scope.gridCategoriesApi.grid.modifyRows($scope.gridCategoriesOptions.data);

                    var index = $scope.gridCategoriesOptions.data.indexOf(data.Data);
                    $scope.gridCategoriesApi.selection.selectRow($scope.gridCategoriesOptions.data[index]);
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function() {
                messageBoxService.showError("Ошибка при добавлении категории!");
            });
    }

    //Удаляем категорию
    $scope.removeCategory = function() {

        if ($scope.selectedCategory == null)
            return;

        var data = JSON.stringify({ id: $scope.selectedCategory.Id });
        $scope.categoriesLoading = $http.post('/GoodsCategoryEditor/RemoveCategory/', data)
            .then(function(response) {
                var data = response.data;
                if (data.Success) {
                    var index = $scope.gridCategoriesOptions.data.indexOf($scope.selectedCategory);
                    $scope.gridCategoriesOptions.data.splice(index, 1);
                    $scope.selectedCategory = null;
                    $scope.selectedKeyword = null;
                    loadKeywords();
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function() {
                messageBoxService.showError("Ошибка при удалении категории!");
            });
    };

    //Изменить раздел у категории
    $scope.changeSection = function () {

        if ($scope.selectedCategory == null)
            return;

        var modalSectionChangeInstance = $uibModal.open({
            animation: true,
            templateUrl: '_goodsSectionChangeView.html',
            controller: 'GoodsSectionChangeController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams : {
                    sectionsList: $scope.gridSectionsOptions.data,
                    selectedSection: $scope.selectedSection
                }
            }
        });

        modalSectionChangeInstance.result.then(function (newSectionId) {
            changeSection(newSectionId);
        }, function () {
        });
    }

    function changeSection(newSectionId) {
        if ($scope.selectedCategory.GoodsSectionId === newSectionId)
            return;

        var data = JSON.stringify({ categoryId: $scope.selectedCategory.Id, newSectionId: newSectionId });
        $scope.categoriesLoading = $http.post('/GoodsCategoryEditor/ChangeSection/', data)
            .then(function (response) {
                var data = response.data;
                if (data.Success) {
                    var index = $scope.gridCategoriesOptions.data.indexOf($scope.selectedCategory);
                    $scope.gridCategoriesOptions.data.splice(index, 1);
                    $scope.selectedCategory = null;
                    $scope.selectedKeyword = null;
                    loadKeywords();
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function () {
                messageBoxService.showError("Ошибка при изменении раздела!");
            });
    }

    // ================================== Ключевые слова ==================================

    $scope.gridKeywordsOptions = uiGridCustomService.createOptions('GoodsCategoryEditor_KeywordsGrid');

    var gridKeywordsOptions = {
        customEnableRowSelection: true,
        multiSelect: false,
        enableFullRowSelection: true,
        enableRowHeaderSelection: false,
        appScopeProvider: $scope,
        enableRowSelection: true,
        showGridFooter: false,
        noUnselect: true,
        columnDefs: [
            { name: 'Ключевые слова', field: 'Name', enableCellEdit: true }
        ]
    };

    angular.extend($scope.gridKeywordsOptions, gridKeywordsOptions);


    $scope.gridKeywordsOptions.onRegisterApi = function (gridApi) {
        $scope.gridKeywordsApi = gridApi;
        gridApi.selection.on.rowSelectionChanged($scope, selectKeyword);
        gridApi.edit.on.afterCellEdit($scope, renameKeyword);
    };

    function selectKeyword(row) {
        if (row.isSelected)
            $scope.selectedKeyword = row.entity;
    }

    // Загрузить все ключевые слова выбранной категории
    function loadKeywords() {
        $scope.gridKeywordsOptions.data = [];

        if ($scope.selectedCategory == null)
            return;

        $scope.keywordsLoading = $http.post('/GoodsCategoryEditor/GetKeywords/', JSON.stringify({ id: $scope.selectedCategory.Id }))
        .then(function (response) {
            var data = response.data;
            if (data.Success) {
                $scope.gridKeywordsOptions.data = data.Data;
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }
        }, function () {
            messageBoxService.showError("Ошибка при загрузке списка ключевых слов!");
        });
    }

    //Добавляем ключевое слово
    $scope.addKeyword = function () {
        var data = JSON.stringify({ categoryId: $scope.selectedCategory.Id });

        $scope.keywordsLoading = $http.post('/GoodsCategoryEditor/AddKeyword/', data)
        .then(function (response) {
            var data = response.data;
            if (data.Success) {
                $scope.gridKeywordsOptions.data.push(data.Data);

                $scope.gridKeywordsApi.grid.modifyRows($scope.gridKeywordsOptions.data);

                var index = $scope.gridKeywordsOptions.data.indexOf(data.Data);
                $scope.gridKeywordsApi.selection.selectRow($scope.gridKeywordsOptions.data[index]);
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }
        }, function () {
            messageBoxService.showError("Ошибка при добавлении ключевого слова!");
        });
    }

    //Удаляем ключевое слово
    $scope.removeKeyword = function () {
        var data = JSON.stringify({ id: $scope.selectedKeyword.Id });

        $scope.keywordsLoading = $http.post('/GoodsCategoryEditor/RemoveKeyword/', data)
            .then(function(response) {
                var data = response.data;
                if (data.Success) {
                    var index = $scope.gridKeywordsOptions.data.indexOf($scope.selectedKeyword);
                    $scope.gridKeywordsOptions.data.splice(index, 1);
                    $scope.selectedKeyword = null;
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function() {
                messageBoxService.showError("Ошибка при удалении ключевого слова!");
            });
    }

    //Редактируем ключевое слово
    function renameKeyword(rowEntity, colDef, newValue, oldValue) {
        var data = JSON.stringify({ id: rowEntity.Id, value: newValue });
        $scope.keywordsLoading = $http.post('/GoodsCategoryEditor/RenameKeyword/', data)
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
        }, function () {
            messageBoxService.showError("Ошибка при редактировании ключевого слова!");
            rowEntity[colDef.field] = oldValue;
            $scope.$apply();
            return false;
        });
    }

    loadSections();
}