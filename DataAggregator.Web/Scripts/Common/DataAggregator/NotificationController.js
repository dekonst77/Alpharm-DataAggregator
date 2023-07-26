angular
    .module('DataAggregatorModule')
    .controller('NotificationController', ['$scope', '$http', 'uiGridCustomService', '$uibModal', 'messageBoxService', 'errorHandlerService', NotificationController]);

function NotificationController($scope, $http, uiGridCustomService, $uibModal, messageBoxService, errorHandlerService) {
    $scope.selectedGroupId = null;
    $scope.selectedUsers = [];

    /*группы*/
    $scope.groupListGrid = uiGridCustomService.createGridClassMod($scope, 'Notification_groupListGrid');
    $scope.groupListGrid.Options.enableRowSelection = true;
    $scope.groupListGrid.Options.enableFullRowSelection = false;
    $scope.groupListGrid.Options.enableRowHeaderSelection = true;
    $scope.groupListGrid.Options.noUnselect = false;

    $scope.groupListGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number', width: 50 },
        { name: 'Наименование', field: 'Name', enableCellEdit: true, width: 250, filter: { condition: uiGridCustomService.condition } }
    ];

    $scope.groupListGrid.SetDefaults();

    $scope.groupListGrid.Options.onRegisterApi = function (gridApi) {
        gridApi.selection.on.rowSelectionChanged($scope, selectGroup);
        gridApi.edit.on.afterCellEdit($scope, renameGroup);
    };

    function renameGroup(rowEntity, colDef, newValue, oldValue) {
        if (newValue && newValue.length > 0) {
            var json = JSON.stringify({ id: rowEntity.Id, name: newValue });
            $scope.templatesLoading = $http({
                method: "POST",
                url: "/Notification/RenameGroup/",
                data: json
            }).then(function () {
                $scope.getGroupList();
                return true;
            }, function (response) {
                errorHandlerService.showResponseError(response);
                rowEntity[colDef.field] = oldValue;
                $scope.$apply();
                return false;
            });
        }
        else {
            messageBoxService.showError('Наименование обязательно');
            rowEntity[colDef.field] = oldValue;
            $scope.$apply();
            return false;
        }
    }

    function selectGroup(row) {
        if (row.isSelected && row.entity.Id) {
            $scope.selectedGroupId = row.entity.Id;
            $scope.loadGroupUsers();
        }
        else
            $scope.selectedGroupId = null;
    }

    /*пользователи*/
    $scope.userListGrid = uiGridCustomService.createGridClassMod($scope, 'Notification_userListGrid');
    $scope.userListGrid.Options.enableRowSelection = true;
    $scope.userListGrid.Options.enableFullRowSelection = false;
    $scope.userListGrid.Options.enableRowHeaderSelection = true;
    $scope.userListGrid.Options.noUnselect = false;

    $scope.userListGrid.Options.columnDefs = [
        { name: 'UserId', field: 'UserId' },
        { name: 'Имя', field: 'FullName', width: 250, filter: { condition: uiGridCustomService.condition } },
        { name: 'Email', field: 'Email', width: 250, filter: { condition: uiGridCustomService.condition } }
    ];

    $scope.userListGrid.SetDefaults();

    $scope.userListGrid.Options.onRegisterApi = function (gridApi) {
        $scope.userGridApi = gridApi;

        gridApi.selection.on.rowSelectionChanged($scope, selectUser);
        gridApi.selection.on.rowSelectionChangedBatch($scope, selectUser);
    };

    function selectUser() {
        $scope.selectedUsers = $scope.userGridApi.selection.getSelectedRows().map(function (value) {
            return value.UserId;
        });
    }

    $scope.getGroupList = function () {
        $scope.loading = $http({
            method: 'GET',
            url: '/Notification/GetGroupList'
        }).then(function (response) {
            $scope.groupListGrid.Options.data = response.data;
        }, function () {
            $scope.groupListGrid.Options.data = [];
        });
    }

    $scope.getGroupList();

    $scope.loadGroupUsers = function () {
        $scope.loading = $http({
            method: "GET",
            url: "/Notification/GetGroupUserList?groupId=" + $scope.selectedGroupId
        }).then(function (response) {
            $scope.userListGrid.Options.data = response.data;

        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.addGroupUsers = function () {
        if ($scope.selectedGroupId == null) {
            messageBoxService.showError('Выберите группу');
            return false;
        }
        else {
            $http({
                method: 'GET',
                url: '/Notification/GetUserList',
                async: false
            }).then(function (response) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/Views/Common/Notification/NewGroupUser.html',
                    size: 'lg',
                    controller: 'NewGroupUserController',
                    windowClass: 'center-modal',
                    backdrop: 'static',
                    resolve: {
                        allUsers: function () {
                            return response.data
                        }
                    }
                });

                modalInstance.result.then(function (users) {
                    $scope.loading = $http({
                        method: 'POST',
                        url: '/Notification/AddGroupUsers/',
                        data: JSON.stringify({
                            groupId: $scope.selectedGroupId,
                            users: users
                        })
                    }).then(function () {
                        $scope.loadGroupUsers();
                    }, function (response) {
                        errorHandlerService.showResponseError(response);
                    });
                });
            });
        }
    }

    $scope.removeGroupUsers = function () {
        $scope.loading = $http({
            method: 'POST',
            url: '/Notification/RemoveGroupUsers/',
            data: JSON.stringify({
                groupId: $scope.selectedGroupId,
                users: $scope.selectedUsers
            })
        }).then(function () {
            $scope.loadGroupUsers();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }
}

angular
    .module('DataAggregatorModule')
    .controller('NewGroupUserController', [
        '$scope', 'messageBoxService', '$uibModalInstance', NewGroupUserController]);

function NewGroupUserController($scope, messageBoxService, $modalInstance) {
    $scope.users = [];

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };

    $scope.save = function () {
        if ($scope.users.length === 0) {
            messageBoxService.showError('Не выбран ни один пользователь', 'Ошибка');
            return;
        }
        else 
            $modalInstance.close($scope.users);
    };
}