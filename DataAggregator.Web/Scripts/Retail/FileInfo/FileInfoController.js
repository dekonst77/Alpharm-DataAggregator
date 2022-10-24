angular
    .module('DataAggregatorModule')
    .controller('FileInfoController', ['$scope', '$http', '$uibModal', 'uiGridCustomService', 'formatConstants', FileInfoController]);

function FileInfoController($scope, $http, $uibModal, uiGridCustomService, formatConstants) {
    $scope.fileInfoGrid = uiGridCustomService.createGridClass($scope, 'FileInfo_Grid');

    $scope.fileInfoGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', width: 50, type: 'number' },
        { name: 'Статус', field: 'FileStatus', width: 200 },
        { name: 'Файл обновлен', field: 'LastWriteTime', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, width: 180 },
        { name: 'Описание ошибки', field: 'Description' },
        { name: 'Путь', field: 'Path' }
    ];

    $scope.fileInfoGrid.Options.showGridFooter = true;

    $scope.filterList = null;
    $scope.filter = {};

    init();

    function init() {
        $scope.loading = $http({
            method: "POST",
            url: "/FileInfo/Initialize"
        }).then(function (response) {
            var data = response.data;
            $scope.filterList = data;
            $scope.filter = data.Filter;
            $scope.filter.date = new Date(data.Filter.Year, data.Filter.Month - 1, 15);

            //Отслеживаем изменения поисковой формы
            $scope.$watch(function () { return $scope.filter.date; },
                function () {
                    if ($scope.filter.date) {
                        $scope.filter.Year = $scope.filter.date.getFullYear();
                        $scope.filter.Month = $scope.filter.date.getMonth() + 1;
                    }

                    if (!$scope.fileInfoForm.$invalid)
                        $scope.getInfo();

                }, true);

            $scope.$watch(function () { return $scope.filter.Source; },
                function () {
                    if (!$scope.fileInfoForm.$invalid)
                        $scope.getInfo();
                }, true);

        }, function () {
            $scope.filterList = [];
            $scope.filter = {};
        });

    };

    $scope.checkFiles = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.loading = $http({
            method: "POST",
            url: "/FileInfo/CheckFiles",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            $scope.fileInfoGrid.Options.data = response.data;
        }, function () {
            $scope.fileInfoGrid.Options.data = null;
        });
    }

    $scope.getInfo = function () {

        if ($scope.fileInfoForm.$invalid)
            return;

        $scope.loading = $http({
            method: "POST",
            url: "/FileInfo/GetInfo",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            $scope.fileInfoGrid.Options.data = response.data;
        }, function () {
            $scope.fileInfoGrid.Options.data = null;
        });
    };

    $scope.getErrorInfo = function () {

        if (!$scope.canShowError())
            return;

        $scope.loading = $http({
            method: "POST",
            url: "/FileInfo/GetErrorInfo",
            data: JSON.stringify({ filter: $scope.filter })
        }).then(function (response) {
            $scope.fileInfoGrid.Options.data = response.data;
        }, function () {
            $scope.fileInfoGrid.Options.data = null;
        });
    }

    //Отправить файлы на перезагрузку
    $scope.reloadFiles = function () {

        var item = $scope.fileInfoGrid.getSelected();

        $scope.loading = $http({
            method: "POST",
            url: "/FileInfo/SendFileInfoReload",
            data: JSON.stringify({ ids: item })
        }).then(function () {
            $scope.getInfo();
        }, function () {
            $scope.getInfo();
        });
    }

    $scope.canShowError = function () {
        return $scope.filter.Year != null && $scope.filter.Month != null && $scope.filter.Year != undefined && $scope.filter.Month != undefined;
    }

    //Возможность отправить файлы на перезагрузку
    $scope.canReload = function () {
        var item = $scope.fileInfoGrid.getSelectedItem();

        if (item == null || item.length == 0)
            return false;

        var c = item.filter(function (item) { return item.FileStatusId == null || (item.FileStatusId != 3 && item.FileStatusId != 6) });

        return c != null && c.length == 0;
    }

    //Отправить файлы на удаление
    $scope.deleteFiles = function () {
        var item = $scope.fileInfoGrid.getSelected();

        $scope.loading = $http({
            method: "POST",
            url: "/FileInfo/SendFileInfoDelete",
            data: JSON.stringify({ ids: item })
        }).then(function () {
            $scope.getInfo();
        }, function () {

        });
    }

    //Возможность отпраивть файлы на удаление
    $scope.canDelete = function () {
        var item = $scope.fileInfoGrid.getSelectedItem();

        if (item == null || item.length === 0)
            return false;

        var c = item.filter(function (item) {
            return item.FileStatusId == null || (item.FileStatusId !== 1 && item.FileStatusId !== 3)
        });

        return c != null && c.length === 0;
    }


}