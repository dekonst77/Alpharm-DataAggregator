
//angular.module('moduleName', [])
//    .directive('directiveName', function () {
//        return {
//             compile: function compile(temaplateElement, templateAttrs) {
//                return {
//                    pre: function (scope, element, attrs) {
//                    },
//                    post: function(scope, element, attrs) { 
//                    }
//                }
//            },
//            link: function (scope, element, attrs) {

//            },
//            priority: 0,
//            terminal:false,
//            template: '<div></div>',
//            templateUrl: 'template.html',
//            replace: false,
//            transclude: false,
//            restrict: 'A',
//            scope: false,
//            controller: function ($scope, $element, $attrs, $transclude, otherInjectables) {
//            }           
//        }
//    });
angular.module('DataAggregatorModule').directive('datetime', ['$uibModal', function ($uibModal) {
    return {
        scope: {
            ngModel: '=',
            ngDisabled: '=',
            apLabel: '@',
            format: '@',
            ngRequired: '=',
            placeholder: '@'
        },
        templateUrl: 'Views/Static/datetime.html',
       /* link: function (scope, elem, attrs) {
            scope.data = []; // The directive template will loop this data

            // Watch for changes in the service
            scope.$watch(function () {
                return Tree.rebuild;
            }, function (newVal, oldVal) {
                if (newVal) Tree.Build(function (response) { scope.data = response; Tree.rebuild = false; });
            }
            );
    },*/
        controller: ['$scope', function ($scope) {
            $scope.param = {
                DateOptions: {
                    minDate: null
                },
                Opened: false
            };
            $scope.value = null;

            function parseISOAsUTC(s) {
                if (s === null)
                    return null;
                try {
                    var b = s.split(/\D/);
                    return new Date(Date.UTC(b[0], --b[1], b[2], b[3], b[4], b[5], b[6] || 0));
                }
                catch(e)
                {
                    if (s.getMonth) {
                        var date_1 = new Date(s.getFullYear(), s.getMonth(), s.getDate(), s.getHours(), s.getMinutes(), s.getSeconds());
                        date_1 = new Date(date_1.getTime() - 60000 * date_1.getTimezoneOffset());
                        return date_1;
                    }
                }
                return null;
            }

            function parse_to_datettime(s) {
                if (s === null)
                    return null;
                return new Date(s).toISOString().substr(0, 19);
            }
 
            $scope.$watch(function () { return $scope.ngModel; }, function (value) {
                if (!value) {
                    return;
                }
                $scope.value = parseISOAsUTC($scope.ngModel);
            });   
            
            $scope.$watch(function () {
                return $scope.value;
            }, function (value) {
                if (!value) {
                    return;
                }
                $scope.ngModel = parse_to_datettime($scope.value);
            });             

            if ($scope.placeholder === undefined && $scope.apLabel !== '')
                $scope.placeholder = $scope.apLabel;
        }]
    };
}]);
//<multi-select ng-model="filter.tradeName" ap-label="TRADE_NAME"></multi-select>
