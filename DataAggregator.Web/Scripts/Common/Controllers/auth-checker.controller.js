angular.module('DataAggregatorModule')
    .controller('AuthCheckerController',
        [
            '$scope', '$http', '$interval', '$translate', '$location', 'messageBoxService',
            function ($scope, $http, $interval, $translate, $location, messageBoxService) {

                var intervalPromise;
                var isFirstCheckingRequest = true;

                $scope.$on('userChanged', function () { isFirstCheckingRequest = true; });

                function runChecking() {
                    intervalPromise = $interval(function() {
                            $interval.cancel(intervalPromise);

                        $http.post('/AccountMini/IsAuthenticated').then(function(response) {
                                var data = response.data;

                                if (data.IsAuthenticated) {
                                    runChecking();
                                    return;
                                }

                                if (!isFirstCheckingRequest) {
                                    showAlert(gotoLoginPage);

                                    return;
                                }

                            }).then(() => isFirstCheckingRequest = false);

                        },
                        60000);
                }


                function showAlert(callback) {
                    //messageBoxService.showInfo($translate.instant('AUTH_CHECKER.AUTO_LOGOUT_MESSAGE')).then(callback);
                    messageBoxService.showConfirm($translate.instant('AUTH_CHECKER.AUTO_LOGOUT_MESSAGE'))
                        .then(
                            function (result) {
                                callback;
                            },
                            function (result) {
                                if (result === 'no') {
                                    //$scope.Search_AC();
                                }
                                else {
                                    //var d = "отмена";
                                }

                            });
                }

                function gotoLoginPage() {
                    $location.path('/Login');
                }

                runChecking();
            }
        ]);