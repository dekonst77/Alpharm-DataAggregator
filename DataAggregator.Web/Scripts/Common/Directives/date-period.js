angular.module('DataAggregatorModule').directive('datePeriod', function () {
        return {
            scope: {
                period: '=',
                ngDisabled: '=',
                ngRequired: '=',
                apLabel: '@',
                placeholder: '@'
            },

            controller: ['$scope', function ($scope) {

                if ($scope.apLabel === undefined)
                    $scope.apLabel = 'PERIOD';

                if ($scope.placeholder === undefined && $scope.apLabel !== '')
                    $scope.placeholder = $scope.apLabel;

                $scope.format = 'MM.yyyy';

                //Обработка событий календаря
                $scope.popupDate = {
                    opened: false
                };

                //Обработка событий календаря
                $scope.openDate = function () {
                    $scope.popupDate.opened = true;
                };

            }],

            templateUrl: 'Views/Static/DatePeriod.html'
        };
    });