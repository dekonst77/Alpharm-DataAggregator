angular
    .module('DataAggregatorModule')
    .controller('ReestrController', [
        '$scope', '$route', '$http', '$uibModal', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', ReestrController]);


function ReestrController($scope, $route, $http, $uibModal, messageBoxService, uiGridCustomService, errorHandlerService) {

    $scope.clearId = function (item) {
        item.Id = null;
    };
}