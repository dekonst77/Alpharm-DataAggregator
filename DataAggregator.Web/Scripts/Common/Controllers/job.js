angular.module('DataAggregatorModule').directive('job', ['$uibModal', function ($uibModal) {
    return {
        scope: {
            jobTitle: '@',
            jobServer: '@',
            jobName: '@',
        },
        templateUrl: 'Views/Static/job.html',
        controller: ['$scope', '$http', '$interval', 'errorHandlerService', '$sce', function ($scope, $http, $interval, errorHandlerService, $sce) {
            $scope.sce = $sce;
            $scope.Status = "";
            $scope.StatusId = 0;
            $scope.Go = function (Fl1) {
                $scope.loading = $http({
                    method: "POST",
                    url: "/Global/Job",
                    data: JSON.stringify({ Server: $scope.jobServer, Name: $scope.jobName, Run: Fl1 })
                }).then(function (response) {
                    $scope.Status = response.data.Data.Status;
                    $scope.StatusId = GetJobStatus($scope.Status);
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
            };
            function GetJobStatus(value) {
                if (value.indexOf("Запущена - Выполняющиеся") !== -1) {
                    return 10;
                }
                if (value.indexOf("В прошлый раз: ошибка") !== -1) {
                    return 0;
                }
                if (value.indexOf("В прошлый раз: ошибка") !== -1) {
                    return 0;
                }
                if (value.indexOf("В прошлый раз: выполнено успешно") !== -1) {
                    return 1;
                }
                if (value.indexOf("В прошлый раз: отменено") !== -1) {
                    return 3;
                }
                return -1;
            };
            $scope.Go(0);

            function runChecking() {
                var intervalPromise = $interval(function () {
                    $scope.Go(0);
                },
                    30000);
            }
            runChecking();
        }]
    };
}]);
