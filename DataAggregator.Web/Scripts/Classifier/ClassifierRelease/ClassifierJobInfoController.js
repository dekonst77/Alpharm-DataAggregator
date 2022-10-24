angular
    .module('DataAggregatorModule')
    .controller('ClassifierJobInfoController', ['$scope', '$http', '$uibModalInstance', 'model', 'uiGridCustomService', 'formatConstants', '$translate', 'errorHandlerService', ClassifierJobInfoController]);


function ClassifierJobInfoController($scope, $http, $modalInstance, model, uiGridCustomService, formatConstants, $translate, errorHandlerService) {


    $scope.jobStatus = undefined;

    $scope.grid = uiGridCustomService.createGridClass($scope, 'ClassifierJobInfoControllerViewGrid');
    $scope.grid.Options.multiSelect = false;
    $scope.grid.Options.modifierKeyToMultiSelect = false;
    // Колонки грида
    $scope.grid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Описание', field: 'Step' },
        { name: 'Дата', field: 'Date', type: 'Date', cellFilter: formatConstants.FILTER_DATE_TIME },
    ];

  

    // Первоначальная загрузка формы при редактировании
    function initialize() {

     
        
        $scope.loading = $http({
            method: "POST",
            url: '/ClassifierRelease/JobInfo',
            data: JSON.stringify({ jobName: model.JobName })
        }).then(function (response) {

            var data = response.data;

            $scope.jobStatus = data.JobStatus;
            $scope.grid.Options.data = data.JobHistory;


        }, function (response) {
            $scope.jobStatus = 'Ошибка отображения';
            $scope.grid.Options.data = null;
            errorHandlerService.showResponseError(response);
        });

    }

    initialize();

    $scope.Run = function () {

        $scope.loading = $http({
            method: "POST",
            url: '/ClassifierRelease/JobRun',
            data: JSON.stringify({ jobName: model.JobName })
        }).then(function (response) {
            initialize();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };

    $scope.Stop = function () {

        $scope.loading = $http({
            method: "POST",
            url: '/ClassifierRelease/JobStop',
            data: JSON.stringify({ jobName: model.JobName })
        }).then(function (response) {
            initialize();
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.canRun = function () {

        return true;
    };

    // Закрыть форму
    $scope.Close = function () {
        $modalInstance.dismiss();
    };

}