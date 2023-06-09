angular
    .module('DataAggregatorModule')
    .controller('SourcePharmaciesEditorController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'messageBoxService', 'Upload', 'errorHandlerService', SourcePharmaciesEditorController]);

function SourcePharmaciesEditorController($scope, $http, $uibModal, uiGridCustomService, messageBoxService, Upload, errorHandlerService) {
    $scope.sourcePharmaciesGrid = uiGridCustomService.createGridClass($scope, 'SourcePharmaciesEditor_SourcePharmaciesGrid');
    $scope.groupsGrid = uiGridCustomService.createGridClass($scope, 'SourcePharmaciesEditor_GroupsGrid');

    var booleanCellTemplate = uiGridCustomService.getBooleanCellTemplate();
    var booleanCondition = uiGridCustomService.booleanCondition;

    $scope.sourcePharmaciesGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Используется', field: 'Use', cellTemplate: booleanCellTemplate, filter: { condition: booleanCondition } },
        { name: 'Одиночная', field: 'IsSingle', cellTemplate: booleanCellTemplate, filter: { condition: booleanCondition } },
        { name: 'Название источника', field: 'SourceName' },
        { name: 'Название источника (детально)', field: 'SourceNameDetailed' },
        { name: 'Юрлицо', field: 'EntityName' },
        { name: 'Название аптеки', field: 'PharmacyName' },
        { name: 'Номер аптеки', field: 'PharmacyNumber' },
        { name: 'Название сети', field: 'NetName' },
        { name: 'Адрес', field: 'Address' },
        { name: 'FiasGuid', field: 'FiasGuid' },
        { name: 'Имена файлов', field: 'FileNames', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.SourcePharmacyGroupId == null ? row.entity.FileNames : ""}}</div>' },
        { name: 'TargetPharmacyId', field: 'TargetPharmacyId', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.SourcePharmacyGroupId == null ? row.entity.TargetPharmacyId : ""}}</div>' },
        { name: 'Группа', field: 'SourcePharmacyGroup' }
    ];

    $scope.groupsGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Имя', field: 'GroupName' },
        { name: 'Имена файлов', field: 'FileNames' }
    ];

    $scope.sourcePharmaciesGrid.Options.modifierKeysToMultiSelect = true;
    $scope.sourcePharmaciesGrid.Options.showGridFooter = true;

    $scope.groupsGrid.Options.multiSelect = false;
    $scope.groupsGrid.Options.showGridFooter = true;

    $scope.sourcePharmaciesGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.sourcePharmaciesGridApi = gridApi;

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, selectSourcePharmacy);
        gridApi.selection.on.rowSelectionChangedBatch($scope, selectSourcePharmacy);
    };

    $scope.groupsGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.groupsGridApi = gridApi;
    };

    $scope.loadSourcePharmacies = function () {
        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/GetSourcePharmacies"
        }).then(function (response) {
            $scope.sourcePharmaciesGrid.Options.data = response.data;
            $scope.loadGroups();
        }, function () {
            $scope.sourcePharmaciesGrid.Options.data = [];
        });
    }

    $scope.loadGroups = function () {
        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/GetGroups"
        }).then(function (response) {
            $scope.groupsGrid.Options.data = response.data;
        }, function () {
            $scope.groupsGrid.Options.data = [];
        });
    }

    $scope.deleteSelectedPharmacies = function () {
        var selectedPharmacies = $scope.sourcePharmaciesGridApi.selection.getSelectedRows();

        var modalDeleteDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/SourcePharmaciesEditor/_DeletePharmaciesView.html',
            controller: 'DeletePharmaciesController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: function () {
                    return selectedPharmacies;
                }
            }
        });

        modalDeleteDialog.result.then(
            // ok
            function (data) {
                $scope.deletePharmacies();
            },
            // cancel
            function (reason) {
            }
        );
    }

    $scope.editSelectedPharmacies = function () {
        var selectedPharmacies = $scope.sourcePharmaciesGridApi.selection.getSelectedRows();

        var modalEditDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/SourcePharmaciesEditor/_SourcePharmaciesEditView.html',
            controller: 'SourcePharmaciesEditController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: function () {
                    return {
                        action: "edit",
                        data: selectedPharmacies,
                        availableGroups: $scope.groupsGrid.Options.data
                    };
                }
            }
        });

        modalEditDialog.result.then(
            // ok
            function (data) {
                for (var i = 0; i < selectedPharmacies.length; i++) {
                    selectedPharmacies[i].IsSingle = data.fieldsToSave.IsSingle ? data.dialogData.IsSingle : selectedPharmacies[i].IsSingle;
                    selectedPharmacies[i].Use = data.fieldsToSave.Use ? data.dialogData.Use : selectedPharmacies[i].Use;
                    selectedPharmacies[i].SourceName = data.fieldsToSave.SourceName ? data.dialogData.SourceName : selectedPharmacies[i].SourceName;
                    selectedPharmacies[i].SourceNameDetailed = data.fieldsToSave.SourceNameDetailed ? data.dialogData.SourceNameDetailed : selectedPharmacies[i].SourceNameDetailed;
                    selectedPharmacies[i].EntityName = data.fieldsToSave.EntityName ? data.dialogData.EntityName : selectedPharmacies[i].EntityName;
                    selectedPharmacies[i].PharmacyName = data.fieldsToSave.PharmacyName ? data.dialogData.PharmacyName : selectedPharmacies[i].PharmacyName;
                    selectedPharmacies[i].PharmacyNumber = data.fieldsToSave.PharmacyNumber ? data.dialogData.PharmacyNumber : selectedPharmacies[i].PharmacyNumber;
                    selectedPharmacies[i].NetName = data.fieldsToSave.NetName ? data.dialogData.NetName : selectedPharmacies[i].NetName;
                    selectedPharmacies[i].Address = data.fieldsToSave.Address ? data.dialogData.Address : selectedPharmacies[i].Address;
                    selectedPharmacies[i].FiasGuid = data.fieldsToSave.FiasGuid ? data.dialogData.FiasGuid : selectedPharmacies[i].FiasGuid;
                    selectedPharmacies[i].FileName = data.fieldsToSave.FileName ? data.dialogData.FileName : selectedPharmacies[i].FileName;
                    selectedPharmacies[i].TargetPharmacyId = data.fieldsToSave.TargetPharmacyId ? data.dialogData.TargetPharmacyId : selectedPharmacies[i].TargetPharmacyId;
                    selectedPharmacies[i].SourcePharmacyGroupId = data.fieldsToSave.SourcePharmacyGroup ? data.dialogData.SourcePharmacyGroup.Id : selectedPharmacies[i].SourcePharmacyGroupId;
                    selectedPharmacies[i].SourcePharmacyGroup = data.fieldsToSave.SourcePharmacyGroup ? data.dialogData.SourcePharmacyGroup.GroupName : selectedPharmacies[i].SourcePharmacyGroup;
                }

                $scope.savePharmacies({
                    pharmaciesToSave: selectedPharmacies,
                    fieldsToSave: data.fieldsToSave,
                    filesToSave: data.filesToSave
                });
            },
            // cancel
            function (reason) {
            }
        );
    }

    $scope.addNewGroup = function() {
        var modalAddGroupDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/SourcePharmaciesEditor/_GroupEditView.html',
            controller: 'GroupEditController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: function () {
                    return {
                        action: "add",
                        data: []
                    };
                }
            }
        });

        modalAddGroupDialog.result.then(
            // ok
            function (dataToAdd) {
                $scope.addGroup(dataToAdd);
            },
            // cancel
            function (reason) {
            }
        );
    }

    $scope.addGroup = function(data) {
        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/AddGroup",
            data: JSON.stringify(data)
        }).then(function (response) {
            $scope.groupsGrid.Options.data.push(response.data);
        }, function () {
            alert('Ошибка!');
        });
    }

    $scope.editSelectedGroups = function () {
        var selectedGroups = $scope.groupsGridApi.selection.getSelectedRows();

        var modalEditDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/SourcePharmaciesEditor/_GroupEditView.html',
            controller: 'GroupEditController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: function () {
                    return {
                        action: 'edit',
                        data: selectedGroups
                };
                }
            }
        });

        modalEditDialog.result.then(
            // ok
            function (data) {
                if (selectedGroups.length > 0) {
                    selectedGroups[0].GroupName = data.dialogData.GroupName;
                }
                $scope.editGroups(data);
            },
            // cancel
            function (reason) {
            }
        );
    }

    $scope.editGroups = function(data) {
        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/SaveGroup",
            data: JSON.stringify(data)
        }).then(function () {

        }, function () {
            alert('Ошибка!');
        });
    }

    $scope.deleteSelectedGroups = function () {
        var selectedGroups = $scope.groupsGridApi.selection.getSelectedRows();

        var modalDeleteDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/SourcePharmaciesEditor/_DeleteGroupView.html',
            controller: 'DeleteGroupController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: function () {
                    return selectedGroups[0];
                }
            }
        });

        modalDeleteDialog.result.then(
            // ok
            function (data) {
                $scope.deleteGroup();
            },
            // cancel
            function (reason) {
            }
        );
    }

    $scope.deleteGroup = function() {
        var selectedGroups = $scope.groupsGridApi.selection.getSelectedRows();

        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/DeleteGroup",
            data: { "idToDelete": selectedGroups[0].Id }
        }).then(function () {
            $scope.groupsGrid.Options.data.removeitem(selectedGroups[0]);
        }, function () {
            alert('Ошибка!');
        });
    }


    $scope.addNewPharmacy = function () {
        var modalAddDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/SourcePharmaciesEditor/_SourcePharmaciesEditView.html',
            controller: 'SourcePharmaciesEditController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: function () {
                    return {
                        action: "add",
                        data: [],
                        availableGroups: $scope.groupsGrid.Options.data
                    };
                }
            }
        });

        modalAddDialog.result.then(
            // ok
            function (dataToAdd) {
                $scope.addPharmacy(dataToAdd);
            },
            // cancel
            function (reason) {
            }
        );
    }

    $scope.canEdit = function () {
        return $scope.sourcePharmaciesGridApi && !($scope.sourcePharmaciesGridApi.selection.getSelectedRows().length > 0);
    }

    $scope.canEditGroup = function () {
        return $scope.groupsGridApi && !($scope.groupsGridApi.selection.getSelectedRows().length > 0);
    }

    $scope.canMerge = function () {

        if (!$scope.sourcePharmaciesGridApi)
            return false;

        var sameSource = true;

        if ($scope.sourcePharmaciesGridApi.selection.getSelectedRows().length > 1) {
            var selectedPharmaciesSource = $scope.sourcePharmaciesGridApi.selection.getSelectedRows().map(function (value) {
                return value.SourceName;
            });

            var sourceName = selectedPharmaciesSource[0];

            for (var i = 1; i < selectedPharmaciesSource.length; i++) {
                if (sourceName != selectedPharmaciesSource[i]) {
                    sameSource = false;
                    break;
                }
            }
        }

        return !(($scope.sourcePharmaciesGridApi.selection.getSelectedRows().length > 1) && sameSource);
    }

    $scope.mergeSelectedPharmacies = function () {
        var selectedPharmacies = $scope.sourcePharmaciesGridApi.selection.getSelectedRows();

        var modalMergeDialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Retail/SourcePharmaciesEditor/_MergePharmaciesView.html',
            controller: 'MergePharmaciesController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                dialogParams: function () {
                    return selectedPharmacies;
                }
            }
        });

        modalMergeDialog.result.then(
            // ok
            function (mergeTo) {
                $scope.mergePharmacies(mergeTo);
            },
            // cancel
            function (reason) {
            }
        );
    }

    $scope.savePharmacies = function(dataToSave) {
        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/SavePharmacies",
            data: JSON.stringify(dataToSave)
        }).then(function () {

        }, function () {
            alert('Ошибка!');
        });
    }

    $scope.addPharmacy = function (data) {
        var newPharmacy = {};

        newPharmacy.IsSingle = data.dialogData.IsSingle;
        newPharmacy.Use = data.dialogData.Use;
        newPharmacy.SourceName = data.dialogData.SourceName;
        newPharmacy.SourceNameDetailed = data.dialogData.SourceNameDetailed;
        newPharmacy.EntityName = data.dialogData.EntityName;
        newPharmacy.PharmacyName = data.dialogData.PharmacyName;
        newPharmacy.PharmacyNumber = data.dialogData.PharmacyNumber;
        newPharmacy.NetName = data.dialogData.NetName;
        newPharmacy.Address = data.dialogData.Address;
        newPharmacy.FiasGuid = data.dialogData.FiasGuid;
        newPharmacy.FileName = data.dialogData.FileName;
        newPharmacy.TargetPharmacyId = data.dialogData.TargetPharmacyId;
        newPharmacy.SourcePharmacyGroupId = data.dialogData.SourcePharmacyGroup.Id;
        newPharmacy.SourcePharmacyGroup = data.dialogData.SourcePharmacyGroup.GroupName;

        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/AddPharmacy",
            data: JSON.stringify({
                pharmacyToAdd: newPharmacy,
                filesToAdd: data.filesToSave
            })
        }).then(function (response) {
            $scope.sourcePharmaciesGrid.Options.data.push(response.data);
        }, function () {
            alert('Ошибка!');
        });
    }

    $scope.deletePharmacies = function() {
        var selectedPharmacies = $scope.sourcePharmaciesGridApi.selection.getSelectedRows();

        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/DeletePharmacies",
            data: selectedPharmacies.map(function (value) {
                return value.Id;
            })
        }).then(function () {
            for (var i = 0; i < selectedPharmacies.length; i++) {
                $scope.sourcePharmaciesGrid.Options.data.removeitem(selectedPharmacies[i]);
            }
        }, function () {
            alert('Ошибка!');
        });
    }

    $scope.mergePharmacies = function (mergeTo) {
        var selectedPharmacies = $scope.sourcePharmaciesGridApi.selection.getSelectedRows();

        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/MergePharmacies",
            data: {
                pharmaciesToMerge: selectedPharmacies,
                mergeTo: mergeTo
            }
        }).then(function (response) {
            var data = response.data;
            for (var i = 0; i < selectedPharmacies.length; i++) {
                if (selectedPharmacies[i].Id != data.Id) {
                    $scope.sourcePharmaciesGrid.Options.data.removeitem(selectedPharmacies[i]);
                }
            }
            for (var i = 0; i < $scope.sourcePharmaciesGrid.Options.data.length; i++) {
                if ($scope.sourcePharmaciesGrid.Options.data[i].Id == data.Id) {
                    $scope.sourcePharmaciesGrid.Options.data[i] = data;
                    break;
                }
            }
        }, function () {
            alert('Ошибка!');
        });
    }

    function selectSourcePharmacy(row) {
        if (row.entity) {
        }
    }

    $scope.import = function (event, file) {
        event.stopPropagation();

        if (file == null)
            return;

        $scope.loading = Upload.upload({
            url: '/SourcePharmaciesEditor/ImportPharmacies_from_Excel/',
            data: {
                uploads: file
            }
        }).then(function () {
            messageBoxService.showError('Файл загружен', 'Успешно');
            $scope.loadSourcePharmacies();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };

    $scope.loadSourcePharmacies();
}