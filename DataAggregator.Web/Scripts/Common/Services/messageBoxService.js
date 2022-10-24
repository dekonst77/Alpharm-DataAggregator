angular.module('DataAggregatorModule').service('messageBoxService', ['$uibModal', function ($uibModal) {

    this.showInfo = function(message, header) {

        var controller = ['$scope', '$translate', 'model', function ($scope, $translate, model) {
            $scope.messageText = model.message;
            $scope.headerText = model.header ? model.header : $translate.instant('MESSAGE_BOX_SERVICE.INFO');
        }];

        var modal = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Static/message-box-service/info.html',
            controller: controller,
            resolve: {
                model: function() {
                    return {
                        message: message,
                        header: header
                    };
                }
            }
        });

        return modal.result;
    };

    this.showError = function(message, header) {

        var controller = ['$scope', '$translate','model',function($scope, $translate, model) {
            $scope.messageText = model.message;
            $scope.headerText = model.header ? model.header : $translate.instant('MESSAGE_BOX_SERVICE.ERROR');
        }];

        var modal = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Static/message-box-service/error.html',
            controller: controller,
            resolve: {
                model: function() {
                    return {
                        message: message,
                        header: header
                    };
                }
            }
        });

        return modal.result;
    };

    this.showConfirm = function(message, header) {

        var controller = ['$scope', '$translate', 'model', function ($scope, $translate, model) {
            $scope.messageText = model.message;
            $scope.headerText = model.header ? model.header : $translate.instant('MESSAGE_BOX_SERVICE.CONFIRMATION');
        }];

        var modal = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Static/message-box-service/confirmation.html',
            controller: controller,
            resolve: {
                model: function() {
                    return {
                        message: message,
                        header: header
                    };
                }
            }
        });

        return modal.result;
    };

}]);