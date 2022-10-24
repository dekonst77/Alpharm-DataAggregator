angular
    .module('DataAggregatorModule')
    .controller('BudjetController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', BudjetController]);

function BudjetController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.Title = "Бюджет";
    $scope.user = userService.getUser();

    $scope.Budjet_Init = function () {
        
    };

}