angular
    .module('DataAggregatorModule')
    .controller('DepartmentDictionaryController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'messageBoxService',
        function ($scope, $http, $uibModal, uiGridCustomService, messageBoxService) {

                $scope.users = [];

                var departmentDictionaryGridApi = undefined;

                $scope.editMode = false;

                $scope.currentItem = {};

                $scope.managerDisplayValue = undefined;

                $scope.departmentDictionaryGrid = {
                    options: uiGridCustomService.createOptions('DepartmentDictionary_Grid')
                };

                var gridOptions = {
                    customEnableRowSelection: true,
                    enableRowSelection: true,
                    multiSelect: false
                };

                $scope.departmentDictionaryGrid.options.columnDefs = [
                    { displayName: 'COMMON.NAME', field: 'Name' },
                    { displayName: 'COMMON.SHORT_NAME', field: 'ShortName' },
                    { displayName: 'COMMON.MANAGER', field: 'ManagerName' }
                ];

                angular.extend($scope.departmentDictionaryGrid.options, gridOptions);


                $scope.departmentDictionaryGrid.options.onRegisterApi = function (gridApi) {

                    departmentDictionaryGridApi = gridApi;

                    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                        $scope.currentItem = angular.copy(row.entity);
                        $scope.managerDisplayValue = $scope.currentItem.ManagerName;
                    });

                };


                $scope.clearFields = function () {
                    var item =
                    {
                            Id: $scope.currentItem.Id
                    };

                    $scope.currentItem = item;
                    $scope.managerDisplayValue = undefined;
                };

                $scope.createItem = function () {
                    $scope.editMode = true;
                    $scope.currentItem = {};
                    $scope.managerDisplayValue = undefined;
                };

                $scope.editItem = function () {
                    $scope.editMode = true;
                };

                $scope.cancelEditing = function () {
                    $scope.editMode = false;

                    var items = getSelectedRows();
                    if (items.length === 1) {
                        $scope.currentItem = angular.copy(items[0]);
                        $scope.managerDisplayValue = $scope.currentItem.ManagerName;
                    } else {
                        $scope.currentItem = {};
                        $scope.managerDisplayValue = undefined;
                    }
                };


                // Доступность кнопок
                $scope.canCreate = function () { return !$scope.editMode; };
                $scope.canEdit = function () { return !$scope.editMode && getSelectedRows().length === 1; };
                $scope.canSave = function () { return $scope.editMode && fieldsAreValid(); };
                $scope.canCancel = function () { return $scope.editMode; };
                $scope.canDelete = function () { return !$scope.editMode && getSelectedRows().length === 1; };
                $scope.canClearFields = function () {
                    return $scope.editMode &&
                        (
                            $scope.currentItem.Name ||
                            $scope.currentItem.ShortName ||
                            $scope.currentItem.ManagerId
                        );
                };





                function getSelectedRows() {
                    return departmentDictionaryGridApi ? departmentDictionaryGridApi.selection.getSelectedRows() : [];
                }

                function fieldsAreValid() {
                    return $scope.currentItem.Name &&
                        $scope.currentItem.ShortName &&
                        $scope.currentItem.ManagerId;
                }

                function getDepartments() {
                    $scope.loading = $http.get('/DepartmentDictionary/GetDepartments')
                        .then(function (response) {
                            $scope.departmentDictionaryGrid.options.data = response.data;
                        },
                            function () {
                                alert('Ошибка');
                            });
                }

                getDepartments();

                function findIndexById(items, id) {
                    var index = -1;
                    for (var i = 0; i < items.length; i++)
                        if (items[i].Id === id) {
                            index = i;
                            break;
                        }

                    return index;
                }

                
                // Удалить элемент
                $scope.deleteItem = function () {
                    var id = $scope.currentItem.Id;
                    messageBoxService.showConfirm('Удалить подразделение?', 'Внимание')
                        .then(function () {
                            $scope.loading = $http.post('/DepartmentDictionary/DeleteRow', JSON.stringify({ id: id }))
                                .then(function () {
                                    var items = $scope.departmentDictionaryGrid.options.data;

                                    var index = findIndexById(items, id);

                                    if (index !== -1) {
                                        items.splice(index, 1);
                                        // clear all fields
                                        $scope.currentItem = {};
                                        $scope.managerDisplayValue = undefined;
                                    }

                                }, function () {
                                    alert('Ошибка');
                                });
                        }, function () { });
                };

                $scope.saveItem = function () {
                    $scope.loading = $http.post('/DepartmentDictionary/SaveRow', JSON.stringify({ model: $scope.currentItem }))
                        .then(function (response) {
                            var data = response.data;

                            if (data.isError) {
                                alert(data.errorMessage);
                            } else {
                                var items = $scope.departmentDictionaryGrid.options.data;

                                if ($scope.currentItem.Id === undefined) {
                                    items.unshift(data);
                                } else {
                                    var id = data.Id;

                                    var index = findIndexById(items, id);

                                    items[index] = data;
                                }

                                $scope.currentItem = angular.copy(data);

                                $scope.editMode = false;
                            }
                        },
                        function () {
                            alert('Ошибка');
                        });
                };

                $scope.managerChanged = function (item) {
                    $scope.currentItem.ManagerId = item !== undefined ? item.Id : undefined;
                    $scope.currentItem.ManagerName = item !== undefined ? item.Value : undefined;
                };

                (function () {
                    var httpPromise =
                        $http.get('/UsersAdmin/GetAllUsers/')
                                .then(function (response) {

                                    $scope.users = response.data;
                                });

                    $scope.loading = httpPromise;
                })();
            }
        ]);

