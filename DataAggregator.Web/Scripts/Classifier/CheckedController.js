angular
    .module('DataAggregatorModule')
    .controller('CheckedController', ['$scope', '$window', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', '$interval', '$uibModal', CheckedController])

function CheckedController($scope, $window, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService, $interval, $uibModal) {
    $scope.message = {};
    $scope.user = userService.getUser();

    var selectedRows = null;
    $scope.selectedCount = 0;

    $scope.currentTabIndex = 0;

    $scope.setTabIndex = function (index) {
        $scope.currentTabIndex = index;
    };

    $scope.showModal = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/Checkeds/CheckedsExceptionList.html',
            controller: 'CheckedController',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function (newValue) {
            $scope.loading = $http.post("/GoodsParametersEditor/AddParameterGroup/", { newValue: newValue, goodsCategoryId: $scope.goodsCategory.Id })
                .then(function (response) {
                    var selectedItemId = response.data.Data.Id;
                    $scope.getParameterGroups(selectedItemId);
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }, function () {
        });
    }

    $scope.Checked_Init = function () {

        hotkeys.bindTo($scope).add({
            combo: 'shift+d',
            description: 'Копировать описание',
            callback: function (event) {
                var cb = "";
                $scope.Grid_Checked.selectedRows().forEach(function (item) {
                    cb = item.TradeName + " " + item.DrugDescription + " " + item.OwnerTradeMark;
                });
                $scope.CopyToBuffer(cb);
                event.preventDefault();
            }
        });
        $scope.CopyToBuffer = function (value) {
            navigator.clipboard.writeText(value)
                .then(() => {
                    // Получилось!
                })
                .catch(err => {
                    console.log('Something went wrong', err);
                });
        }
        $scope.filter = {
            IsBrick: false, isOther: false, ToBlock: false
        }

        //--------------------
        // таблица Бессмертные
        //--------------------
        var Grid_Checked_onSelectionChanged = function (row) {
            selectedRows = $scope.Grid_Checked.gridApi.selection.getSelectedRows();
            $scope.selectedCount = selectedRows.length;
        }

        $scope.Grid_Checked = uiGridCustomService.createGridClassMod($scope, "Grid_Checked", Grid_Checked_onSelectionChanged);

        let cellTemplateHint = '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'

        $scope.Grid_Checked.Options.columnDefs = [
            { name: 'ClassifierId', field: 'Id', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'CertificateNumber', field: 'RegistrationCertificateNumber', filter: { condition: uiGridCustomService.condition } },
            { name: 'OFD_Sum', displayName: 'OFD Sum', field: 'OFD_Sum_LastMonth', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Audit_Sum', field: 'Audit_Sum_LastMonth', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },

            { name: 'ToRetail', enableCellEdit: true, field: 'ToRetail', type: 'boolean' },
            { name: 'ToOFD', displayName: 'To OFD', enableCellEdit: true, field: 'ToOFD', type: 'boolean' },
            { name: 'дробить по МНН', enableCellEdit: true, field: 'ToSplitMnn', width: 160, type: 'boolean' },
            { name: 'Проверено дробить по МНН', displayName: 'Проверено дробить по МНН', width: 240, enableCellEdit: true, field: 'ToSplitMnn_Signed', type: 'boolean' },

            { name: 'на блокирование', enableCellEdit: true, field: 'ToBlockUsed', type: 'boolean' },

            { name: 'ci_comment', enableCellEdit: true, field: 'ci_comment', filter: { condition: uiGridCustomService.condition } },

            { name: 'TradeName', field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
            { name: 'DrugDescription', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },

            { name: 'INNGroupId', field: 'INNGroupId', visible: false, type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'INNGroup', displayName: 'INNGroup', field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },

            { name: 'DrugId', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'OwnerTradeMark', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
            { name: 'Packer', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
            { name: 'Used', field: 'Used', type: 'boolean', enableCellEdit: userService.isInRole("ClassifierUsed") },
            { name: 'IsOther', field: 'IsOther', type: 'boolean' },
            { name: 'СТМ', field: 'IsSTM', type: 'boolean', enableCellEdit: true },
            { name: 'New', field: 'PriceNew', enableCellEdit: true, type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'LastWhen', field: 'LastWhen', type: 'date' },

            {
                name: 'OperatorComments',
                field: 'OperatorComments',
                filter: { condition: uiGridCustomService.condition },
                cellTemplate: cellTemplateHint
            }
        ];

        $scope.Grid_Checked.SetDefaults();

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Checked/Checked_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            return 1;
        });
        //--------------------
        // таблица Бессмертные
        //--------------------

        //----------------------------
        // таблица <Список исключений>
        //----------------------------
        $scope.Grid_CheckedExceptionList = uiGridCustomService.createGridClassMod($scope, "Grid_Checked");

        $scope.Grid_CheckedExceptionList.Options.columnDefs = [
            { name: 'Id', field: 'Id', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'Description', displayName: 'МНН', field: 'Description', filter: { condition: uiGridCustomService.condition }, width: 600 },
            { name: 'IsException', displayName: 'Исключение', field: 'IsException', type: 'boolean', enableCellEdit: true, width: 150 }
        ];

        $scope.Grid_CheckedExceptionList.SetDefaults();

        $scope.ExceptionListInit();
        //----------------------------
        // таблица <Список исключений>
        //----------------------------
    };

    $scope.Checked_search_AC = function () {

        $scope.message.caption = `Пожалуйста, ожидайте. Загрузка данных...`;
        $scope.message.params = [
            `На блокирование : ${$scope.filter.ToBlock ? 'Да' : 'Нет'}`,
            `ОФД без Аудита: ${$scope.filter.IsBrick ? 'Да' : 'Нет'}`,
            `ДОП: ${$scope.filter.isOther ? 'Да' : 'Нет'}`
        ]

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Checked/Checked_search/',
                data: JSON.stringify({ IsBrick: $scope.filter.IsBrick, isOther: $scope.filter.isOther, ToBlock: $scope.filter.ToBlock })
            }).then(function (response) {

                var data = response.data;

                if (data.Success) {
                    $scope.Grid_Checked.Options.data = data.Data.Checkeds;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Checked_search = function () {
        if ($scope.Grid_Checked.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранённые результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Checked_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Checked_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Checked_search_AC();
        }
    };

    $scope.Checked_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Checked/Checked_save/',
                data: JSON.stringify({
                    array_UPD: $scope.Grid_Checked.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.Checked_search_AC();
                    }
                    else {
                        $scope.Grid_Checked.ClearModify();
                        messageBoxService.showInfo("Сохранено записей: " + data.count);
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Checked_set_ci_comment = function (value) {
        $scope.Grid_Checked.selectedRows().forEach(function (item) {
            $scope.Grid_Checked.GridCellsMod(item, "ci_comment", value);

        });
    };
    $scope.ToBlockUsed = function (value) {
        $scope.Grid_Checked.selectedRows().forEach(function (item) {
            $scope.Grid_Checked.GridCellsMod(item, "ToBlockUsed", value);

        });
    };
    $scope.IsSTM = function (value) {
        $scope.Grid_Checked.selectedRows().forEach(function (item) {
            $scope.Grid_Checked.GridCellsMod(item, "IsSTM", value);

        });
    };

    // сохранение списка исключений
    $scope.ExceptionListSave = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Checked/ExceptionListSave/',
                data: JSON.stringify({
                    array_UPD: $scope.Grid_CheckedExceptionList.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;

                if (data.Success) {
                    messageBoxService.showInfo("Сохранено записей: " + $scope.Grid_CheckedExceptionList.GetArrayModify().length);

                    if (action === "search") {
                        $scope.ExceptionListInit();
                    }

                    $scope.Grid_CheckedExceptionList.ClearModify();
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    // обновить список исключений
    $scope.ExceptionListRefresh = function () {
        if ($scope.Grid_CheckedExceptionList.NeedSave === true) {

            messageBoxService.showConfirm('Есть несохранённые результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.ExceptionListSave("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.ExceptionListInit();
                        }
                        else {
                            var d = "отмена";
                        }
                    });

        }
        else {
            $scope.ExceptionListInit();
        }
    };

    // данные из списка исключений
    $scope.ExceptionListInit = function () {

        $scope.message.caption = `Пожалуйста, ожидайте. Загрузка списка исключений...`;
        $scope.message.params = [];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Checked/ExceptionListInit/',
            data: JSON.stringify({})
        }).then(function (response) {
            var data = response.data;

            if (data.Success) {
                $scope.Grid_CheckedExceptionList.Options.data = data.Data.ExceptionList;
            }
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });

    };

    // Множественная простановка всем "дробить по МНН", поле [Classifier].[ClassifierInfo].[ToSplitMnn]
    $scope.SplitByINN = function (value) {

        if (selectedRows === null) {
            return;
        }

        selectedRows.forEach(function (item) {
            item.ToSplitMnn = value;
            item["@modify"] = true;
        });

        $scope.Grid_Checked.NeedSave = true;

        return;
        /*
        let classifireIdArray = selectedRows.map(item => item.Id);

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Checked/SplitByINN',
            data: JSON.stringify({ array_UPD: classifireIdArray, value: value })
        }).then(function (response) {

            if (response.status === 200) {

                var data = response.data.Data.Data.ClassifierCheckedRecords;

                // корректируем оригинал
                data.forEach(el => {
                    var index = $scope.Grid_Checked.Options.data.findIndex(item => item.Id === el.Id);
                    $scope.Grid_Checked.Options.data[index].ToSplitMnn = el.ToSplitMnn;
                });

                messageBoxService.showInfo("Сохранено записей: " + data.length);

                $scope.Grid_Checked.gridApi.selection.clearSelectedRows();
                $scope.Grid_Checked.ClearModify();
            }

        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
        */
    };

    // Проставить (снять) проверку на дробление по МНН, поле [Classifier].[ClassifierInfo].[ToSplitMnn_Signed]
    $scope.SplitByINNSigned = function (value) {

        if (selectedRows === null) {
            return;
        }

        selectedRows.forEach(function (item) {
            item.ToSplitMnn_Signed = value;
            item["@modify"] = true;
        });

        $scope.Grid_Checked.NeedSave = true;

        return;
        /*
        let classifireIdArray = selectedRows.map(item => item.Id);

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Checked/SplitByINN_Signed',
            data: JSON.stringify({ array_UPD: classifireIdArray, value: value })
        }).then(function (response) {

            if (response.status === 200) {

                var data = response.data.Data.Data.ClassifierCheckedRecords;

                // корректируем оригинал
                data.forEach(el => {
                    var index = $scope.Grid_Checked.Options.data.findIndex(item => item.Id === el.Id);
                    $scope.Grid_Checked.Options.data[index].ToSplitMnn_Signed = el.ToSplitMnn_Signed;
                });

                messageBoxService.showInfo("Сохранено записей: " + data.length);

                $scope.Grid_Checked.gridApi.selection.clearSelectedRows();
                $scope.Grid_Checked.ClearModify();
            }

        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
        */
    };

    // Экспорт в Excel.
    // Выгружать в отчет все данные, которые на данный момент присутствуют в блоке бессмертные.
    $scope.Checked_To_Excel = function () {
        $scope.message.caption = `Пожалуйста, ожидайте. Идёт экспорт в excel всех данных, которые на данный момент присутствуют в блоке <Бессмертные>...`;
        $scope.message.params = [];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Checked/Checked_To_Excel/',
            data: "",
            headers: { 'Content-type': 'application/json' },
            responseType: 'arraybuffer'
        }).then(function (response) {
            
            var blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var fileName = 'Дробить по МНН.xlsx';
            saveAs(blob, fileName);

        }, function (error) {
            console.log(error);
            messageBoxService.showError('Rejected:' + error);
        });
    }
}