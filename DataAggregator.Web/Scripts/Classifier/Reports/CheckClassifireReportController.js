angular
    .module('DataAggregatorModule')
    .controller('CheckClassifireReportController', ['$scope', '$http', '$q', '$cacheFactory', '$filter', '$timeout', 'userService', 'uiGridCustomService', 'errorHandlerService', 'messageBoxService', 'uiGridConstants', 'formatConstants', 'uiGridPaginationService', CheckClassifireReportController]);

function CheckClassifireReportController($scope, $http, $q, $cacheFactory, $filter, $timeout, userService, uiGridCustomService, errorHandlerService, messageBoxService, uiGridConstants, formatConstants, uiGridPaginationService) {
    $scope.Title = "Отчет проверки классификатора";
    $scope.user = userService.getUser();
    $scope.canSearch = function () { return true; }
    $scope.RegistrationCertificateNumberList = []; // список РУ
    $scope.CheckClassifireReportList = []; // список отчётов

    $scope.CheckClassifireReport_Init = function () {

        //******** Grid ******** ->
        $scope.Exception_Grid = uiGridCustomService.createGridClassMod($scope, 'Exception_Grid');
        $scope.Exception_Grid.Options.showGridFooter = true;
        $scope.Exception_Grid.Options.showColumnFooter = false;
        $scope.Exception_Grid.Options.multiSelect = true;
        $scope.Exception_Grid.Options.enableSelectAll = true;
        $scope.Exception_Grid.Options.enableFiltering = true;
        $scope.Exception_Grid.Options.modifierKeysToMultiSelect = true;

        $scope.Exception_Grid.Options.columnDefs = [
            { headerTooltip: true, name: 'Id', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Id', type: 'number', visible: false, nullable: false },

            {
                enableCellEdit: true, width: 300, name: 'RegistrationCertificateNumber', field: 'RegistrationCertificateId', nullable: false, filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.RegistrationCertificateNumberList,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            },

            {
                enableCellEdit: true, width: 300, displayName: 'Отчет, в котором он исключен', name: 'ClassifierReportId', field: 'ClassifierReportId', nullable: false, filter: { condition: uiGridCustomService.conditionList },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.CheckClassifireReportList,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            }
        ];

        $scope.Exception_Grid.SetDefaults();
        
        // редактируемые поля: Comment
        function editRowDataSource(rowEntity, colDef, newValue, oldValue) {
            /*
            const year = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[1];
            const month = /(\d{4})-(\d{2})/.exec($scope.currentperiod)[2];
            const day = 15;
            const period = new Date(Date.UTC(year, month - 1, day));

            console.log('editRowDataSource() -> year = ' + year);
            console.log('editRowDataSource() -> month = ' + month);
            console.log('editRowDataSource() -> period = ' + period);

            // проверка на изменение
            if (newValue === oldValue || newValue === undefined)
                return;

            $scope.dataLoading = $http({
                method: "POST",
                url: "/SalesSKUbySF/Edit/",
                data: { record: rowEntity, fieldname: colDef.field }
            }).then(function (response) {

                var data = response.data.Data;
                if (data.Success) {
                    let record = data.Data.SalesSKUBySFRecord[0];
                    rowEntity[colDef.field] = record[colDef.field];

                    //console.log(colDef);
                    //console.log(record);

                    if ((colDef.field === "Correction_factor") || (colDef.field === "CalculatedData_PackagesNumber"))
                        rowEntity["CalculatedData_PackagesNumber_Correction"] = record["CalculatedData_PackagesNumber_Correction"];

                    //console.log(record[colDef.field]);
                } else {
                    console.error(data.ErrorMessage);
                    messageBoxService.showError(data.ErrorMessage);
                }

                return true;
            }, function (response) {
                rowEntity[colDef.field] = oldValue;
                errorHandlerService.showResponseError(response);
                return false;
            });
            */
            return;
        }
        //******** Grid ******** <-

        $scope.CheckClassifireReport_Search();
    }

    $scope.CheckClassifireReport_Search = function () {

        $scope.dataLoading = $q.all([RegistrationCertificateNumberLoading(), CheckClassifireReportLoading()]).then(
            function (response) {
                console.log("$scope.dataLoading success");
                //throw Error("Enter your error message here");
                return true;
            }, function (response) {
                console.error("$scope.dataLoading error");

                if (response.data == undefined)
                    messageBoxService.showError(response);
                else
                    errorHandlerService.showResponseError(response);

                return false;
            }
        );

        $scope.dataLoading.catch(
            function (err) {
                console.error("Ошибка загрузки: " + err);
                messageBoxService.showError("Ошибка загрузки: " + err);
                return false;
            }
        );

        $scope.dataLoading.then(
            function () {
                CheckClassifireReportView();
            }
        );

    }

    $scope.CheckClassifireReport_To_Excel = function () {
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/CheckClassifireReport/CheckClassifireReport_To_Excel/',
            data: "",
            headers: { 'Content-type': 'application/json' },
            responseType: 'arraybuffer'
        }).then(function (response) {
            console.log(response);
            /*Date.now().toISOString() +*/
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'Отчет проверки классификатора.xlsx';
            saveAs(blob, fileName);
        }, function (error) {
            console.log(error);
            messageBoxService.showError('Rejected:' + error);
        });
    }

    // загрузка списка исключений
    function CheckClassifireReportView() {
        $scope.message = 'Пожалуйста, ожидайте... Загрузка списка исключений';

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/CheckClassifireReport/CheckClassifireReportView/',
            data: ""
        }).then(function (response) {

            if (response.status === 200) {
                $scope.Exception_Grid.Options.data = response.data;

                //localStorage.setItem("currperiod", $scope.currentperiod);
                //localStorage.setItem("selregions", JSON.stringify($scope.selectByGroupModel));
            }
            else {
                console.error(response);
                messageBoxService.showError(response.statusText);
            }

        }, function (response) {
            console.error('errorHandlerService.showResponseError = ' + response);
            errorHandlerService.showResponseError(response);
        });
    }

    // загрузка сертификатов
    function RegistrationCertificateNumberLoading() {
        $scope.message = 'Пожалуйста, ожидайте... Загрузка сертификатов.';

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/CheckClassifireReport/RegistrationCertificateNumberLoad/'
        }).then(function (response) {
            if (response.data.Data.RegistrationCertificateNumberList !== undefined) {
                $scope.RegistrationCertificateNumberList.splice(0, $scope.RegistrationCertificateNumberList.length);
                Array.prototype.push.apply($scope.RegistrationCertificateNumberList, response.data.Data.RegistrationCertificateNumberList);
            }
        });
    }

    // загрузка отчётов по проверке классификатора
    function CheckClassifireReportLoading() {
        $scope.message = 'Пожалуйста, ожидайте... Загрузка отчётов.';

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/CheckClassifireReport/CheckClassifireReportLoad/'
        }).then(function (response) {
            if (response.data.Data.CheckClassifireReportList !== undefined) {
                $scope.CheckClassifireReportList.splice(0, $scope.CheckClassifireReportList.length);
                Array.prototype.push.apply($scope.CheckClassifireReportList, response.data.Data.CheckClassifireReportList);
            }
        });
    }

    $scope.addData = function () {
        var total = $scope.Exception_Grid.Options.data.push({
            "Id": "-1",
            "RegistrationCertificateId": "Выберите сертификат",
            "ClassifierReportId": "Выберите отчёт",
        });

        $scope.scrollTo(total -1, 0);

        $scope.Exception_Grid.gridApi.selection.selectRow($scope.Exception_Grid.Options.data[total-1]);
    };

    $scope.scrollTo = function (rowIndex, colIndex) {
        $timeout(
            function () {
                $scope.Exception_Grid.gridApi.core.scrollTo($scope.Exception_Grid.Options.data[rowIndex], $scope.Exception_Grid.Options.columnDefs[colIndex]);
            }, 100);
    };

}