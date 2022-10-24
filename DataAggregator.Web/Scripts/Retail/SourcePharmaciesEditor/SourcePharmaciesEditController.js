angular
    .module('DataAggregatorModule')
    .controller('SourcePharmaciesEditController', ['$scope', '$http', '$uibModalInstance', 'dialogParams', 'uiGridCustomService', SourcePharmaciesEditController]);

function SourcePharmaciesEditController($scope, $http, $modalInstance, dialogParams, uiGridCustomService) {
    $scope.action = dialogParams.action;

    $scope.data = dialogParams.data;

    $scope.emptyGroup = { Id: null, GroupName: "Нет группы" };

    $scope.availableGroups = [$scope.emptyGroup].concat(dialogParams.availableGroups);

    $scope.dialogData = {};
    $scope.fieldsToSave = {};

    $scope.oldFileNames = null;

    $scope.filesGrid = uiGridCustomService.createGridClass($scope, 'SourcePharmaciesEditor_FilesGrid');
    $scope.filesGrid.Options.columnDefs = [
        { name: 'Имя файла', field: 'FileName', enableCellEdit: true }
    ];

    $scope.filesGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.filesGridApi = gridApi;

        gridApi.edit.on.afterCellEdit($scope, function(rowEntity, colDef, newValue, oldValue) {
            if ($scope.data[0]) {
                $scope.data[0].FileNames = $scope.filesGrid.Options.data.map(function (value) {
                    return value.FileName;
                }).join("; ");
            }
        });
    };

    $scope.filesGrid.Options.enableFiltering = false;

    if ($scope.data.length == 0) {
        $scope.dialogData.Id = "";
        $scope.dialogData.IsSingle = false;
        $scope.dialogData.Use = true;
        $scope.dialogData.SourceName = "";
        $scope.dialogData.SourceNameDetailed = "";
        $scope.dialogData.EntityName = "";
        $scope.dialogData.PharmacyName = "";
        $scope.dialogData.PharmacyNumber = "";
        $scope.dialogData.NetName = "";
        $scope.dialogData.Address = "";
        $scope.dialogData.FiasGuid = "";
        //$scope.dialogData.FileName = "";
        $scope.dialogData.TargetPharmacyId = "";
        $scope.dialogData.FileNameToAdd = "";
        $scope.dialogData.SourcePharmacyGroup = $scope.emptyGroup;

        $scope.fieldsToSave.Id = true;
        $scope.fieldsToSave.IsSingle = true;
        $scope.fieldsToSave.Use = true;
        $scope.fieldsToSave.SourceName = true;
        $scope.fieldsToSave.SourceNameDetailed = true;
        $scope.fieldsToSave.EntityName = true;
        $scope.fieldsToSave.PharmacyName = true;
        $scope.fieldsToSave.PharmacyNumber = true;
        $scope.fieldsToSave.NetName = true;
        $scope.fieldsToSave.Address = true;
        $scope.fieldsToSave.FiasGuid = true;
        //$scope.fieldsToSave.FileName = true;
        $scope.fieldsToSave.FileNames = true;
        $scope.fieldsToSave.TargetPharmacyId = true;
        $scope.fieldsToSave.SourcePharmacyGroup = true;
    } else if ($scope.data.length == 1) {
        $scope.loading = $http({
            method: "POST",
            url: "/SourcePharmaciesEditor/GetPharmacy",
            data: JSON.stringify({ pharmacyId: $scope.data[0].Id })
        }).then(function (response) {
            var data = response.data;
            $scope.dialogData.Id = data.Id;
            $scope.dialogData.IsSingle = data.IsSingle;
            $scope.dialogData.Use = data.Use;
            $scope.dialogData.SourceName = data.SourceName;
            $scope.dialogData.SourceNameDetailed = data.SourceNameDetailed;
            $scope.dialogData.EntityName = data.EntityName;
            $scope.dialogData.PharmacyName = data.PharmacyName;
            $scope.dialogData.PharmacyNumber = data.PharmacyNumber;
            $scope.dialogData.NetName = data.NetName;
            $scope.dialogData.Address = data.Address;
            $scope.dialogData.FiasGuid = data.FiasGuid;
            //$scope.dialogData.FileName = data.FileName;
            $scope.dialogData.TargetPharmacyId = data.TargetPharmacyId;
            $scope.dialogData.SourcePharmacyGroup = data.SourcePharmacyGroup == null ? $scope.emptyGroup : data.SourcePharmacyGroup;
            $scope.dialogData.FileNameToAdd = "";

            $scope.oldFileNames = data.FileNames;

            $scope.filesGrid.Options.data = data.SourcePharmacyFile;
        }, function () {
            alert('Ошибка!');
        });

        $scope.fieldsToSave.Id = true;
        $scope.fieldsToSave.IsSingle = true;
        $scope.fieldsToSave.Use = true;
        $scope.fieldsToSave.SourceName = true;
        $scope.fieldsToSave.SourceNameDetailed = true;
        $scope.fieldsToSave.EntityName = true;
        $scope.fieldsToSave.PharmacyName = true;
        $scope.fieldsToSave.PharmacyNumber = true;
        $scope.fieldsToSave.NetName = true;
        $scope.fieldsToSave.Address = true;
        $scope.fieldsToSave.FiasGuid = true;
        //$scope.fieldsToSave.FileName = true;
        $scope.fieldsToSave.FileNames = true;
        $scope.fieldsToSave.TargetPharmacyId = true;
        $scope.fieldsToSave.SourcePharmacyGroup = true;
    } else {
        $scope.dialogData.Id = $scope.data.map(function (value) {
            return value.Id;
        }).join(", ");
        $scope.dialogData.IsSingle = false;
        $scope.dialogData.Use = false;
        $scope.dialogData.SourceName = "";
        $scope.dialogData.SourceNameDetailed = "";
        $scope.dialogData.EntityName = "";
        $scope.dialogData.PharmacyName = "";
        $scope.dialogData.PharmacyNumber = "";
        $scope.dialogData.NetName = "";
        $scope.dialogData.Address = "";
        $scope.dialogData.FiasGuid = "";
        //$scope.dialogData.FileName = "";
        $scope.dialogData.TargetPharmacyId = "";
        $scope.dialogData.FileNameToAdd = "";
        $scope.dialogData.SourcePharmacyGroup = $scope.emptyGroup;

        $scope.fieldsToSave.Id = false;
        $scope.fieldsToSave.IsSingle = false;
        $scope.fieldsToSave.Use = false;
        $scope.fieldsToSave.SourceName = false;
        $scope.fieldsToSave.SourceNameDetailed = false;
        $scope.fieldsToSave.EntityName = false;
        $scope.fieldsToSave.PharmacyName = false;
        $scope.fieldsToSave.PharmacyNumber = false;
        $scope.fieldsToSave.NetName = false;
        $scope.fieldsToSave.Address = false;
        $scope.fieldsToSave.FiasGuid = false;
        //$scope.fieldsToSave.FileName = false;
        $scope.fieldsToSave.FileNames = false;
        $scope.fieldsToSave.TargetPharmacyId = false;
        $scope.fieldsToSave.SourcePharmacyGroup = false;
    }

    $scope.ok = function () {
        $modalInstance.close({
            dialogData: $scope.dialogData,
            fieldsToSave: $scope.fieldsToSave,
            filesToSave: $scope.filesGrid.Options.data
        });
    };

    $scope.cancel = function () {
        if ($scope.oldFileNames != null) {
            $scope.data[0].FileNames = $scope.oldFileNames;
        }
        $modalInstance.dismiss('dismiss');
    };

    $scope.addFile = function () {
        if ($scope.dialogData.FileNameToAdd != "") {
            $scope.filesGrid.Options.data.push({
                "Id": 0,
                "SourcePharmacyId": $scope.dialogData.Id,
                "FileName": $scope.dialogData.FileNameToAdd
            });
            $scope.dialogData.FileNameToAdd = "";

            if ($scope.data[0]) {
                $scope.data[0].FileNames = $scope.filesGrid.Options.data.map(function (value) {
                    return value.FileName;
                }).join("; ");
            }
        }
    };

    $scope.removeFile = function () {
        var selectedFiles = $scope.filesGridApi.selection.getSelectedRows();
        for (var i = 0; i < selectedFiles.length; i++) {
            $scope.filesGrid.Options.data.removeitem(selectedFiles[i]);
        }

        if ($scope.data[0]) {
            $scope.data[0].FileNames = $scope.filesGrid.Options.data.map(function (value) {
                return value.FileName;
            }).join("; ");
        }
    };
}

