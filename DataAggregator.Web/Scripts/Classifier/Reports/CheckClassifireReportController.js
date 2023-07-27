angular
    .module('DataAggregatorModule')
    .controller('CheckClassifireReportController', ['$scope', '$http', '$q', '$cacheFactory', '$filter', '$timeout', 'userService', 'uiGridCustomService', 'errorHandlerService', 'messageBoxService', 'uiGridConstants', 'formatConstants', 'hotkeys', '$uibModal', CheckClassifireReportController])
    .filter('mapReport', mapReport);

function mapReport() {
    var ReportrHash = {
        1: 'ATCEphmraDescription',
        2: 'FormProduct',
        3: 'FTG',
        4: 'TN+Brand',
        5: 'ATCWhoDescription'
    };

    return function (input) {
        if (!input) {
            return 'Не определено';
        } else {
            
            return ReportrHash[input];
        }
    };
}

function CheckClassifireReportController($scope, $http, $q, $cacheFactory, $filter, $timeout, userService, uiGridCustomService, errorHandlerService, messageBoxService, uiGridConstants, formatConstants, hotkeys, $uibModal) {
    $scope.Title = "Отчет проверки классификатора";
    $scope.user = userService.getUser();
    $scope.canSearch = function () { return true; }
    $scope.RegistrationCertificateNumberList = []; // список РУ
    $scope.CheckClassifireReportList = []; // список отчётов

    $scope.CheckClassifireReportFilterList = [];

    $scope.CheckClassifireReport_Init = function () {

        hotkeys.bindTo($scope).add({
            combo: 'shift+d',
            description: 'Удалить исключение',
            callback: function (event) {
                $scope.deleteRecords(event);
            }
        });

        hotkeys.bindTo($scope).add({
            combo: 'shift+e',
            description: 'Редактировать исключение',
            callback: function (event) {
                $scope.editData(event);
            }
        });

        //******** Grid ******** ->
        $scope.Exception_Grid = uiGridCustomService.createGridClassMod($scope, 'Exception_Grid', null, "ExceptionGrid_dblClick");
        $scope.Exception_Grid.Options.showGridFooter = true;
        $scope.Exception_Grid.Options.showColumnFooter = false;
        $scope.Exception_Grid.Options.multiSelect = false;
        $scope.Exception_Grid.Options.enableSelectAll = false;
        $scope.Exception_Grid.Options.enableFiltering = true;
        $scope.Exception_Grid.Options.modifierKeysToMultiSelect = false;

        $scope.Exception_Grid.Options.columnDefs = [
            { headerTooltip: true, name: 'Id', enableCellEdit: false, width: 100, cellTooltip: true, field: 'Id', type: 'number', visible: false, nullable: false },

            { enableCellEdit: false, width: 300, name: 'RegistrationCertificateNumber ID', field: 'RegistrationCertificateId', nullable: false, filter: { condition: uiGridCustomService.numberCondition } },
            { enableCellEdit: false, width: 300, name: 'RegistrationCertificateNumber', field: 'RegistrationCertificateNumber', nullable: false, filter: { condition: uiGridCustomService.condition } },

            //{
            //    enableCellEdit: true, width: 300, displayName: 'Отчет, в котором он исключен', name: 'ClassifierReportId', field: 'ClassifierReportId', nullable: false, filter: { condition: uiGridCustomService.condition },
            //    editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
            //    editDropdownOptionsArray: $scope.CheckClassifireReportList,
            //    editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value'
            //},

            {
                enableCellEdit: true, width: 300, displayName: 'Отчет, в котором он исключен', name: 'ClassifierReportId', field: 'ClassifierReportId', nullable: false,
                filter: {
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: $scope.CheckClassifireReportFilterList
                },
                editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
                editDropdownOptionsArray: $scope.CheckClassifireReportList,
                editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Value',
                cellFilter: 'mapReport'
            }
        ];

        $scope.Exception_Grid.SetDefaults();
        //******** Grid ******** <-

        $scope.CheckClassifireReport_Search();
    }

    $scope.ExceptionGrid_dblClick = function (field, rowEntity) {
        if ((field === 'RegistrationCertificateId') || (field === 'RegistrationCertificateNumber'))
            $scope.editData();
    };



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
                $scope.CheckClassifireReportView();
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
    $scope.CheckClassifireReportView = function () {
        $scope.message = 'Пожалуйста, ожидайте... Загрузка списка исключений';

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/CheckClassifireReport/CheckClassifireReportView/',
            data: ""
        }).then(function (response) {

            if (response.status === 200) {
                $scope.Exception_Grid.Options.data = response.data;
            }
            else {
                console.error(response);
                messageBoxService.showError(response.statusText);
            }

        }, function (response) {
            console.error('errorHandlerService.showResponseError = ' + response);
            errorHandlerService.showResponseError(response);
        });

        return $scope.dataLoading;
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

                Array.prototype.push.apply($scope.CheckClassifireReportFilterList, $scope.CheckClassifireReportList.map(function (obj) {
                    var rObj = { 'value': obj.Id, 'label': obj.Value };
                    return rObj;
                }));
             

                $scope.Exception_Grid.Options.columnDefs.find(item => item.field === 'ClassifierReportId').filter.selectOptions = $scope.CheckClassifireReportFilterList;
            }
        });
    }

    $scope.ReportFilter = new Object;

    // добавление записи
    $scope.addData = function () {

        var modalClassifieInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/Reports/CheckReport/_CheckReportDialog.html',
            controller: 'CheckReportDialogController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                ReportFilter: function () {
                    return {
                        Action: 'add',
                        CurrentReport: {
                            Id: -1,
                            RegistrationCertificateNumber: null, // РУ
                            CheckClassifireReport: null // Отчёт
                        },
                        ReportFilterList: $scope.CheckClassifireReportList
                    };
                }
            }
        });

        modalClassifieInstance.result.then(
            function (ReportFilter) {
                $scope.ReportFilter = ReportFilter;

                if (($scope.ReportFilter.RegistrationCertificateNumber == null) || ($scope.ReportFilter.CheckClassifireReport == null))
                    throw 'Ошибка при вводе данных'

                $scope.ReportExceptionListSave();
            },
            function () {
            }
        );

        /*
        var total = $scope.Exception_Grid.Options.data.push({
            "Id": "-1",
            "RegistrationCertificateId": "Выберите сертификат",
            "ClassifierReportId": "Выберите отчёт",
        });

        $timeout(function () {
            $scope.Exception_Grid.gridApi.selection.selectRow($scope.Exception_Grid.Options.data[total - 1]);
        });

        $scope.scrollTo(total - 1, 0);
        */
    };

    // редактирование записи
    $scope.editData = function () {
        //Выберем все выделенные строки, которые видны.
        var selectedRows = $scope.Exception_Grid.gridApi.selection.getSelectedGridRows();

        if (selectedRows.length == 0)
            return;

        let currentRecord = selectedRows[0].entity;

        var modalClassifieInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/Reports/CheckReport/_CheckReportDialog.html',
            controller: 'CheckReportDialogController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                ReportFilter: function () {
                    return {
                        Action: 'edit',
                        CurrentReport: {
                            Id: currentRecord.Id,
                            RegistrationCertificateNumber: { Id: currentRecord.RegistrationCertificateId, Value: currentRecord.RegistrationCertificateNumber }, // РУ
                            CheckClassifireReport: { Id: currentRecord.ClassifierReportId, Value: currentRecord.ReportCode } // Отчёт
                        },
                        ReportFilterList: $scope.CheckClassifireReportList
                    };
                }
            }
        });

        modalClassifieInstance.result.then(
            function (ReportFilter) {
                $scope.ReportFilter = ReportFilter;

                if (($scope.ReportFilter.RegistrationCertificateNumber == null) || ($scope.ReportFilter.CheckClassifireReport == null))
                    throw 'Ошибка при вводе данных'

                $scope.ReportExceptionListSave();
            },
            function () {
            }
        );
    };

    $scope.scrollTo = function (rowIndex, colIndex) {
        $timeout(
            function () {
                $scope.Exception_Grid.gridApi.core.scrollTo($scope.Exception_Grid.Options.data[rowIndex], $scope.Exception_Grid.Options.columnDefs[colIndex]);
            }, 100);
    };

    // сохранение изменений в списке исключений
    $scope.ReportExceptionListSave = function (action) {

        var NewRecord = $scope.Exception_Grid.GetArrayModify();
        if (NewRecord.length === 0) {
            let item = {
                Id: $scope.ReportFilter.Id,
                RegistrationCertificateId: $scope.ReportFilter.RegistrationCertificateNumber.Id,
                ClassifierReportId: $scope.ReportFilter.CheckClassifireReport.Id
            }
            NewRecord.push(item);
        }

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/CheckClassifireReport/ReportExceptionListSave/',
                data: JSON.stringify({
                    ExceptionList: NewRecord
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Exception_Grid.ClearModify();

                    $scope.CheckClassifireReportView().then(function () {
                        let lastElem = data.Data.ReportExceptionRecords[data.Data.ReportExceptionRecords.length - 1]; // последний сохранённый элемент
                        let lastIndex = $scope.Exception_Grid.Options.data.findLastIndex(element => element.Id == lastElem.Id);

                        $scope.scrollTo(lastIndex, 0);
                        $timeout(function () {
                            $scope.Exception_Grid.gridApi.selection.selectRow($scope.Exception_Grid.Options.data[lastIndex]);
                        });
                    });

                    messageBoxService.showError('Сохранение успешно.<br/>');
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    //Отправить записи на удаление
    $scope.deleteRecords = function () {

        //Выберем все выделенные строки, которые видны.
        var selectedRows = $scope.Exception_Grid.gridApi.selection.getSelectedGridRows();

        if (selectedRows.length == 0)
            return;

        var r = messageBoxService.showConfirm('Вы уверены что хотите удалить выбранные записи?', 'Удаление')
        r.then(
            function () {

                txt = "You pressed OK!";

                var item = selectedRows[0].entity;

                $scope.dataLoading = $http({
                    method: "POST",
                    url: "/CheckClassifireReport/ReportExceptionListRemove/",
                    data: JSON.stringify({ ExceptionId: item.Id })
                }).then(function (response) {
                    $scope.CheckClassifireReportView();
                    messageBoxService.showInfo("Успешное удаление", 'Удаление');
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });

            },
            function () {

            });
    }

    // отправка через SmtpClient client
    $scope.CheckClassifireReport_To_Email = function () {
        $scope.message = 'Пожалуйста, ожидайте... Загрузка отчётов.';

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/CheckClassifireReport/CheckClassifireReport_To_Email/'
        }).then(function (response) {

        }, function (response) {
            console.error('errorHandlerService.showResponseError = ' + response);
            errorHandlerService.showResponseError(response);
        });
    }

    // отправка через хран. проц-ру
    $scope.CheckClassifireReportToEmailOverDBProfile = function () {
        $scope.message = 'Пожалуйста, ожидайте... Загрузка отчётов.';

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/CheckClassifireReport/CheckClassifireReportToEmailOverDBProfile/'
        }).then(function (response) {

        }, function (response) {
            console.error('errorHandlerService.showResponseError = ' + response);
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };
}