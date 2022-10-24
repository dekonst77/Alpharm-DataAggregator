angular
    .module('DataAggregatorModule')
    .controller('LoginController', ['$scope', '$http', '$location', 'userService', 'errorHandlerService', '$routeParams', LoginController]);

function LoginController($scope, $http, $location, userService, errorHandlerService, $routeParams) {
    $scope.model = {};

    var returnUrl = $routeParams.returnUrl;
    if (!returnUrl)
        returnUrl = '/';

    $scope.IsAuth = function () {
        if (userService.isAuthenticated()===true) {
            $location.path(returnUrl).search({});//Подмена адреса без перегрузки   
        }
    };

    $scope.login = function () {

        $scope.loading = $http.post('Account/Login', JSON.stringify($scope.model))
            .then(function (response) {
                var user = response.data;
                userService.setUser(user);
                //location.reload(true);//Перегрузить Страницу
                //$location.path("");
                location.reload(true);

                //$location.path(returnUrl).search({});//Подмена адреса без перегрузки                
                //
                //returnUrl='#'+returnUrl
               // location.replace(returnUrl);
            }, function(response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.IsAuth();
}