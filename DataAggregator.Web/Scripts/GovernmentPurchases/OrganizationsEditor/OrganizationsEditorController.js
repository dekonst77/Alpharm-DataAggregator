angular
    .module('DataAggregatorModule')
    .controller('OrganizationsEditorController', ['$scope','$route', '$http', '$uibModal', 'uiGridCustomService', 'errorHandlerService', 'commonService', 'formatConstants', 'messageBoxService', OrganizationsEditorController]);

function OrganizationsEditorController($scope, $route, $http, $uibModal, uiGridCustomService, errorHandlerService, commonService, formatConstants, messageBoxService) {
    $scope.IsNew = false;
    $scope.filter = {
        Id: '',
        Inn: '',
        FederalDistrict: {
            selectedItems: [],
            displayValue: '',
            availableItems: null
        },
        FederationSubject: {
            selectedItems: [],
            displayValue: '',
            availableItems: null
        },
        OnlyDrugsLinked: true,
        OnlyEmptyType: false,
        OnlyEmptyRegion: false,
        is_LO: false,
        is_CP: false,
        is_Actual: true
    };
    $scope.Nature = [];
    $scope.Category = [];
    $scope.OrganizationTypes = [];
    GetNature();
    getOrganizationTypes();
    getFilterFederalDistrictList();
    getFilterFederationSubjectList();
    getRegionList();

    $scope.organizationsGrid = uiGridCustomService.createGridClassMod($scope, 'OrganizationsEditor_Grid', onSelectionChanged);
    $scope.organizationsGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number', width: 50, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'ActualId', field: 'ActualId', enableCellEdit: true, type: 'number', width: 50, filter: { condition: uiGridCustomService.numberCondition } },
        { name: 'ФЗ', field: 'FZ', type: 'number', width: 50 },
        { name: 'Комментарий', enableCellEdit: true , field: 'comment', width: 100 },
        { name: 'GosZakId', enableCellEdit: true, field: 'GosZakId', type: 'number', width: 100 },
        //{ name: 'OrganizationTypeId', field: 'OrganizationTypeId', type: 'number', width: 50 },
        //{ name: 'Тип организации', field: 'OrganizationTypeText', width: 150 },
        {
            headerTooltip: true, enableCellEdit: true, width: 100, name: 'Тип организации', field: 'OrganizationTypeId', filter: { condition: uiGridCustomService.condition },
            editableCellTemplate: 'ui-grid/dropdownEditor', cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
            editDropdownOptionsArray: $scope.OrganizationTypes,
            editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name'
        },
        {
            headerTooltip: true, enableCellEdit: true, width: 100, name: 'Характер', field: 'FixedNatureId', filter: { condition: uiGridCustomService.condition },
            editableCellTemplate: 'ui-grid/dropdownEditor',cellFilter: 'griddropdownSSA:this', editType: 'dropdown',
            editDropdownOptionsArray: $scope.Nature,
            editDropdownIdLabel: 'Id', editDropdownValueLabel: 'Name'
        },
        { name: 'URL', field: 'Url', width: 150, enableCellEdit: true, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="{{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
        { name: 'Рег. статус', field: 'RegistrationStatus', width: 150 },
        { name: 'Дата регистрации', field: 'RegistrationDate', width: 150 },
        { name: 'Полное наименование', field: 'FullName', enableCellEdit: true, width: 150 },
        { name: 'Наименование', field: 'ShortName', enableCellEdit: true, width: 150 },
        { name: 'ЛО', field: 'Is_LO', width: 50, enableCellEdit: true, type: 'boolean'},
        { name: 'ЦП', field: 'Is_CP', width: 50, enableCellEdit: true, type: 'boolean' },
        { name: 'МБ заказчик', field: 'Is_Customer', width: 50, enableCellEdit: true, type: 'boolean' },
        { name: 'МБ получатель', field: 'Is_Recipient', width: 50, enableCellEdit: true, type: 'boolean' },
        { name: 'ОГРН', field: 'OGRN', width: 150, enableCellEdit: true },
        { name: 'ИНН', field: 'INN', width: 150, enableCellEdit: true },
        { name: 'КПП', field: 'KPP', width: 150, enableCellEdit: true },
        { name: 'Факт. адрес', field: 'LocationAddress', enableCellEdit: true, width: 150 },
        { name: 'Почтовый . адрес', field: 'PostAddress', enableCellEdit: true, width: 150 },
        { name: 'RegionId', field: 'RegionId', type: 'number', width: 150 },
        { name: 'Федеральный округ', field: 'FederalDistrict', width: 150 },
        { name: 'Субъект федерации', field: 'FederationSubject', width: 150 },
        { name: 'IsAnalyze', field: 'IsAnalyze', type: 'number', width: 150 },
        { name: 'RegionOfLocalizationId', field: 'RegionOfLocalizationId', type: 'number', width: 150 },
        { name: 'Федеральный округ локализации', field: 'FederalDistrictOfLocalization', width: 150 },
        { name: 'Субъект федерации локализации', field: 'FederationSubjectOfLocalization', width: 150 },
        { name: 'Посл. изм.', field: 'LastChangedUser', width: 150 },
        { name: 'Дата посл. изм.', field: 'LastChangedDate', type: 'date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition }, width: 150 }

    ];
    $scope.organizationsGrid.SetDefaults();

    $scope.Organization_search = function () {
        if ($scope.organizationsGrid.NeedSave === true) {
            messageBoxService.showConfirm('Есть несохранёные результаты. Сохранить?', 'Изменение')
                .then(
                    function (result) {
                        $scope.Organization_save("search");
                    },
                    function (result) {
                        if (result === 'no') {
                            $scope.Organization_search_ACDialog();
                        }
                        else {
                            var d = "отмена";
                        }

                    });
        }
        else {
            $scope.Organization_search_ACDialog();
        }

    };
    $scope.Organization_search_ACDialog = function () {
        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/OrganizationsEditor/_OrganizationsFilterView.html',
            controller: 'OrganizationsFilterController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                filter: $scope.filter
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                $scope.filter = data.filter;
                $scope.Organization_search_AC();
            },
            // cancel
            function () {
            }
        );
    };
    $scope.Organization_search_AC = function () {
        $scope.filter.SelectedFederalDistrictNames = $scope.filter.FederalDistrict.selectedItems.map(function (fd) { return fd.displayValue });
        $scope.filter.SelectedFederationSubjectNames = $scope.filter.FederationSubject.selectedItems.map(function (fs) { return fs.displayValue });

        $scope.loading = $http({
            method: 'POST',
            url: '/OrganizationsEditor/GetOrganizations',
            data: {
                filter: $scope.filter
            }
        }).then(function (response) {
            $scope.selectedRows = null;

            var data = response.data;
            $scope.IsNew = false;
            $scope.organizationsGrid.Options.data = data;
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    };

    $scope.Organization_save = function (action) {
        $scope.loading =
            $http({
                method: 'POST',
                url: '/OrganizationsEditor/Organization_save/',
                data: JSON.stringify({ array: $scope.organizationsGrid.GetArrayModify() })
            }).then(function (response) {
                    $scope.organizationsGrid.ClearModify();
                    if (action === "search") {
                        $scope.Organization_search_ACDialog();
                    }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };
    //сделать новую организацию
    $scope.CreateNew = function () {
        $scope.loading = $http({
            method: 'POST',
            url: '/OrganizationsEditor/GetOrganizationsNew',
            data: {           }
        }).then(function (response) {

            var data = response.data;            
            $scope.IsNew = true;
            $scope.organizationsGrid.Options.data.push(data);
            $scope.organizationsGrid.GridCellsMod(item, "@modify", true)

        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
        
    };
    // клик по кнопке "Объединить"
    $scope.openMergeDialog = function () {
        if ($scope.selectedRows === undefined || $scope.selectedRows.length < 2) {
            return;
        }

        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/OrganizationsEditor/_OrganizationsMergeView.html',
            controller: 'OrganizationsMergeController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                gridData: function () {
                    return $scope.selectedRows;
                }
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                var selectedRows = $scope.organizationsGrid.selectedRows();
                selectedRows.forEach(function (item) {
                    if (item.Id !== data.ActualId) {
                        $scope.organizationsGrid.GridCellsMod(item, "ActualId", data.ActualId)
                    }
                });
            },
            // cancel
            function () {
            }
        );
    };

    // клик по кнопке "Изменить тип организации"
    $scope.openChangeOrganizationTypeDialog = function () {
        if ($scope.selectedRows === undefined || $scope.selectedRows.length === 0) {
            return;
        }

        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/OrganizationsEditor/_OrganizationsChangeOrganizationTypeView.html',
            controller: 'OrganizationsChangeOrganizationTypeController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                gridData: function () {
                    return $scope.filter.organizationTypes;
                }
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                var selectedRows = $scope.organizationsGrid.selectedRows();
                selectedRows.forEach(function (item) {
                    $scope.organizationsGrid.GridCellsMod(item, "OrganizationTypeId", data.OrganizationTypeId);
                    $scope.organizationsGrid.GridCellsMod(item, "OrganizationTypeText", data.OrganizationTypeText);
                });
            },
            // cancel
            function () {
            }
        );
    };

    // клик по кнопке "Изменить регион"
    $scope.openChangeRegionDialog = function () {
        if ($scope.selectedRows === undefined || $scope.selectedRows.length === 0) {
            return;
        }

        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/OrganizationsEditor/_OrganizationsChangeRegionView.html',
            controller: 'OrganizationsChangeRegionController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        headerText: "Изменение региона",
                        gridData: $scope.regionList
                    }
                }
            }
        });

        dialog.result.then(
            // ok
            function (data) {
                var selectedRows = $scope.organizationsGrid.selectedRows();
                selectedRows.forEach(function (item) {
                    $scope.organizationsGrid.GridCellsMod(item, "RegionId", data.RegionId);
                    $scope.organizationsGrid.GridCellsMod(item, "FederalDistrict", data.FederalDistrict);
                    $scope.organizationsGrid.GridCellsMod(item, "FederationSubject", data.FederationSubject);
                });
            },
            // cancel
            function () {
            }
        );
    };

    // клик по кнопке "Изменить регион локализации"
    $scope.openChangeRegionOfLocalizationDialog = function () {
        if ($scope.selectedRows === undefined || $scope.selectedRows.length === 0) {
            return;
        }

        var dialog = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/OrganizationsEditor/_OrganizationsChangeRegionView.html',
            controller: 'OrganizationsChangeRegionController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                data: function () {
                    return {
                        headerText: "Изменение региона локализации",
                        gridData: $scope.regionList
                    }
                }

            }
        });

        dialog.result.then(
            // ok
            function (data) {
                var updatingOrganizationsIds = [];
                var selectedRows = $scope.organizationsGrid.selectedRows();
                selectedRows.forEach(function (item) {
                    $scope.organizationsGrid.GridCellsMod(item, "RegionOfLocalizationId", data.RegionId);
                    $scope.organizationsGrid.GridCellsMod(item, "FederalDistrictOfLocalization", data.FederalDistrict);
                    $scope.organizationsGrid.GridCellsMod(item, "FederationSubjectOfLocalization", data.FederationSubject);
                });
            },
            // cancel
            function () {
            }
        );
    };
    function getOrganizationTypes() {
        $scope.loading = $http.post("/OrganizationsEditor/GetOrganizationTypes/")
            .then(function (response) {
                Array.prototype.push.apply($scope.OrganizationTypes, response.data);
                $scope.filter.organizationTypes = response.data;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    function onSelectionChanged(row) {
        $scope.selectedRows = $scope.organizationsGrid.selectedRows();
    };
    function GetNature() {
        $scope.loading = $http.post("/OrganizationsEditor/GetNature/")
            .then(function (response) {
                Array.prototype.push.apply($scope.Nature, response.data.Nature);
                Array.prototype.push.apply($scope.Category, response.data.Category);
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }


    function getFilterFederalDistrictList() {
        $scope.loading = $http.post("/OrganizationsEditor/GetRegionNames/", { level: 1 })
            .then(function (response) {
                $scope.filter.FederalDistrict.availableItems = response.data;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    function getFilterFederationSubjectList() {
        $scope.loading = $http.post("/OrganizationsEditor/GetRegionNames/", { level: 2 })
            .then(function (response) {
                $scope.filter.FederationSubject.availableItems = response.data;
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    function getRegionList() {
        $scope.loading = $http({
            method: 'POST',
            data: { level: 2 },
            url: '/OrganizationsEditor/GetRegionList/'
        }).then(function (response) {
            $scope.regionList = response.data;
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    $(window).keydown(function (event) {
        if (event.keyCode === 16) {
            setIsShift(true);
        }
    });

    $(window).keyup(function (event) {
        if (event.keyCode === 16) {
            setIsShift(false);
        }
    });

    function setIsShift(value) {
        if (value && !$('#organizations-grid-block').hasClass('disableSelection')) {
            $scope.selectedText = commonService.getSelectionText();
        }

        if (value) {
            $('#organizations-grid-block').addClass('disableSelection');
        } else {
            $('#organizations-grid-block').removeClass('disableSelection');
        }
    };


    $scope.Load = function () {
        var Id = $route.current.params["Id"];
        var Inn = $route.current.params["Inn"];
        if (Id > 0) {
            $scope.filter.is_Actual = false;
            $scope.filter.OnlyDrugsLinked = false;
            $scope.filter.Id = Id;
            $scope.Organization_search_AC();
            return;
        }
        if (Inn !== undefined && Inn!==null) {
            $scope.filter.is_Actual = false;
            $scope.filter.OnlyDrugsLinked = false;
            $scope.filter.Inn = Inn;
            $scope.Organization_search_AC();
            return;
        }

        $scope.Organization_search();
    };

    $scope.Load();

    $scope.SetNature = function (value) {
        var selectedRows = $scope.organizationsGrid.selectedRows();
        selectedRows.forEach(function (item) {
            $scope.organizationsGrid.GridCellsMod(item, "FixedNatureId", value);
        });
    };
    $scope.SetOrganizationTypes = function (value) {
        var selectedRows = $scope.organizationsGrid.selectedRows();
        selectedRows.forEach(function (item) {
            $scope.organizationsGrid.GridCellsMod(item, "OrganizationTypeId", value);
        });
    };
}