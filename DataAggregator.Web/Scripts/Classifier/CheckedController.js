angular
    .module('DataAggregatorModule')
    .controller('CheckedController', [
        '$scope', '$window', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', CheckedController]);

function CheckedController($scope, $window, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.user = userService.getUser();

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
        $scope.Grid_Checked = uiGridCustomService.createGridClassMod($scope, "Grid_Checked");

        let cellTemplateHint = '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD}}</div>'

        $scope.Grid_Checked.Options.columnDefs = [
            { name: 'ClassifierId', field: 'Id', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'CertificateNumber', field: 'RegistrationCertificateNumber', filter: { condition: uiGridCustomService.condition } },
            { name: 'OFD_Sum', field: 'OFD_Sum_LastMonth',type:'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Audit_Sum', field: 'Audit_Sum_LastMonth', type:'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },

            { name: 'ToRetail', enableCellEdit: true, field: 'ToRetail', type: 'boolean' },
            { name: 'ToOFD', enableCellEdit: true, field: 'ToOFD', type: 'boolean' },
            { name: 'дробить по МНН', enableCellEdit: true, field: 'ToSplitMnn', type: 'boolean' },

            { name: 'на блокирование', enableCellEdit: true, field: 'ToBlockUsed', type: 'boolean' },

            { name: 'ci_comment', enableCellEdit: true, field: 'ci_comment', filter: { condition: uiGridCustomService.condition } },

            { name: 'TradeName', field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
            { name: 'DrugDescription', field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
            { name: 'INNGroup', field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },

            { name: 'DrugId', field: 'DrugId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT},
            { name: 'OwnerTradeMark', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
            { name: 'Packer', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
            { name: 'Used', field: 'Used', type: 'boolean', enableCellEdit: userService.isInRole("ClassifierUsed")},
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
           // $scope.Checked_search();
            return 1;
        });
    };
    $scope.Checked_search_AC = function () {
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
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
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
                        alert("Сохранил");
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
}