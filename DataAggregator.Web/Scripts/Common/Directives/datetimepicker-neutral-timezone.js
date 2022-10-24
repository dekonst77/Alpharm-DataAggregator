angular.module('DataAggregatorModule').directive('datetimepickerNeutralTimezone', function () {
    return {
        restrict: 'A',
        priority: 1,
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            ctrl.$formatters.push(function (value) {
                if (value !== undefined && value !== null) {
                    var date = new Date(Date.parse(value));
                    date = new Date(date.getTime() + (60000 * date.getTimezoneOffset()));
                    return date;
                }
            });

            ctrl.$parsers.push(function (value) {
                if (value !== undefined && value !== null) {
                    var date = new Date(value.getTime() - (60000 * value.getTimezoneOffset()));
                    return date;
                }
            });
        }
    };
})

