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

    $scope.Company6FPList = [];
    $scope.Rules_filterList = null;
    //Инициализация блока Для перезагрузки чеков
    $scope.CheckReload_Init = function () {
        $scope.Title = "Невалидные чеки";


        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/CheckReloadInit/',
            data: JSON.stringify({ param: "CheckReloadInit" })
        }).then(function (response) {
            var data = response.data;
            $scope.filterList = data;
            $scope.filter = data.Filter;
            // $scope.filter.date = new Date(data.Filter.Year, data.Filter.Month - 1, 15);
            //Отслеживаем изменения поисковой формы
            $scope.$watch(function () { return $scope.filter.date; },
                function () {
                    if ($scope.filter.date) {
                        $scope.filter.Year = $scope.filter.date.getFullYear();
                        $scope.filter.Month = $scope.filter.date.getMonth() + 1;
                    }

                 //   if (!$scope.CheckReloadForm.$invalid)
                    //    getRules_Clients();
                    
                }, true);

            $scope.$watch(function () { return $scope.filter.Company; },
                function () {
                  //  if (!$scope.CheckReloadForm.$invalid)
                      //  getRules_Clients();
                }, true);
            //return response.data;
            str = JSON.stringify($scope.filter, null, 4); // (Optional) beautiful indented output.
            console.log(str);

        });
    };

   



}