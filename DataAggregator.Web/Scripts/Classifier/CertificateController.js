angular
    .module('DataAggregatorModule')
    .controller('CertificateController', [
        '$scope', '$window', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'uiGridCustomService', 'errorHandlerService', 'uiGridConstants', 'formatConstants', 'userService', CertificateController]);

function CertificateController($scope, $window, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, uiGridCustomService, errorHandlerService, uiGridConstants, formatConstants, userService) {
    $scope.IsRowSelection = false;
    $scope.Title = "Справочник";
    $scope.user = userService.getUser();

    $scope.Certificates_Init = function () {
        $scope.searchText = $route.current.params["searchText"];
        //$window.document.title = "Справочник " + $scope.name;
        $scope.Grid_Certificates = uiGridCustomService.createGridClassMod($scope, "Grid_Certificates");
        $scope.Grid_Certificates.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: 'Номер', enableCellEdit: false, field: 'Number', filter: { condition: uiGridCustomService.condition } },
            { name: 'Номер2', enableCellEdit: false, field: 'Number_ID', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/Certificate?Id={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'Тип', enableCellEdit: false, field: 'type', filter: { condition: uiGridCustomService.condition } },
            { name: 'Взаимозаменяемый', enableCellEdit: false, field: 'Exchangeable', type: 'boolean' },
            { name: 'Референтный', enableCellEdit: false, field: 'Reference', type: 'boolean' },
            { name: 'Дата завершения', enableCellEdit: false, field: 'data_end', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Дата анулирования', enableCellEdit: false, field: 'data_Annul', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Срок годности', enableCellEdit: false, field: 'StorageLife', filter: { condition: uiGridCustomService.condition } },
            { name: 'Дата регистрации', enableCellEdit: false, field: 'date_registration', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Наименование держателя или владельца', enableCellEdit: false, field: 'Owner_Name', filter: { condition: uiGridCustomService.condition } },
            { name: 'Страна', enableCellEdit: false, field: 'Owner_Country', filter: { condition: uiGridCustomService.condition } },
            { name: 'Торговое наименование лекарственного препарата', enableCellEdit: false, field: 'TN', filter: { condition: uiGridCustomService.condition } },
            { name: 'МНН', enableCellEdit: false, field: 'INN', filter: { condition: uiGridCustomService.condition } },
            { name: 'Фармако-терапевтическая группа', enableCellEdit: false, field: 'FTG', filter: { condition: uiGridCustomService.condition } },
            { name: 'АТХ', enableCellEdit: false, field: 'ATC_WHO', filter: { condition: uiGridCustomService.condition } },
            { name: 'ЖНВЛП', enableCellEdit: false, field: 'ved', type: 'boolean' },
            { name: 'обновлён', enableCellEdit: false, field: 'last_update', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'обнаружен', enableCellEdit: false, field: 'last_control', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Статус', enableCellEdit: false, field: 'status', filter: { condition: uiGridCustomService.condition } },
            { name: 'Рецептурный отпуск', enableCellEdit: false, field: 'prescription', filter: { condition: uiGridCustomService.condition } }
        ];
        $scope.Grid_Certificates.SetDefaults();
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Certificate/Certificates_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            if ($scope.searchText !== "" && $scope.searchText !== undefined) {
                $scope.Certificates_search();
            }
            return 1;
        });
    };
    $scope.Certificates_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Certificate/Certificates_search/',
                data: JSON.stringify({ searchText: $scope.searchText })
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.Grid_Certificates.Options.data = response.data.Data.Certificates;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Certificates_search = function () {
        if ($scope.Grid_Certificates.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Certificates_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Certificates_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Certificates_search_AC();
        }

    };
    $scope.Certificate_Init = function () {
        $scope.Id = $route.current.params["Id"];
        $scope.Grid_Certificate = uiGridCustomService.createGridClassMod($scope, "Grid_Certificate");
        $scope.Grid_Certificate.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: 'Номер', enableCellEdit: false, field: 'Number', filter: { condition: uiGridCustomService.condition } },
            { name: 'Номер2', enableCellEdit: false, field: 'Number_ID', filter: { condition: uiGridCustomService.condition } },
            { name: 'Взаимозаменяемый', enableCellEdit: false, field: 'Exchangeable', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' },
            { name: 'Референтный', enableCellEdit: false, field: 'Reference', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' },
            { name: 'Дата завершения', enableCellEdit: false, field: 'data_end', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Дата анулирования', enableCellEdit: false, field: 'data_Annul', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Срок годности', enableCellEdit: false, field: 'StorageLife', filter: { condition: uiGridCustomService.condition } },
            { name: 'Дата регистрации', enableCellEdit: false, field: 'date_registration', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Наименование держателя или владельца', enableCellEdit: false, field: 'Owner_Name', filter: { condition: uiGridCustomService.condition } },
            { name: 'Страна', enableCellEdit: false, field: 'Owner_Country', filter: { condition: uiGridCustomService.condition } },
            { name: 'Торговое наименование лекарственного препарата', enableCellEdit: false, field: 'TN', filter: { condition: uiGridCustomService.condition } },
            { name: 'МНН', enableCellEdit: false, field: 'INN', filter: { condition: uiGridCustomService.condition } },
            { name: 'Фармако-терапевтическая группа', enableCellEdit: false, field: 'FTG', filter: { condition: uiGridCustomService.condition } },
            { name: 'АТХ', enableCellEdit: false, field: 'ATC_WHO', filter: { condition: uiGridCustomService.condition } },
            { name: 'ЖНВЛП', enableCellEdit: false, field: 'ved', filter: { condition: uiGridCustomService.booleanConditionX }, type: 'boolean' },
            { name: 'обновлён', enableCellEdit: false, field: 'last_update', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'обнаружен', enableCellEdit: false, field: 'last_control', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Статус', enableCellEdit: false, field: 'status', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.Grid_FV = uiGridCustomService.createGridClassMod($scope, "Grid_FV");
        $scope.Grid_FV.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: 'Лекарственная форма', enableCellEdit: false, field: 'LekForm', filter: { condition: uiGridCustomService.condition } },
            { name: 'Дозировка', enableCellEdit: false, field: 'Dosage', filter: { condition: uiGridCustomService.condition } },
            { name: 'Срок годности', enableCellEdit: false, field: 'ExpirationDate', filter: { condition: uiGridCustomService.condition } },
            { name: 'Условия хранения', enableCellEdit: false, field: 'StorageCondition', filter: { condition: uiGridCustomService.condition } },
            { name: 'Упаковки', enableCellEdit: false, field: 'FormV', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.Grid_ManufactureWay = uiGridCustomService.createGridClassMod($scope, "Grid_ManufactureWay");
        $scope.Grid_ManufactureWay.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: '№ п/п', enableCellEdit: false, field: 'Np', filter: { condition: uiGridCustomService.condition } },
            { name: 'Стадия производства', enableCellEdit: false, field: 'Stage', filter: { condition: uiGridCustomService.condition } },
            { name: 'Производитель', enableCellEdit: false, field: 'Manufacturer', filter: { condition: uiGridCustomService.condition } },
            { name: 'Адрес производителя', enableCellEdit: false, field: 'Address', filter: { condition: uiGridCustomService.condition } },
            { name: 'Страна', enableCellEdit: false, field: 'Country', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.Grid_SubstRaw = uiGridCustomService.createGridClassMod($scope, "Grid_SubstRaw");
        $scope.Grid_SubstRaw.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: 'Международное непатентованное или группировочное или химическое наименование', enableCellEdit: false, field: 'INN', filter: { condition: uiGridCustomService.condition } },
            { name: 'Торг. наим.', enableCellEdit: false, field: 'INN2', filter: { condition: uiGridCustomService.condition } },
            { name: 'Производитель', enableCellEdit: false, field: 'Manufacturer', filter: { condition: uiGridCustomService.condition } },
            { name: 'Адрес', enableCellEdit: false, field: 'Address', filter: { condition: uiGridCustomService.condition } },
            { name: 'Срок годности', enableCellEdit: false, field: 'ExpirationDate', filter: { condition: uiGridCustomService.condition } },
            { name: 'Условия хранения', enableCellEdit: false, field: 'StorageCondition', filter: { condition: uiGridCustomService.condition } },
            { name: 'Фармакоп. статья / Номер НД', enableCellEdit: false, field: 'NumberND', filter: { condition: uiGridCustomService.condition } },
            { name: 'Входит в перечень нарк. средств, псих. веществ и их прекурсоров', enableCellEdit: false, field: 'Nark', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Certificate/Certificate_Init/',
            data: JSON.stringify({ Id: $scope.Id })
        }).then(function (response) {
            if (response.data.Success) {
                $scope.Grid_Certificate.Options.data = response.data.Data.Certificate;
                $scope.Grid_FV.Options.data = response.data.Data.FV;
                $scope.Grid_ManufactureWay.Options.data = response.data.Data.ManufactureWay;
                $scope.Grid_SubstRaw.Options.data = response.data.Data.SubstRaw;
            }
        });
    };




    $scope.AddChemicals = function (event) {
        if ($scope.CurrentId > 0) {
            var new_element = {
                Id: 0,
                NumberINN_NewId: $scope.CurrentId,
                INN: $scope.CurrentINN,
                LekForm: $scope.CurrentLekForm,
                ChemicalSPRId: 0,
                Value: "",
                Description: ""
            };
            new_element["@modify"] = true;
            $scope.Grid_Chemicals.NeedSave = true;

            $scope.Grid_Chemicals.Options.data.push(new_element);
        }
        if (event) {
            event.preventDefault();
        }
    };
    $scope.DeleteChemicals = function (event) {

        $scope.Grid_Chemicals.selectedRows().forEach(function (item) {
            $scope.Grid_Chemicals.GridCellsMod(item, "Id", -1 * item["Id"]);
        });
        if (event) {
            event.preventDefault();
        }
    };
    $scope.CopySubstanceId = function (event) {
        var row_sel = $scope.Grid_Substance.selectedRows();
        if (row_sel.length === 1) {
            $scope.copyId_new = row_sel[0]["Id_new"];
        }
        if (event) {
            event.preventDefault();
        }
    };
    $scope.PasteSubstanceId_new = function (event) {
        if ($scope.copyId_new > 0) {
            $scope.Grid_Substance.selectedRows().forEach(function (item) {
                $scope.Grid_Substance.GridCellsMod(item, "Id_new", $scope.copyId_new);
            });
        }
        if (event) {
            event.preventDefault();
        }
    };
    $scope.SetSubstanceId_restore = function (event) {
        $scope.Grid_Substance.selectedRows().forEach(function (item) {
            $scope.Grid_Substance.GridCellsMod(item, "Id_new", item["Id"]);
        });
        if (event) {
            event.preventDefault();
        }
    };
    $scope.SetSubstanceId_New_IsMain = function (event) {
        var isMainId = 0;
        $scope.Grid_Substance.selectedRows().forEach(function (item) {
            if (item["Id_new"] === item["Id"])
                isMainId = item["Id_new"];
        });
        if (isMainId > 0) {
            $scope.Grid_Substance.selectedRows().forEach(function (item) {
                $scope.Grid_Chemicals.GridCellsMod(item, "Id_new", isMainId);
            });
        }
        if (event) {
            event.preventDefault();
        }
    };


    $scope.Substance_Init = function () {
        $scope.CurrentId = 0;
        $scope.CurrentINN = "";
        $scope.CurrentLekForm = "";
        $scope.copyId_new = 0;
        $scope.ChemicalSPR = [];
        hotkeys.bindTo($scope).add({
            combo: 'shift+e',
            description: 'Добавить субстанцию',
            callback: function (event) {
                $scope.AddChemicals(event);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+d',
            description: 'Удалить субстанцию',
            callback: function (event) {
                $scope.DeleteChemicals(event);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+c',
            description: 'копировать id',
            callback: function (event) {
                $scope.CopySubstanceId(event);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+v',
            description: 'вставить id в id_new',
            callback: function (event) {
                $scope.PasteSubstanceId_new(event);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+z',
            description: 'вставить в id_new значение из Id',
            callback: function (event) {
                $scope.SetSubstanceId_restore(event);
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+x',
            description: 'вставить в id_new значение из isMain',
            callback: function (event) {
                $scope.SetSubstanceId_New_IsMain(event);
            }
        });
        $scope.Grid_Substance = uiGridCustomService.createGridClassMod($scope, "Grid_Substance", onSelectionChanged);
        $scope.Grid_Substance.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
            { name: 'Номер', enableCellEdit: false, field: 'Number', filter: { condition: uiGridCustomService.condition } },
            { name: 'МНН', enableCellEdit: false, field: 'INN', filter: { condition: uiGridCustomService.condition } },
            { name: 'Лекарственная форма', enableCellEdit: false, field: 'LekForm', filter: { condition: uiGridCustomService.condition } },
            { name: 'Id_new', field: 'Id_new', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
            { name: 'Основной', enableCellEdit: false, field: 'isMain', type: 'boolean' },
            { name: 'в формулах', enableCellEdit: false, field: 'Chemicals_Count', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
            { name: 'в классификаторе', enableCellEdit: false, field: 'Classifier_Count', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' }
        ];
        $scope.Grid_Substance.SetDefaults();

        $scope.Grid_Chemicals = uiGridCustomService.createGridClassMod($scope, "Grid_Chemicals");
        $scope.Grid_Chemicals.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
            { name: 'РУМННид', enableCellEdit: false, field: 'NumberINN_NewId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
            { name: 'МНН', enableCellEdit: false, field: 'INN', filter: { condition: uiGridCustomService.condition } },
            { name: 'Лекарственная форма', enableCellEdit: false, field: 'LekForm', filter: { condition: uiGridCustomService.condition } },
            { name: 'МННид', enableCellEdit: false, field: 'ChemicalSPRId', filter: { condition: uiGridCustomService.numberCondition }, type: 'number' },
            {
                name: 'Химическое наименование сокращённое', enableCellEdit: true, field: 'Value', filter: { condition: uiGridCustomService.condition },
                allowCellFocus: false, enableCellEditOnFocus: true,
                editableCellTemplate: '<div><form name="inputForm"><input type="text" ng-model="MODEL_COL_FIELD"  uib-typeahead="data.Value for data in grid.appScope.ChemicalSPR | filter:$viewValue | limitTo:8" typeahead-on-select="grid.appScope.setSPRId($item,row.entity)" ng-change="grid.appScope.ChangeSPRId(row.entity)" class="typeahead form-control"></form></div>'
                //<input type="text" ng-model="CurrentLekForm" uib-typeahead="data.Value for data in ChemicalSPR | filter:$viewValue | limitTo:12" class="form-control">
                //onblur="uiGridEventEndCellEdit()" проблемка при выборе в выпадающем это тоже потеря фокуса и он это делает раньше выбора
            },
            { name: 'Химическое наименование полное', enableCellEdit: true, field: 'Description', filter: { condition: uiGridCustomService.condition } }
        ];

        $scope.Search();
    };
    uiGridEventEndCellEdit = function () {
        $scope.$broadcast("uiGridEventEndCellEdit");
        //END_CELL_EDIT
    };
    $scope.setSPRId = function (dictionaryItem, item) {
        item.ChemicalSPRId = dictionaryItem.Id;
        item.Value = dictionaryItem.Value;
        item.Description = dictionaryItem.Description;

        //END_CELL_EDIT
        uiGridEventEndCellEdit();
    };
    $scope.ChangeSPRId = function (item) {
        //  $scope.$emit(uiGridEditConstants.events.END_CELL_EDIT);
        $scope.Grid_Chemicals.GridCellsMod(item, "ChemicalSPRId", 0)
    };
    $scope.Search = function () {
        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Certificate/Substance_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            if (response.data.Success) {
                $scope.Grid_Substance.Options.data = response.data.Data.Substance;
                $scope.Grid_Chemicals.Options.data = response.data.Data.Chemicals;
                while ($scope.ChemicalSPR.length > 0) {
                    $scope.ChemicalSPR.pop();
                } // Fastest
                Array.prototype.push.apply($scope.ChemicalSPR, response.data.Data.ChemicalSPR);
                if ($scope.CurrentId === 0) {
                    onSelectionChanged($scope.Grid_Substance.Options.data[0]);
                }
            }
        });
    };
    function onSelectionChanged(row) {
        uiGridEventEndCellEdit();
        if (row !== undefined) {
            if (row.entity === undefined) {
                $scope.CurrentId = row["Id_new"];
                $scope.CurrentINN = row["INN"];
                $scope.CurrentLekForm = row["LekForm"];
            }
            else {
                $scope.CurrentId = row.entity["Id_new"];
                $scope.CurrentINN = row.entity["INN"];
                $scope.CurrentLekForm = row.entity["LekForm"];
            }
            //$scope.Grid_Chemicals.ClearFilters();
            $scope.Grid_Chemicals.FilterSet("NumberINN_NewId", $scope.CurrentId);
            $scope.Grid_Chemicals.FilterSet("Id", ">-1");
        }
    }

    $scope.Substance_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Certificate/Substance_save/',
                data: JSON.stringify({
                    array_Substance: $scope.Grid_Substance.GetArrayModify(),
                    array_Chemicals: $scope.Grid_Chemicals.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        //$scope.Search_AC();
                    }
                    else {
                        $scope.Grid_Substance.ClearModify();
                        $scope.Grid_Chemicals.ClearModify();
                        //$scope.Search_AC();
                        $scope.Search();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.Classifier_Grid_create = function () {
        $scope.Grid_Classifier = uiGridCustomService.createGridClassMod($scope, "Grid_Classifier", null, "Classifier_dblClick");
        if ($scope.TypeSet === "MnfWay") {
            $scope.Grid_Classifier.Options.columnDefs = [
                { name: 'ClassifierId', visible: true, field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
                { name: 'CertificateNumber', enableCellEdit: false, field: 'RegistrationCertificateNumber', filter: { condition: uiGridCustomService.condition } },
                { name: 'Торговое наименование', cellTooltip: true, enableCellEdit: false, field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
                { name: 'ФВ + Ф + Д + К', cellTooltip: true, enableCellEdit: false, field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },
                { name: 'Правообладатель', cellTooltip: true, enableCellEdit: false, field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
                { name: 'OwnerTradeMarkId', enableCellEdit: false, field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
                { name: 'МНН', cellTooltip: true, enableCellEdit: false, field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
                { name: 'Упаковщик', cellTooltip: true, enableCellEdit: false, field: 'Packer', filter: { condition: uiGridCustomService.condition } },
                { name: 'PackerId', enableCellEdit: false, field: 'PackerId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
                { name: 'Used', field: 'Used', type: 'boolean' }
            ];
            $scope.Grid_Classifier.SetDefaults();
        }
        if ($scope.TypeSet === "ESKLP") {
            $scope.Grid_Classifier.Options.columnDefs = [
                { name: 'ClassifierId', visible: true, field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
                { name: 'ClassifierPackingId', visible: true, field: 'ClassifierPackingId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
                { name: 'в ЕСКЛП', visible: true, field: 'ESKLPCount', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
                { name: 'CertificateNumber', enableCellEdit: false, field: 'RegistrationCertificateNumber', filter: { condition: uiGridCustomService.condition } },
                { name: 'Торговое наименование', cellTooltip: true, enableCellEdit: false, field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
                { name: 'ФВ + Ф + Д + К', cellTooltip: true, enableCellEdit: false, field: 'DrugDescription', filter: { condition: uiGridCustomService.condition } },

                { name: 'Первичная упаковка', cellTooltip: true, enableCellEdit: false, field: 'PrimaryPacking', filter: { condition: uiGridCustomService.condition } },
                { name: 'Количество в первичной упаковке', cellTooltip: true, enableCellEdit: true, field: 'CountInPrimaryPacking', filter: { condition: uiGridCustomService.numberCondition } },
                { name: 'Потребительская упаковка', cellTooltip: true, enableCellEdit: false, field: 'ConsumerPacking', filter: { condition: uiGridCustomService.condition } },
                { name: 'Количество первичных упаковок', cellTooltip: true, enableCellEdit: true, field: 'CountPrimaryPacking', filter: { condition: uiGridCustomService.numberCondition } },
                { name: 'Комплектность', cellTooltip: true, enableCellEdit: true, field: 'PackingDescription', filter: { condition: uiGridCustomService.condition } },
                { name: 'Фасовка', visible: true, field: 'ConsumerPackingCount', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },

                { name: 'Правообладатель', cellTooltip: true, enableCellEdit: false, field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
                { name: 'OwnerTradeMarkId', enableCellEdit: false, field: 'OwnerTradeMarkId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
                { name: 'МНН', cellTooltip: true, enableCellEdit: false, field: 'INNGroup', filter: { condition: uiGridCustomService.condition } },
                { name: 'Упаковщик', cellTooltip: true, enableCellEdit: false, field: 'Packer', filter: { condition: uiGridCustomService.condition } },
                { name: 'PackerId', enableCellEdit: false, field: 'PackerId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
                { name: 'Used', field: 'Used', type: 'boolean' }
            ];
            $scope.Grid_Classifier.SetDefaults();
            $scope.Grid_Classifier.afterCellEdit = $scope.Grid_Classifier_afterCellEdit;
        }
    };
    $scope.Grid_Classifier_afterCellEdit = function (rowEntity, colDef, newValue, oldValue) {
        if (colDef.field === "PrimaryPacking" || colDef.field === "ConsumerPacking" || colDef.field === "PackingDescription") {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/Certificate/Classifier_' + colDef.field+'_Set/',
                    data: JSON.stringify({ ClassifierPackingId: rowEntity.ClassifierPackingId, newValue: newValue })
                }).then(function (response) {

                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        };
    };
    $scope.ClassifierFilter = {
RuNumber:"none"
    };
    $scope.Classifier_Get = function (RuNumber) {
        $scope.ClassifierFilter.RuNumber = RuNumber;
        $scope.Classifier_Search();
    }
    $scope.Classifier_Search = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Certificate/Classifier_search/',
                data: JSON.stringify({ TypeSet: $scope.TypeSet, RuNumber: $scope.ClassifierFilter.RuNumber })
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.Grid_Classifier.Options.data = response.data.Data.classifier;
                    if ($scope.TypeSet === "ESKLP") {
                        ESKLP_LightClassrow();
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.Classifier_dblClick = function (field, rowEntity) {
        if ($scope.TypeSet === "MnfWay") {
            $scope.Grid_MnfWay.selectedRows().forEach(function (item) {
                $scope.Grid_MnfWay.GridCellsMod(item, "PackerId", rowEntity["PackerId"]);
                $scope.Grid_MnfWay.GridCellsMod(item, "PackerValue", rowEntity["Packer"]);
            });
        }
        if ($scope.TypeSet === "ESKLP" && field !== "CountInPrimaryPacking" && field !== "CountPrimaryPacking") {
            $scope.Grid_ESKLP.selectedRows().forEach(function (item) {
                $scope.Grid_ESKLP.GridCellsMod(item, "ClassifierId", rowEntity["ClassifierId"]);
                $scope.Grid_ESKLP.GridCellsMod(item, "ClassifierPackingId", rowEntity["ClassifierPackingId"]);
            });
        }
    };

    $scope.MnfWay_Init = function () {
        hotkeys.bindTo($scope).add({
            combo: 'shift+v',
            description: 'вставить ManufacturerClearValue',
            callback: function (event) {
                navigator.clipboard.readText()
                    .then(text => {
                        // `text` содержит текст, прочитанный из буфера обмена
                        $scope.Grid_MnfWay.selectedRows().forEach(function (item) {
                            $scope.Grid_MnfWay.GridCellsMod(item, "ManufacturerClearId", 0);
                            $scope.Grid_MnfWay.GridCellsMod(item, "ManufacturerClearValue", text);
                        });
                    })
                    .catch(err => {
                        // возможно, пользователь не дал разрешение на чтение данных из буфера обмена
                        console.log('Something went wrong', err);
                    });
                $scope.Grid_Network.gridApi.core.refresh();
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+z',
            description: 'Очистить',
            callback: function (event) {
                $scope.Grid_MnfWay.selectedRows().forEach(function (item) {
                    $scope.Grid_MnfWay.GridCellsMod(item, "Status", 0);
                    $scope.Grid_MnfWay.GridCellsMod(item, "PackerId", null);
                    $scope.Grid_MnfWay.GridCellsMod(item, "PackerValue", "");
                });
            }
        });
        hotkeys.bindTo($scope).add({
            combo: 'shift+x',
            description: 'Cвязки не будет',
            callback: function (event) {
                $scope.Grid_MnfWay.selectedRows().forEach(function (item) {
                    $scope.Grid_MnfWay.GridCellsMod(item, "Status", 5);
                });
            }
        });
        $scope.TypeSet = "MnfWay";
        $scope.IsEmpty = false;
        $scope.IsOnlyEX = false;
        $scope.ManufacturerClear = [];
        $scope.Grid_MnfWay = uiGridCustomService.createGridClassMod($scope, "Grid_MnfWay");
        $scope.Grid_MnfWay.Options.columnDefs = [
            { name: 'Id', visible: false, field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: 'Номер', width: 100, enableCellEdit: false, field: 'Number', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'Номер', field: 'Number', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.Classifier_Get(row.entity.Number)">{{COL_FIELD}}</label></div>'
            },
            { name: 'Номер2', width: 100, enableCellEdit: false, field: 'Number_ID', filter: { condition: uiGridCustomService.condition }, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/Certificate?Id={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'Тип', width: 100, enableCellEdit: false, field: 'CertStatus', filter: { condition: uiGridCustomService.condition } },
            { name: 'Статус', width: 100, enableCellEdit: false, field: 'CertType', filter: { condition: uiGridCustomService.condition } },
            { name: 'Торговое наименование лекарственного препарата', width: 100, cellTooltip: true, enableCellEdit: false, field: 'TN', filter: { condition: uiGridCustomService.condition } },
            { name: 'МНН', width: 100, cellTooltip: true, enableCellEdit: false, field: 'INN', filter: { condition: uiGridCustomService.condition } },
            { name: 'Np', width: 100, enableCellEdit: false, field: 'Np', filter: { condition: uiGridCustomService.condition } },
            { name: 'Stage', width: 100, cellTooltip: true, enableCellEdit: false, field: 'Stage', filter: { condition: uiGridCustomService.condition } },
            { name: 'Наименование держателя или владельца', width: 100, cellTooltip: true, enableCellEdit: false, field: 'Manufacturer', filter: { condition: uiGridCustomService.condition } },
            { name: 'ManufacturerClearId', visible: false, enableCellEdit: false, field: 'ManufacturerClearId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'ManufacturerClearValue', width: 100, cellTooltip: true, enableCellEdit: true, field: 'ManufacturerClearValue', filter: { condition: uiGridCustomService.condition } },
            { name: 'Адресс', width: 100, cellTooltip: true, enableCellEdit: false, field: 'Address', filter: { condition: uiGridCustomService.condition } },
            { name: 'Страна', width: 100, cellTooltip: true, enableCellEdit: false, field: 'Country', filter: { condition: uiGridCustomService.condition } },
            { name: 'PackerId', enableCellEdit: false, field: 'PackerId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'PackerValue', width: 100, cellTooltip: true, enableCellEdit: false, field: 'PackerValue', filter: { condition: uiGridCustomService.condition } },
            { name: 'Status', enableCellEdit: false, field: 'Status', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT }
        ];
        $scope.Classifier_Grid_create();

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Certificate/MnfWay_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.ManufacturerClear, response.data.Data.ManufacturerClear);
            if ($scope.searchText !== "" && $scope.searchText !== undefined) {
                $scope.MnfWay_search();
            }
            return 1;
        });
    };
    $scope.MnfWay_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Certificate/MnfWay_search/',
                data: JSON.stringify({ IsEmpty: $scope.IsEmpty, IsOnlyEX: $scope.IsOnlyEX })
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.Grid_MnfWay.Options.data = response.data.Data.MnfWay;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.MnfWay_search = function () {
        if ($scope.Grid_MnfWay.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.MnfWay_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.MnfWay_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.MnfWay_search_AC();
        }

    };
    $scope.MnfWay_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Certificate/MnfWay_save/',
                data: JSON.stringify({
                    array_MnfWay: $scope.Grid_MnfWay.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.MnfWay_search_AC();
                    }
                    else {
                        $scope.Grid_MnfWay.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };




    $scope.ESKLP_Init = function () {
        $scope.Equipment = [];
        $scope.TypeSet = "ESKLP";
        $scope.IsEmpty = false;
        $scope.IsOnlyEX = false;
        $scope.Text = "";
        $scope.CurrentRowESKLP = null;
        $scope.ManufacturerClear = [];


        $scope.Grid_ESKLP = uiGridCustomService.createGridClassMod($scope, "Grid_ESKLP", ESKLP_onSelectionChanged);
        $scope.Grid_ESKLP.Options.columnDefs = [
            { name: 'Код КЛП', visible: true, field: 'Id', filter: { condition: uiGridCustomService.condition } },
            { name: 'Актуальный', field: 'IsActual', type: 'boolean' },
            { name: 'Торговое наименование', width: 100, cellTooltip: true, enableCellEdit: false, field: 'TradeName', filter: { condition: uiGridCustomService.condition } },
            {
                headerTooltip: true, enableCellEdit: false, width: 100, name: 'РУ Номер', field: 'RuNumber', headerCellClass: 'Green', filter: { condition: uiGridCustomService.condition },
                cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><label ng-click="grid.appScope.Classifier_Get(row.entity.RuNumber)">{{COL_FIELD}}</label></div>'
            },
            { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'ClassifierPackingId', enableCellEdit: false, field: 'ClassifierPackingId', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT },
            { name: 'статус', width: 100, cellTooltip: true, enableCellEdit: false, field: 'FlagSpr.Value', filter: { condition: uiGridCustomService.condition } },
            // { name: 'Стандартизованное МНН', width: 100, cellTooltip: true, enableCellEdit: false, field: 'StandardINN', filter: { condition: uiGridCustomService.condition } },
            // { name: 'Стандартизованная лекарственная форма и дозировка', width: 100, cellTooltip: true, enableCellEdit: false, field: 'StandardFV', filter: { condition: uiGridCustomService.condition } },
            { name: 'Нормализованное МНН', width: 100, cellTooltip: true, enableCellEdit: false, field: 'NormINN', filter: { condition: uiGridCustomService.condition } },
            { name: 'Нормализованная лекарственная форма', width: 100, cellTooltip: true, enableCellEdit: false, field: 'NormFV', filter: { condition: uiGridCustomService.condition } },
            { name: 'Нормализованная дозировка', width: 100, cellTooltip: true, enableCellEdit: false, field: 'NormDosage', filter: { condition: uiGridCustomService.condition } },
            { name: 'Наименование единицы измерения лекарственного препарата', width: 100, cellTooltip: true, enableCellEdit: false, field: 'EI', filter: { condition: uiGridCustomService.condition } },
            { name: 'Кол-во ЕИ ЛП  во вторичной (потребительской) упаковке', enableCellEdit: false, field: 'AmountEISecond', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Первичная упаковка Кол-во лекарственной формы', enableCellEdit: false, field: 'FirstAmount', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Первичная упаковка Наименование', width: 100, cellTooltip: true, enableCellEdit: false, field: 'FirstFV', filter: { condition: uiGridCustomService.condition } },
            { name: 'Вторичная (потребительская) упаковка Кол-во первичных упаковок', enableCellEdit: false, field: 'SecondAmount', type: 'number', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_PRICE },
            { name: 'Вторичная (потребительская) упаковка Наименование', width: 100, cellTooltip: true, enableCellEdit: false, field: 'SecondFV', filter: { condition: uiGridCustomService.condition } },
            { name: 'Комплектность вторичной (потребительской) упаковки', width: 100, cellTooltip: true, enableCellEdit: false, field: 'SecondFVAdd', filter: { condition: uiGridCustomService.condition } },
            { name: 'РУ Владелец регистрационного удостоверения', width: 100, cellTooltip: true, enableCellEdit: false, field: 'RuOwner', filter: { condition: uiGridCustomService.condition } },
            { name: 'Выпущены в ГО', field: 'IsGo', type: 'boolean', enableCellEdit: false },
            //  { name: 'РУ Страна', width: 100, cellTooltip: true, enableCellEdit: false, field: 'RuCountry', filter: { condition: uiGridCustomService.condition } },
            //  { name: 'РУ Дата получения', field: 'RuDate', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            { name: 'Производитель Наименование', width: 100, cellTooltip: true, enableCellEdit: false, field: 'Manufacturer', filter: { condition: uiGridCustomService.condition } }
            //  { name: 'Производитель Страна', width: 100, cellTooltip: true, enableCellEdit: false, field: 'ManufacturerCountry', filter: { condition: uiGridCustomService.condition } },
            //  { name: 'Производитель Адрес', width: 100, cellTooltip: true, enableCellEdit: false, field: 'ManufacturerAddress', filter: { condition: uiGridCustomService.condition } },
            //  { name: 'ЖНВЛП', field: 'IsVed', type: 'boolean'},
            //  { name: 'Наличие в лекарственном препарате наркотических средств, психотропных веществ и их прекурсоров', field: 'IsNark', type: 'boolean'},
            //  { name: 'Период действия КЛП Начало', field: 'DateStart', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            //  { name: 'Период действия КЛП Окончание', field: 'DateEnd', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            // { name: 'Дата изменения записи', field: 'DateMod', filter: { condition: uiGridCustomService.condition }, type: 'date', cellFilter: formatConstants.FILTER_DATE },
            // { name: 'Предельная отпускная цена на позицию КЛП', width: 100, enableCellEdit: false, field: 'MaxPrice', filter: { condition: uiGridCustomService.condition } },
            // { name: 'Некорректные данные', width: 100, enableCellEdit: false, field: 'AddText', filter: { condition: uiGridCustomService.condition } },
        ];
        $scope.Grid_ESKLP.Options.noUnselect = false;
        $scope.Grid_ESKLP.SetDefaults();
        $scope.Classifier_Grid_create();

        $scope.dataLoading = $http({
            method: 'POST',
            url: '/Certificate/ESKLP_Init/',
            data: JSON.stringify({})
        }).then(function (response) {
            Array.prototype.push.apply($scope.Equipment, response.data.Data.Equipment);
            if ($scope.searchText !== "" && $scope.searchText !== undefined) {
                $scope.ESKLP_search();
            }
            return 1;
        });
    };
    $scope.ESKLP_reset = function () {
        $scope.Grid_ESKLP.selectedRows().forEach(function (item) {
            $scope.Grid_ESKLP.GridCellsMod(item, "ClassifierId", null);
            $scope.Grid_ESKLP.GridCellsMod(item, "ClassifierPackingId", null);
            $scope.ESKLP_setFlag1(0, item);
        });
    }
    $scope.ESKLP_setFlagAll = function (Id) {
        $scope.Grid_ESKLP.selectedRows().forEach(function (item) {
            $scope.ESKLP_setFlag1(Id, item);
        });
    };
    $scope.ESKLP_setFlag1 = function (Id, item) {
        $scope.Grid_ESKLP.GridCellsMod(item, "Flag", Id);

        Value = "?";
        if (Id === 0)
            Value = "в обработку";
        if (Id === 5) {
            Value = "нет в реестре/ГО";
            $scope.Grid_ESKLP.GridCellsMod(item, "ClassifierId", null);
            $scope.Grid_ESKLP.GridCellsMod(item, "ClassifierPackingId", null);
        }
        FlagSpr = { Id: Id, Value: Value };
        $scope.Grid_ESKLP.GridCellsMod(item, "FlagSpr", FlagSpr);
    };

    $scope.ESKLPIsActual = function (Value) {
            $scope.Grid_ESKLP.selectedRows().forEach(function (item) {
                $scope.Grid_ESKLP.GridCellsMod(item, "IsActual", Value);
            });
    };
    $scope.ESKLP_search_AC = function () {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Certificate/ESKLP_search/',
                data: JSON.stringify({ IsEmpty: $scope.IsEmpty, IsOnlyEX: $scope.IsOnlyEX, Text: $scope.Text })
            }).then(function (response) {
                if (response.data.Success) {
                    $scope.Grid_ESKLP.Options.data = response.data.Data.ESKLP;
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.ESKLP_search = function () {
        if ($scope.Grid_ESKLP.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.ESKLP_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.ESKLP_search_AC();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.ESKLP_search_AC();
        }

    };
    $scope.ESKLP_save = function (action) {
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Certificate/ESKLP_save/',
                data: JSON.stringify({
                    array_ESKLP: $scope.Grid_ESKLP.GetArrayModify(),
                    array_ClassifierPacking: $scope.Grid_Classifier.GetArrayModify()
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    if (action === "search") {
                        $scope.ESKLP_search_AC();
                    }
                    else {
                        $scope.Grid_ESKLP.ClearModify();
                        $scope.Grid_Classifier.ClearModify();
                        alert("Сохранил");
                    }
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.ESKLP_ClassifierPacking_save = function (isAdd) {
        var ESKLPId = $scope.CurrentRowESKLP.Id;
        var ClassifierPackingId = 0;
        var classifiers = $scope.Grid_Classifier.selectedRows();
        if (classifiers.length !== 1) {
            alert("выделена не 1 строка классификатора на обновление");
            return;
        }
        if (isAdd === false) {
            ClassifierPackingId = classifiers[0]["ClassifierPackingId"];
        }


        var ClassifierId = classifiers[0]["ClassifierId"];

        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Certificate/ESKLP_ClassifierPacking_save/',
                data: JSON.stringify({
                    ESKLPId: ESKLPId, ClassifierPackingId: ClassifierPackingId, ClassifierId: ClassifierId
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Classifier_Search();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    $scope.ESKLP_ClassifierPacking_delete = function () {
        var classifiers = $scope.Grid_Classifier.selectedRows();
        if (classifiers.length !== 1) {
            alert("выделена не 1 строка классификатора на обновление");
            return;
        }
        var ClassifierPackingId = classifiers[0]["ClassifierPackingId"];
        var ClassifierId = classifiers[0]["ClassifierId"];
        $scope.dataLoading =
            $http({
                method: 'POST',
                url: '/Certificate/ESKLP_ClassifierPacking_delete/',
                data: JSON.stringify({
                    ClassifierId: ClassifierId, ClassifierPackingId: ClassifierPackingId
                })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Classifier_Search();
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    function ESKLP_onSelectionChanged(row) {
        if (row !== undefined) {
            if (row.entity === undefined) {
                $scope.CurrentRowESKLP = row;
            }
            else {
                $scope.CurrentRowESKLP = row.entity;
            }            
        }
        ESKLP_LightClassrow();
    };
    function ESKLP_LightClassrow() {
        $scope.Grid_Classifier.clearSelection();
        if ($scope.CurrentRowESKLP.ClassifierPackingId > 0) {
            
            $scope.Grid_Classifier.Rows().forEach(function (item) {
                if (item.ClassifierPackingId === $scope.CurrentRowESKLP.ClassifierPackingId) {
                    $scope.Grid_Classifier.setSelected(item);
                }
            });
        }
    };
}