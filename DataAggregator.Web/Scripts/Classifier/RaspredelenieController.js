angular
    .module('DataAggregatorModule')
    .controller('RaspredelenieController', [
        '$scope', '$window', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', RaspredelenieController]);

function RaspredelenieController($scope, $window, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    
    $scope.user = userService.getUser();
    $scope.IsLoad = false;
    $scope.CurrentRaspredelenie = {};
    $scope.Raspredelenies = [];
    $scope.Tables = [];
    $scope.Raspredelenie_Init = function () {
        $scope.Title = "Распределение";
        $scope.Grid_Raspredelenie = uiGridCustomService.createGridClassMod($scope, "Grid_Raspredelenie", null, "Data_dblClick");
        $scope.Grid_Raspredelenie.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.condition } },

            { name: 'былоClassifierId',width:50, visible: true, field: 'ClassifierId_Before', headerCellClass: 'clBefore', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT},
            { name: 'былоTradeName', width: 100, enableCellEdit: false, field: 'TradeName_Before', headerCellClass: 'clBefore', filter: { condition: uiGridCustomService.condition } },
            { name: 'былоDrugDescription', width: 100, enableCellEdit: false, field: 'DrugDescription_Before', headerCellClass: 'clBefore', filter: { condition: uiGridCustomService.condition } },
            { name: 'былоINNGroup_Before', width: 100, enableCellEdit: false, field: 'INNGroup_Before', headerCellClass: 'clBefore', filter: { condition: uiGridCustomService.condition } },
            { name: 'былоOwnerTradeMark', width: 100, enableCellEdit: false, field: 'OwnerTradeMark_Before', headerCellClass: 'clBefore', filter: { condition: uiGridCustomService.condition } },
            { name: 'былоPacker', width: 100, enableCellEdit: false, field: 'Packer_Before', headerCellClass: 'clBefore', filter: { condition: uiGridCustomService.condition } },
            { name: 'былоUsed', width: 50, field: 'Used_Before', headerCellClass: 'clBefore', type: 'boolean' },
            { name: 'будетClassifierId', width: 50, visible: true, field: 'ClassifierId_After', headerCellClass: 'clAfter', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT},
            { name: 'будетTradeName', width: 100, enableCellEdit: false, field: 'TradeName_After', headerCellClass: 'clAfter', filter: { condition: uiGridCustomService.condition } },
            { name: 'будетDrugDescription', width: 100, enableCellEdit: false, field: 'DrugDescription_After', headerCellClass: 'clAfter', filter: { condition: uiGridCustomService.condition } },
            { name: 'будетINNGroup_After', width: 100, enableCellEdit: false, field: 'INNGroup_After', headerCellClass: 'clAfter', filter: { condition: uiGridCustomService.condition } },
            { name: 'будетOwnerTradeMark', width: 100, enableCellEdit: false, field: 'OwnerTradeMark_After', headerCellClass: 'clAfter', filter: { condition: uiGridCustomService.condition } },
            { name: 'будетPacker', width: 100, enableCellEdit: false, field: 'Packer_After', headerCellClass: 'clAfter', filter: { condition: uiGridCustomService.condition } },
            { name: 'будетUsed', width: 50, field: 'Used_After', headerCellClass: 'clAfter', type: 'boolean' },
            { name: 'Пользователь', enableCellEdit: false, field: 'UserName', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid_Raspredelenie.SetDefaults();

        $scope.Grid_Classifier = uiGridCustomService.createGridClassMod($scope, "Grid_Classifier", null, "Classifier_dblClick");
            $scope.Grid_Classifier.Options.columnDefs = [
                { name: 'ClassifierId', width: 50, visible: true, field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
                { name: 'CertificateNumber', width: 50, enableCellEdit: false, field: 'RegistrationCertificateNumber', filter: { condition: uiGridCustomService.condition } },
                { name: 'Торговое наименование', width: 100, cellTooltip: true, enableCellEdit: false, field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
                { name: 'ФВ + Ф + Д + К', width: 100, ecellTooltip: true, nableCellEdit: false, field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
                { name: 'Правообладатель', width: 100, cellTooltip: true, enableCellEdit: false, field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
               // { name: 'OwnerTradeMarkId', enableCellEdit: false, field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
                { name: 'МНН', width: 100, cellTooltip: true, enableCellEdit: false, field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
                { name: 'Упаковщик', width: 100, cellTooltip: true, enableCellEdit: false, field: 'Packer', filter: { condition: uiGridCustomService.condition } },
               // { name: 'PackerId', enableCellEdit: false, field: 'PackerId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
                { name: 'Used', width: 50, field: 'Used', type: 'boolean' }
            ];
            $scope.Grid_Classifier.SetDefaults();

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Raspredelenie/Raspredelenie_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.Raspredelenies, response.data.Data.Raspredelenies);

            $scope.Raspredelenies.forEach(function (item, i, arr) {
                item.Date_Begin = new Date(item.Date_Begin);
                item.Date_End = new Date(item.Date_End);
            });

            Array.prototype.push.apply($scope.Tables, response.data.Data.Tables);
            return 1;
        });
    };
    $scope.Raspredelenie_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Raspredelenie/Raspredelenie_search/',
                data: JSON.stringify({ RaspredelenieId: $scope.CurrentRaspredelenie.Id })
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.IsLoad = true;
                    $scope.Grid_Raspredelenie.Options.data = response.data.Data.result;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Raspredelenie_search = function () {
        if ($scope.Grid_Raspredelenie.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Raspredelenie_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Raspredelenie_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Raspredelenie_search_AC();
        }

    };

    $scope.Raspredelenie_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Raspredelenie/Raspredelenie_save/',
                data: JSON.stringify({
                    array: $scope.Grid_Raspredelenie.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        //$scope.Raspredelenie_search_AC();
                    }
                    else {
                        $scope.Grid_Raspredelenie.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Raspredelenie_Create = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Raspredelenie/Raspredelenie_Create/',
                data: JSON.stringify({
                    Name: $scope.CurrentRaspredelenie.Name,
                    TableId: $scope.CurrentRaspredelenie.Table.Id,
                    Date_Begin: $scope.CurrentRaspredelenie.Date_Begin,
                    Date_End: $scope.CurrentRaspredelenie.Date_End,
                    withRegion: 1
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        //$scope.Raspredelenie_search_AC();
                    }
                    else {
                        $scope.Grid_Raspredelenie.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Raspredelenie_Update = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Raspredelenie/Raspredelenie_Update/',
                data: JSON.stringify({
                    Raspredelenie_Id : $scope.CurrentRaspredelenie.Id
                })
            }).then(function (response) {
                $scope.Raspredelenie_search_AC();

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Raspredelenie_Close = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Raspredelenie/Raspredelenie_Close/',
                data: JSON.stringify({
                    Raspredelenie_Id: $scope.CurrentRaspredelenie.Id
                })
            }).then(function (response) {
                alert("Закрыто");

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    hotkeys.bindTo($scope).add({
        combo: 'shift+f',
        description: 'Поиск по справочнику',
        callback: function (event) {
            $scope.selectedText = commonService.getSelectionText();
            $scope.Classifier_Get(null, null, $scope.selectedText, null, $scope.selectedText, null, null, null);
        }
    });


    $scope.Classifier_Get = function (ClassifierId, RuNumber, TradeName, DrugDescription, INN, OwnerTradeMark, Packer, Used) {
        $scope.Grid_Classifier.Options.data = [];
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Classifier/GetExternalView_FULL/',
                data: JSON.stringify({ ClassifierId: ClassifierId, RuNumber: RuNumber, TradeName: TradeName, DrugDescription: DrugDescription, INN: INN, OwnerTradeMark:OwnerTradeMark, Packer:Packer, Used:Used})
            }).then(function (response) {
                    $scope.Grid_Classifier.Options.data = response.data;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Classifier_dblClick = function (field, rowEntity) {
        $scope.Grid_Raspredelenie.selectedRows().forEach(function (item) {
            $scope.Grid_Raspredelenie.GridCellsMod(item, "ClassifierId_After", rowEntity["ClassifierId"]);
            $scope.Grid_Raspredelenie.GridCellsMod(item, "TradeName_After", rowEntity["TradeName"]);
            $scope.Grid_Raspredelenie.GridCellsMod(item, "DrugDescription_After", rowEntity["DrugDescription"]);
            $scope.Grid_Raspredelenie.GridCellsMod(item, "INNGroup_After", rowEntity["INNGroup"]);
            $scope.Grid_Raspredelenie.GridCellsMod(item, "OwnerTradeMark_After", rowEntity["OwnerTradeMark"]);
            $scope.Grid_Raspredelenie.GridCellsMod(item, "Packer_After", rowEntity["Packer"]);

            $scope.Grid_Raspredelenie.GridCellsMod(item, "UserName", $scope.user.Fullname);
            $scope.Grid_Raspredelenie.GridCellsMod(item, "UserId", $scope.user.UserId);
        });
    };

    $scope.Data_dblClick = function (field, rowEntity) {
        if (field === "ClassifierId") {
            $scope.Classifier_Get(rowEntity.ClassifierId, null, null, null, null, null, null, null);
        };
        if (field === "TradeName_Before") {
            $scope.Classifier_Get(null, null, rowEntity.TradeName_Before, null, null, null, null, null);
        };
        if (field === "INNGroup_Before") {
            $scope.Classifier_Get(null, null, null, null, rowEntity.INNGroup_Before, null, null, null);
        };
        if (field === "DrugDescription_Before") {
            $scope.Classifier_Get(null, null, rowEntity.TradeName_Before, rowEntity.DrugDescription_Before, null, null, null, null);
        };
        if (field === "OwnerTradeMark_Before") {
            $scope.Classifier_Get(null, null, rowEntity.TradeName_Before, rowEntity.DrugDescription_Before, null, rowEntity.OwnerTradeMark_Before, null, null);
        };
    };
        
}