/*#12117 - Фиксации точек ГС для блокировки передачи в ОФД*/
angular
    .module('DataAggregatorModule')
    .controller('GS_ToOFDController', [
        '$scope', '$http', 'errorHandlerService', 'uiGridCustomService', '$uibModal', GS_ToOFDController]);

function GS_ToOFDController($scope, $http, errorHandlerService, uiGridCustomService, $uibModal) {
    $scope.GS_ToOFD_Init = function () {
        $scope.filter = null;
        $scope.pharmacyId = null;
        $scope.inn = null;

        $scope.selectedGSIds = [];

        $scope.Grid_GS_ToOFD = uiGridCustomService.createGridClassMod($scope, "Grid_GS_ToOFD");

        $scope.Grid_GS_ToOFD.Options.enableSelectAll = true;
        $scope.Grid_GS_ToOFD.Options.multiSelect = true;
        $scope.Grid_GS_ToOFD.Options.enableRowSelection = true;
        $scope.Grid_GS_ToOFD.Options.enableFullRowSelection = false;
        $scope.Grid_GS_ToOFD.Options.enableRowHeaderSelection = true;
        $scope.Grid_GS_ToOFD.Options.noUnselect = false;

        $scope.Grid_GS_ToOFD.Options.columnDefs = [
            { name: 'ИНН', enableCellEdit: false, field: 'EntityINN', filter: { condition: uiGridCustomService.condition } },
            { name: 'PharmacyID', enableCellEdit: false, field: 'PharmacyId', filter: { condition: uiGridCustomService.condition } },
            { name: 'Полный адрес', enableCellEdit: false, field: 'Address', filter: { condition: uiGridCustomService.condition } },
            {
                name: 'Блокировка', enableCellEdit: false,
                field: 'ToOFD',
                type: 'boolean',
                filter: {
                    condition: uiGridCustomService.condition
                },
                //для поля "Блокировка" выводим занчения наоборот: false (не пускаем в ОФД), Блокировка = да ИЛИ true (пускаем в ОФД), Блокировка = нет
                cellTemplate: '<span class="glyphicon" style="left: 50%;font-size: 20px;" ng-class="{\'glyphicon-unchecked\':COL_FIELD==true,\'glyphicon-check\':COL_FIELD==false}"></span>'
            },
            { name: 'Комментарий', enableCellEdit: false, field: 'ToOFDComment', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.Grid_GS_ToOFD.Options.onRegisterApi = function (gridApi) {
            $scope.gridApi_Grid_GS_ToOFD = gridApi;
            $scope.gridApi_Grid_GS_ToOFD.selection.on.rowSelectionChanged($scope, GS_ToOFD_select);
            $scope.gridApi_Grid_GS_ToOFD.selection.on.rowSelectionChangedBatch($scope, function (rows) {
                rows.forEach(x => GS_ToOFD_select(x))
            });
        };
    };

    function GS_ToOFD_select(row) {
        if (row.entity && row.entity.Id) {
            let Id = row.entity.Id;
            var index = $scope.selectedGSIds.indexOf(Id);
            if (index !== -1)
                $scope.selectedGSIds.splice(index, 1);
            else
                $scope.selectedGSIds.push(Id);
        }
    }

    $scope.GS_ToOFD_search = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/OFD/GS_ToOFD_search/',
                data: JSON.stringify({
                    filter: $scope.filter, pharmacyId: $scope.pharmacyId, inn: $scope.inn
                })
            }).then(function (response) {
                if (response.data.Success && response.data.Data.GS_ToOFD) 
                    $scope.Grid_GS_ToOFD.Options.data = response.data.Data.GS_ToOFD;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.GS_ToOFD_setBlocked = function () {
        if ($scope.selectedGSIds.length > 0) {

            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'Views/OFD/GS_ToOFD_Comment.html',
                size: 'lg',
                controller: 'GS_ToOFDCommentController',
                windowClass: 'center-modal',
                backdrop: 'static'
            });

            modalInstance.result.then(function (comment) {
                $scope.loading = $http({
                    method: 'POST',
                    url: '/OFD/GS_ToOFD_save/',
                    data: JSON.stringify({
                        array: $scope.selectedGSIds,
                        comment: comment
                    })
                }).then(function (response) {
                    var data = response.data;
                    if (data.Success) {
                        $scope.GS_ToOFD_search();
                        $scope.selectedGSIds = [];
                    }
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
            }, function () {
            });
        }
    }
}

angular
    .module('DataAggregatorModule')
    .controller('GS_ToOFDCommentController', [
        '$scope', 'messageBoxService', '$uibModalInstance', GS_ToOFDCommentController]);

function GS_ToOFDCommentController($scope, messageBoxService, $modalInstance) {
    $scope.comment = null;

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };

    $scope.save = function () {
        if ($scope.comment === null || $scope.comment.trim().length === 0) {
            messageBoxService.showError('Впишите комменатрий', 'Ошибка');
            return;
        }
        else {
            $modalInstance.close($scope.comment);
        }
    };
}