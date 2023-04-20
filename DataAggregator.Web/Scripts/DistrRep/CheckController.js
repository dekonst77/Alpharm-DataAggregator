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
    $scope.ColorButton = ['btn-primary', 'btn-success', 'btn-danger', 'btn-warning', 'btn-info'];
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

    $scope.GetButtonColor = function (value) {
           
        alert(value % 7);
        alert($scope.ColorButton[value % 7])
        
    }


    $scope.Company6FPList = [];
    $scope.Rules_filterList = null;
    $scope.OFDAPIList = [];

   

    //Инициализация блока Для перезагрузки чеков
    $scope.CheckReload_Init = function () {


        $scope.CheckReloadFile_Grid = uiGridCustomService.createGridClass($scope, 'CheckReloadFile_Grid');
        $scope.CheckReloadFile_Grid.Options.data = [];
        $scope.CheckReloadFile_Grid.Options.columnDefs = [
            { name: 'Id', field: 'Id', width: 50, type: 'number' },
            { name: 'Год', field: 'Year', width: 50, type: 'number' },
            { name: 'Месяц', field: 'Month', width: 50, type: 'number' },
            { name: 'CompanyId', field: 'CompanyId', width: 50, type: 'number' },
            { name: 'Компания', field: 'CompanyName', width: 50, type: 'number' },
            { name: 'Дата добавления', field: 'DateInsert', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME, width: 180 },
            { name: 'Путь', field: 'FilePath' },
            { name: 'Источник', field: 'DataSource' },
            { name: 'Кол-во записей', field: 'Cnt', width: 50, type: 'number' },
            { name: 'Кол-во чеков', field: 'CheckCnt', width: 50, type: 'number' },
            { name: 'Некорректные 6ФП', field: 'IsBad6FP', width: 50, type: 'number' },
            { name: 'Найденные', field: 'CheckFound', width: 50, type: 'number' },
            { name: 'В обработке', field: 'CheckInWork', width: 50, type: 'number' },
            { name: 'Обработанные чеки', field: 'CheckGood', width: 50, type: 'number' },
            { name: 'Ошибки', field: 'CheckIsBad', width: 50, type: 'number' },
        ];


        $scope.CheckReloadFile_Grid.Options.customEnableRowSelection = true;
        $scope.CheckReloadFile_Grid.Options.multiSelect = true;
        $scope.CheckReloadFile_Grid.Options.enableFullRowSelection = true;
        $scope.CheckReloadFile_Grid.Options.enableRowHeaderSelection = false;
        $scope.CheckReloadFile_Grid.Options.enableRowSelection = true;
        $scope.CheckReloadFile_Grid.Options.noUnselect = true;
        $scope.CheckReloadFile_Grid.Options.appScopeProvider = $scope;
        $scope.CheckReloadFile_Grid.Options.showGridFooter = true;

        $scope.CheckReloadFile_Grid.Options.onRegisterApi = function (gridApi) {
            $scope.CheckReloadFiledApi = gridApi;
        };



        $scope.Title = "Невалидные чеки";


        $http({
            method: "POST",
            url: "/DistrRep/GetOFDAPIList/",
            data: JSON.stringify({ param: "CheckReloadInit" })
        }).then(function (response) {
            $scope.OFDAPIList =  response.data;
            //  $scope.DataSourceType = response.data;

        }, function () {
            $scope.message = "Unexpected Error";
        });










        $scope.dataLoading = $http({
            method: 'POST',
            url: '/DistrRep/CheckReloadInit/',
            data: JSON.stringify({ param: "CheckReloadInit" })
        }).then(function (response) {
            var data = [];
            var d = new Date();
             data = response.data;
            $scope.filterList = data;
            $scope.filter = data;
            $scope.filter.DateFrom = new Date();
            $scope.filter.DateFrom.setMonth(d.getMonth() - 1);
            $scope.filter.DateTo = new Date();
            //Отслеживаем изменения поисковой формы
            $scope.$watch(function () { return $scope.filter.DateFrom; },
                function () {
                    if ($scope.filter.DateFrom) {
                        $scope.filter.Year = $scope.filter.DateFrom.getFullYear();
                        $scope.filter.Month = $scope.filter.DateFrom.getMonth() + 1;
                      //  alert($scope.filter.DateFrom);
                    }

                    if (!$scope.CheckReloadForm.$invalid) {
                        //    getRules_Clients();         
                        LoadCheckFile();
                    }
                    
                }, true);
            $scope.$watch(function () { return $scope.filter.DateTo; },
                function () {
                    if ($scope.filter.DateTo) {
                      //  $scope.filter.Year = $scope.filter.DateTo.getFullYear();
                       // $scope.filter.Month = $scope.filter.DateTo.getMonth() + 1;
                        //  alert($scope.filter.DateFrom);
                    }

                    if (!$scope.CheckReloadForm.$invalid) {
                        //    getRules_Clients();         
                        LoadCheckFile();
                    }

                }, true);

            $scope.$watch(function () { return $scope.filter.Company; },
                function () {                  
                    if (!$scope.CheckReloadForm.$invalid) {
                      //  alert('Выбрана компания');
                        //  getRules_Clients();
                        LoadCheckFile();
                    }
                }, true);
            //return response.data;
          //  str = JSON.stringify($scope.filter, null, 4); // (Optional) beautiful indented output.
           // console.log(str);

        });


       


   



        $scope.SetCheckReloadOFD = function (Ofdid) {
            //  if ($scope.Grid.selectedRows() === undefined || $scope.Grid.selectedRows().length < 1 || $scope.TypeGridApi.selection.getSelectedRows() === undefined || $scope.TypeGridApi.selection.getSelectedRows() === null || $scope.TypeGridApi.selection.getSelectedRows().length < 1) {
            //      return;
            //  }
            var Ids = $scope.CheckReloadFiledApi.selection.getSelectedRows();
            // var selectedRows = $scope.Grid.selectedRows();
            // selectedRows.forEach(function (item) {
            //     $scope.Grid.GridCellsMod(item, "TypeId", Id);
            alert(Ofdid);
            alert(Ids)
    }




    function LoadCheckFile() {
        if ($scope.CheckReloadForm.$invalid) return;

        $scope.CheckReloadFile_Grid.Options.data = [];

        $scope.templatesLoading = $http({
            method: "POST",
            url: "/DistrRep/GetCheckReloadFileInfo/",
            data: JSON.stringify({ filter:$scope.filter })
        }).then(function (response) {

            $scope.CheckReloadFile_Grid.Options.data = response.data;
        }, function () {
            $scope.message = "Unexpected Error";
        });
        }



    };

}