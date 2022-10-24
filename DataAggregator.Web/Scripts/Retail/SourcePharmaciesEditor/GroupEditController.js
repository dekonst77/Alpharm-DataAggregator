angular
    .module('DataAggregatorModule')
    .controller('GroupEditController', ['$scope', '$http', '$uibModalInstance', 'dialogParams', 'uiGridCustomService', GroupEditController]);

function GroupEditController($scope, $http, $modalInstance, dialogParams, uiGridCustomService) {
    $scope.action = dialogParams.action;

    $scope.data = dialogParams.data;

    $scope.emptySource = { Id: null, Name: "Нет источника" };

    $scope.sources = [];

    $scope.groupFilesGrid = uiGridCustomService.createGridClass($scope, 'SourcePharmaciesEditor_GroupFilesGrid');
    $scope.groupFilesGrid.Options.columnDefs = [
        { name: 'Имя файла', field: 'FileName', enableCellEdit: true }
    ];

    $scope.groupFilesGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.groupFilesGridApi = gridApi;

        gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
            if ($scope.data[0]) {
                $scope.data[0].FileNames = $scope.groupFilesGrid.Options.data.map(function (value) {
                    return value.FileName;
                }).join("; ");
            }
        });
    };

    $scope.groupFilesGrid.Options.enableFiltering = false;

    $scope.dialogData = {};
    $scope.dialogData.Id = "";
    $scope.dialogData.GroupName = "";
    $scope.dialogData.Source = $scope.emptySource;
    $scope.groupFilesGrid.Options.data = [];

    $scope.loading = $http({
        method: "POST",
        url: "/SourcePharmaciesEditor/GetSources"
    }).then(function (response) {
        $scope.sources = [$scope.emptySource].concat(response.data);
    }, function () {
        alert('Ошибка!');
    });

    if ($scope.data.length == 1) {
        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/GetGroup",
            data: JSON.stringify({ groupId: $scope.data[0].Id })
        }).then(function (response) {
            var data = response.data;
            $scope.dialogData.Id = data.Id;
            $scope.dialogData.GroupName = data.GroupName;
            $scope.dialogData.Source = data.Source == null ? $scope.emptySource : data.Source;

            $scope.oldFileNames = data.FileNames;

            $scope.groupFilesGrid.Options.data = data.SourcePharmacyGroupFile;
        }, function () {
            alert('Ошибка!');
        });
    }

    $scope.addGroupFile = function () {
        if ($scope.dialogData.GroupFileNameToAdd != "") {
            $scope.groupFilesGrid.Options.data.push({
                "Id": 0,
                "SourcePharmacyGroupId": $scope.dialogData.Id,
                "FileName": $scope.dialogData.GroupFileNameToAdd
            });
            $scope.dialogData.GroupFileNameToAdd = "";

            if ($scope.data[0]) {
                $scope.data[0].FileNames = $scope.groupFilesGrid.Options.data.map(function (value) {
                    return value.FileName;
                }).join("; ");
            }
        }
    }

    $scope.removeGroupFile = function () {
        var selectedGroupFiles = $scope.groupFilesGridApi.selection.getSelectedRows();
        for (var i = 0; i < selectedGroupFiles.length; i++) {
            $scope.groupFilesGrid.Options.data.removeitem(selectedGroupFiles[i]);
        }

        if ($scope.data[0]) {
            $scope.data[0].FileNames = $scope.groupFilesGrid.Options.data.map(function (value) {
                return value.FileName;
            }).join("; ");
        }
    }

    $scope.ok = function () {
        $modalInstance.close({
            dialogData: $scope.dialogData,
            filesToSave: $scope.groupFilesGrid.Options.data
        });
    };

    $scope.cancel = function () {
        if ($scope.oldFileNames != null) {
            $scope.data[0].FileNames = $scope.oldFileNames;
        }
        $modalInstance.dismiss('dismiss');
    };
}
