angular.module('DataAggregatorModule').directive('inRoles', ['userService', function (userService) {
    return {
        link: function (scope, element, attrs) {
            if (!_.isString(attrs.inRoles)) {
                throw 'inRoles value must be a string';
            }

            var roleConditions = userService.parseRoles(attrs.inRoles);

            toggleVisibilityBasedOnUser();

            scope.$on('userChanged', toggleVisibilityBasedOnUser);

            function toggleVisibilityBasedOnUser() {
                if (userService.isInRoleConditions(roleConditions))
                    element.show();
                else
                    element.hide();
            }
        }
    };
}]);