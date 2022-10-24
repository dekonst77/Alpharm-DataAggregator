angular
    .module('DataAggregatorModule')
    .controller('ManufacturerController', [
        '$scope', '$route', '$http', '$uibModal', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', ManufacturerController]);


function ManufacturerController($scope, $route, $http, $uibModal, messageBoxService, uiGridCustomService, errorHandlerService) {

    $scope.clearId = function (item) {
        item.Id = null;
    };

    function clearForm() {
        $scope.classifier.Value = null;
        $scope.classifier.Id = null;
        $scope.classifier.CorporationId = null;
        $scope.classifier.Corporation_Value = null;
        $scope.classifier.Corporation_Value_eng = null;
        $scope.classifier.Country = { Id: 0, Value: '' };
    }

    $scope.classifier = {
        CorporationId: null,
        Corporation_Value: null,
        Corporation_Value_eng: null,
        Country: { Id: null, Value: null },
        Value: null,
        Value_eng: null,
        Id: true,
        filter: null
    };

    $scope.CountryList = [];
    $scope.loadCountryList = function (selecedItem, event) {

        ////при выборе значения из выпадающего списка сначала срабатывает ng-blur, потом typeahead-on-select.
        ////при blur значение лекарственной формы ещё неизвестно, поэтому ATCWho сбрасывается.
        ////приходится запоминать.
        //if (event != null && event.type === 'blur') {
        //    $scope.lastATCWho = $scope.classifier.ATCWho;
        //} else {
        //    if ($scope.lastATCWho !== null) {
        //        $scope.classifier.ATCWho = $scope.lastATCWho;
        //    }
        //}

        $scope.classifierLoading =
            $http({
                method: 'POST',
                data: {
                    currentCountryId: $scope.classifier.Corporation ? $scope.classifier.Corporation.Id : null
                },
                url: '/Manufacturer/LoadCountryList/'
            }).then(function (response) {
                $scope.CountryList = response.data.CountryList;

                //if (response.data.ShouldClearATCWho) {
                //    $scope.classifier.ATCWho = null;
                //}
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    /*
    $scope.searchDictionary = function (value, dictionary) {
        console.debug('1');
        //Обнуляем счетчик так меняется выпадающий список
        pressedEnter = 0;
        $scope.classifierFilter[dictionary + 'Id'] = null;
        return $http.post('/Dictionary/GetDictionary/', JSON.stringify({ Value: value, Dictionary: dictionary }))
            .then(function (response) {

                return response.data;
            });
    };
    */

    // результат фильтра - получить список данных классификатора
    $scope.searchManufacture = function () {
        clearForm();

        //if (isFilterEmpty()) {
        //    $scope.classifierGrid.Options.data = [];
        //    return;
        //}

        var json = JSON.stringify({ filter: $scope.classifier.filter });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/Manufacturer/GetClassifierEditorView/',
                data: json
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.classifierGrid.Options.data = data.Data;
                    if (data.Data.length > 0) {
                        $scope.classifierGrid.clearSelection();
                        $scope.classifierGrid.setFirstSelected();
                    }
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    $scope.searchDictionary = function (value, dictionary) {
        //$scope.Manufacturer[dictionary + 'Id'] = null;
        return $http({
            method: 'POST',
            url: '/Dictionary/GetDictionary/',
            data: JSON.stringify({ Value: value, Dictionary: dictionary })
        }).then(function (response) {
            //console.debug(response);
            return response.data;
        });
    };

    $scope.changeCorporation = function (value) {
        //console.debug(value);
        $scope.classifier.Corporation_Value = value.Value;
        $scope.classifier.Corporation_Value_eng = value.Value_eng;
    }

    $scope.classifierGrid = uiGridCustomService.createGridClass($scope, 'Manufacturer_ClassifierGrid');

    $scope.classifierGrid.Options.columnDefs = [
        { name: 'ID', field: 'Id' },
        { name: 'Производитель', field: 'Value', filter: { condition: uiGridCustomService.condition } },
        { name: 'Manufacturer', field: 'Value_eng', filter: { condition: uiGridCustomService.condition } },
        { name: 'Корпорация', field: 'Corporation.Value', filter: { condition: uiGridCustomService.condition } },
        { name: 'Corporate', field: 'Corporation.Value_eng', filter: { condition: uiGridCustomService.condition } },
        { name: 'Страна', field: 'Country.Value', filter: { condition: uiGridCustomService.condition } }
    ];

    $scope.classifierGrid.Options.multiSelect = false;
    $scope.classifierGrid.Options.modifierKeysToMultiSelect = false;

    $scope.classifierGrid.selectionChanged = function () {
        var item = $scope.classifierGrid.getSelectedItem();
        if (item.length === 1)
            loadSelectedClassifier(item[0]);
    };
    function loadSelectedClassifier(item) {

        clearForm();

        var json = JSON.stringify({ selectedID: item.Id });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/Manufacturer/LoadClassifier/',
                data: json
            }).then(function (response) {
                var data = response.data;
                // $scope.classifier = data;

                $scope.classifier.Value = data.Value;
                $scope.classifier.Value_eng = data.Value_eng;
                $scope.classifier.Id = data.Id;
                $scope.classifier.CorporationId = data.CorporationId;
                $scope.classifier.Corporation_Value = data.Corporation_Value;
                $scope.classifier.Corporation_Value_eng = data.Corporation_Value_eng;

                $scope.classifier.Country = { Id: data.Country.Id, Value: data.Country.Value };
                //changeDescription();


                //this.location.search('id', '56');
                //$route.updateParams({ id: $scope.classifier.Id});
                //$route.current.params["id"] = $scope.classifier.Id;
                //$route.current.params["search"] = "";

                //const urlTree = $route.createUrlTree([], {
                //    queryParams: { id: $scope.classifier.Id,search:"" },
                //    queryParamsHandling: "merge",
                //    preserveFragment: true
                //});

                //$route.navigateByUrl(urlTree);

                //$scope.loadNfcList();

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    $scope.ManufactureDelete = function () {
        var json = JSON.stringify({ model: $scope.classifier });
        $scope.classifierSave =
            $http({
                method: 'POST',
                url: '/Manufacturer/Delete/',
                data: json
            }).then(function (response) {
                alert("Удалил.");
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    $scope.ManufactureEdit = function (isNew, isMerge) {
        var json = JSON.stringify({ model: $scope.classifier, isNew: isNew, isMerge: isMerge });

        $scope.classifierSave =
            $http({
                method: 'POST',
                url: '/Manufacturer/Save/',
                data: json
            }).then(function (response) {
                alert("Сделал.");
            }, function (response) {
                //if (response.data.message !== undefined && response.data.message.indexOf('Внимание это дубль') >= 0) {
                /*messageBoxService
                    .showConfirm(response.data.message + ' Объединить?', 'Объединение')
                    .then(
                        function (result) {
                            $scope.ManufactureEdit(false, true);
                        },
                        function (result) {
                        });*/
                //}
                // else {
                errorHandlerService.showResponseError(response);
                // }
            });
    };
    $scope.setId = function (dictionaryItem, item) {
        item.Id = dictionaryItem.Id;
        item.Value = dictionaryItem.Value;
    };


    //Загрузка
    $scope.Load = function () {
        $scope.loadCountryList();
        $scope.classifier.filter = "";
        var id_mnf = $route.current.params["id"];
        if (id_mnf !== undefined && id_mnf !== "0") {
            $scope.classifier.filter = id_mnf;
        }
        var search = $route.current.params["search"];
        if (search !== undefined && search !== "") {
            $scope.classifier.filter = search;
        }
        if ($scope.classifier.filter !== "") {
            $scope.searchManufacture();
        }
    };

    $scope.Load();
}