angular
    .module('DataAggregatorModule')
    .controller('MainController', ['$scope', MainController])
    .filter('griddropdownSSA', function () {
        return function (input, context) {

            try {
                if (context.col !== undefined) {
                    var map = context.col.colDef.editDropdownOptionsArray;
                    var idField = context.col.colDef.editDropdownIdLabel;
                    var valueField = context.col.colDef.editDropdownValueLabel;
                    //var initial = context.row.entity[context.col.field];
                    if (typeof map !== "undefined") {
                        for (var i = 0; i < map.length; i++) {
                            if (map[i][idField] == input) {
                                return map[i][valueField];
                            }
                        }
                    }
                }
                /* else if (initial) {
                     return initial;
                 }*/
                return input;

            } catch (e) {
                if (context.grid !== undefined) {
                    context.grid.appScope.log("Error: " + e);
                }
            }
        };
    })
    .config(function () {
        angular.lowercase = angular.$$lowercase;
    });
function MainController() {

}
