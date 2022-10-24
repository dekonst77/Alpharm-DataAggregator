angular.module('DataAggregatorModule').service('userService', ['$rootScope', '$http', function ($rootScope, $http) {

    var user = undefined;

    this.loadUser = loadUser;
    this.getUser = getUser;
    this.setUser = setUser;
    this.isAuthenticated = isAuthenticated;

    this.isInRole = isInRole;
    this.isInRoles = isInRoles;
    this.parseRoles = parseRoles;
    this.isInRoleConditions = isInRoleConditions;

    function loadUser() {
        return $http.get('/UserService/GetUser/')
            .then(function (response) {
                setUser(response.data);
                return true;
            }, function () {
                return false;
            });
    }

    loadUser();

    function getUser() {
        if (!user)
            return undefined;

        return JSON.parse(JSON.stringify(user));
    }

    function isAuthenticated() {
        if (!user)
            return false;
        return user.IsAuthenticated;
    }

    function setUser(newUser) {
        user = JSON.parse(JSON.stringify(newUser));
        $rootScope.$broadcast('userChanged');
        
    }

    function isInRole(roleName) {

        if (!user || !user.Roles)
            return undefined;

        return user.Roles.includes(roleName);
    }

    function isInRoles(roles) {
        var roleConditions = parseRoles(roles);
        return isInRoleConditions(roleConditions);
    }

    function parseRoles(roles) {
        var splitRoles = roles.split(',');

        var roleConditions = [];

        for (var i = 0; i < splitRoles.length; i++) {
            var role = splitRoles[i].trim();
            var isNegative = role[0] === '!';
            if (isNegative) {
                role = role.slice(1).trim();
            }
            roleConditions.push({
                role: role,
                isNegative: isNegative
            });
        }

        return roleConditions;
    }

    function isInRoleConditions(roleConditions) {
        for (var i = 0; i < roleConditions.length; i++) {
            var roleCondition = roleConditions[i];

            var isInRoleFlag = isInRole(roleCondition.role);
            if (isInRoleFlag === undefined)
                return false;

            var isNegative = roleCondition.isNegative;
            if (isInRoleFlag && !isNegative || !isInRoleFlag && isNegative)
                return true;
        }
        return false;
    }

}]);

// Чтобы инициировать сразу userService и затягивание данных по пользователю
angular.module('DataAggregatorModule').run(['userService', function () {
}]);