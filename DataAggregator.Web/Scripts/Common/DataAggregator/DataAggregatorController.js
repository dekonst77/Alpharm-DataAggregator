angular
    .module('DataAggregatorModule')
    .controller('DataAggregatorController', ['$scope', '$http', '$location', 'userService', 'uiGridCustomService',
        function ($scope, $http, $location, userService, uiGridCustomService) {

            // Для данного самого главного контроллера верхней частью является меню
            $scope.topMainPartSize = {
            };

            $scope.isUserDetermine = function () {
                return !!userService.getUser();
            };

            $scope.isAuthenticated = function () {
                return userService.isAuthenticated();
            };

            $scope.getUserName = function () {
                var user = userService.getUser();
                if (!user)
                    return '';
                if (!user.IsAuthenticated)
                    return '';

                return user.Fullname;
            };

            $scope.logout = function () {
                $scope.layoutLoading = $http.post('Account/LogOut')
                    .then(function () {
                        userService.setUser({});
                        $location.path('/Login');
                    }, function () {
                    });
            };

            $scope.DAM_GZ_report = [];
            $scope.DAM_GS_report = [];
            $scope.DAM_Prov_report = [];
            $scope.DAM_Distr_report = [];

            $scope.MenuReportLoad = function () {
                if ($scope.isAuthenticated()) {
                    $scope.layoutLoading = $http.post('Global/GetReportList')
                        .then(function (response) {
                            if (response.data.Data.GZ_report !== undefined) {
                                $scope.DAM_GZ_report.splice(0, $scope.DAM_GZ_report.length);
                                Array.prototype.push.apply($scope.DAM_GZ_report, response.data.Data.GZ_report);
                            }
                            if (response.data.Data.GS_report !== undefined) {
                                $scope.DAM_GS_report.splice(0, $scope.DAM_GS_report.length);
                                Array.prototype.push.apply($scope.DAM_GS_report, response.data.Data.GS_report);
                            }
                            if (response.data.Data.Prov_report !== undefined) {
                                $scope.DAM_Prov_report.splice(0, $scope.DAM_Prov_report.length);
                                Array.prototype.push.apply($scope.DAM_Prov_report, response.data.Data.Prov_report);
                            }
                            if (response.data.Data.Distr_report !== undefined) {
                                $scope.DAM_Distr_report.splice(0, $scope.DAM_Distr_report.length);
                                Array.prototype.push.apply($scope.DAM_Distr_report, response.data.Data.Distr_report);
                            }
                        }, function () {
                        });
                }
            };
            $scope.$on('userChanged', function () { $scope.MenuReportLoad(); });
        }
    ]);

