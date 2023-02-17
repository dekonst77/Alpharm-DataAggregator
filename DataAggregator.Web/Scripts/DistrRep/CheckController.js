angular
    .module('DataAggregatorModule')
    .controller('CheckController', [
        '$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', CheckController])

    .filter('griddropdownSSS', function () {
        return function (input, context) {

            try {

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
                return input;

            } catch (e) {
              
            }
        };
    });
function CheckController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {

    //Функция обработки выделения через Shift
    function setIsShift(value) {
        if (value && !$('#grid-block').hasClass('disableSelection')) {
            $scope.selectedText = commonService.getSelectionText();
        }

        if (value) {
            $('#grid-block').addClass('disableSelection');
        } else {
            $('#grid-block').removeClass('disableSelection');
        }
    }
    $scope.Companylist = [];






}