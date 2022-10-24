angular.module('DataAggregatorModule').directive('multiSelect', [ '$uibModal', function ($uibModal) {
        return {
            scope: {
                ngModel: '=',
                ngDisabled: '=',
                apLabel: '@',
                ngRequired: '=',
                placeholder: '@'
            },

            controller: ['$scope', function ($scope) {

                if ($scope.placeholder === undefined && $scope.apLabel !== '')
                    $scope.placeholder = $scope.apLabel;

                $scope.openDialog = function() {
                    $uibModal.open({
                        animation: false,
                        templateUrl: 'Views/Static/multi-select/multi-select.dialog.html',
                        controller: 'MultiSelectController',
                        size: 'lg',
                        backdrop: true,
                        scope: $scope
                    });
                };
            }],

            templateUrl: 'Views/Static/multi-select/multi-select.html'
        };
    }]);