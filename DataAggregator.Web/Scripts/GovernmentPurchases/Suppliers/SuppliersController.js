angular
    .module('DataAggregatorModule')
    .controller('SuppliersController', ['messageBoxService', '$scope', '$http', '$uibModal', 'uiGridCustomService', 'errorHandlerService', SuppliersController]);

function SuppliersController(messageBoxService, $scope, $http, $uibModal, uiGridCustomService, errorHandlerService) {
    $scope.supplierRawFilter = {
        Ready: false,
        NotReady: true,
        Id: '',
        Name: '',
        Address: '',
        Phone: '',
        INN: '',
        KPP: '',
        SupplierId: '',
        SupplierName: ''
    }

    $scope.supplierFilter = {
        Id: '',
        Name: '',
        INN: '',
        KPP: '',
        LocationAddress: '',
        ContactMail: '',
        PhoneNumber: ''
    }

    // Грид с supplierRaw

    $scope.supplierRawLoading = null;

    $scope.supplierRawGrid = uiGridCustomService.createGridClass($scope, 'Suppliers_SupplierRawGrid');

    $scope.supplierRawGrid.Options.useExternalFiltering = true;
    $scope.supplierRawGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Название', field: 'Name' },
        { name: 'Адрес регистрации', field: 'LocalAddres' },
        { name: 'Телефон', field: 'Phone' },
        { name: 'ИНН', field: 'INN', type: 'number' },
        { name: 'КПП', field: 'KPP', type: 'number' },
        { name: 'Id справочник', field: 'SupplierId', type: 'number' },
        { name: 'Название справочник', field: 'SupplierName' }
    ];

    $scope.supplierRawGrid.Options.multiSelect = true;
    $scope.supplierRawGrid.Options.noUnselect = false;
    $scope.supplierRawGrid.Options.modifierKeysToMultiSelect = true;
    $scope.supplierRawGrid.Options.showGridFooter = true;

    $scope.supplierRawGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.supplierRawGridApi = gridApi;

        $scope.supplierRawGridApi.core.on.filterChanged($scope, function () {
            var grid = this.grid;

            $scope.supplierRawFilter.Id = grid.columns[0].filters[0].term;
            $scope.supplierRawFilter.Name = grid.columns[1].filters[0].term;
            $scope.supplierRawFilter.Address = grid.columns[2].filters[0].term;
            $scope.supplierRawFilter.Phone = grid.columns[3].filters[0].term;
            $scope.supplierRawFilter.INN = grid.columns[4].filters[0].term;
            $scope.supplierRawFilter.KPP = grid.columns[5].filters[0].term;
            $scope.supplierRawFilter.SupplierId = grid.columns[6].filters[0].term;
            $scope.supplierRawFilter.SupplierName = grid.columns[7].filters[0].term;
        });

        //Что-то выделили
        $scope.supplierRawGridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selectedsupplierRawId = $scope.supplierRawGridApi.selection.getSelectedRows().map(function (value) {
                return value.Id;
            });
        });

        //Что-то выделили
        $scope.supplierRawGridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.selectedsupplierRawId = $scope.supplierRawGridApi.selection.getSelectedRows().map(function (value) {
                return value.Id;
            });
        });

        //Очищает выделенное когда изменяется фильтр
        $scope.supplierRawGridApi.core.on.filterChanged($scope, function () {
            $scope.supplierRawGridApi.selection.clearSelectedRows();
        });
    };

    $scope.loadSupplierRawList = function () {
        $scope.supplierRawLoading = $http({
            method: 'POST',
            url: '/Suppliers/LoadSupplierRawList/',
            data: JSON.stringify({ supplierRawFilterJson: $scope.supplierRawFilter })
        }).then(function (response) {
            var data = response.data;
            $scope.supplierRawGrid.Options.data = data.Data;
            $scope.supplierRawNotReadyCount = data.Count;
        }, function () {
            $scope.supplierRawGrid.Options.data = [];
            $scope.message = 'Unexpected Error';
            messageBoxService.showError($scope.message);
        });
    }

    $scope.loadSupplierRawList();

    // Грид с supplier

    $scope.supplierLoading = null;

    $scope.supplierGrid = uiGridCustomService.createGridClass($scope, 'Suppliers_SupplierGrid');

    $scope.supplierGrid.Options.useExternalFiltering = true;
    $scope.supplierGrid.Options.columnDefs = [
        { name: 'Id', field: 'Id', type: 'number' },
        { name: 'Название', field: 'Name' },
        { name: 'ИНН', field: 'INN', type: 'number' },
        { name: 'КПП', field: 'KPP', type: 'number' },
        { name: 'Адрес', field: 'LocationAddress' },
        { name: 'Почта', field: 'ContactMail' },
        { name: 'Телефон', field: 'PhoneNumber' }
    ];

    $scope.supplierGrid.Options.rowTemplate = '_rowSupplierTemplate.html';

    $scope.supplierGrid.Options.multiSelect = true;
    $scope.supplierGrid.Options.noUnselect = false;
    $scope.supplierGrid.Options.modifierKeysToMultiSelect = true;
    $scope.supplierGrid.Options.showGridFooter = true;

    $scope.supplierGrid.Options.onRegisterApi = function (gridApi) {
        //set gridApi on scope
        $scope.supplierGridApi = gridApi;

        $scope.supplierGridApi.core.on.filterChanged($scope, function () {
            var grid = this.grid;

            $scope.supplierFilter.Id = grid.columns[0].filters[0].term;
            $scope.supplierFilter.Name = grid.columns[1].filters[0].term;
            $scope.supplierFilter.INN = grid.columns[2].filters[0].term;
            $scope.supplierFilter.KPP = grid.columns[3].filters[0].term;
            $scope.supplierFilter.LocationAddress = grid.columns[4].filters[0].term;
            $scope.supplierFilter.ContactMail = grid.columns[5].filters[0].term;
            $scope.supplierFilter.PhoneNumber = grid.columns[6].filters[0].term;
        });

        //Что-то выделили
        $scope.supplierGridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.selectedSupplier = $scope.supplierGridApi.selection.getSelectedRows().map(function (value) {
                return value;
            });
        });

        //Что-то выделили
        $scope.supplierGridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.selectedSupplier = $scope.supplierGridApi.selection.getSelectedRows().map(function (value) {
                return value;
            });
        });

        //Очищает выделенное когда изменяется фильтр
        $scope.supplierGridApi.core.on.filterChanged($scope, function () {
            $scope.supplierGridApi.selection.clearSelectedRows();
        });
    };

    $scope.loadSupplierList = function () {
        $scope.supplierLoading = $http({
            method: 'POST',
            url: '/Suppliers/LoadSupplierList/',
            data: JSON.stringify({ supplierFilterJson: $scope.supplierFilter })
        }).then(function (response) {
            $scope.supplierGrid.Options.data = response.data;
        }, function () {
            $scope.supplierGrid.Options.data = [];
            $scope.message = 'Unexpected Error';
            messageBoxService.showError($scope.message);
        });
    }

    $scope.loadSupplierList();


    $scope.bindSupplier = function (element) {

        var supplierRawListId = $scope.selectedsupplierRawId;
        var supplierId = element.Id;
        var supplierName = element.Name;

        $scope.supplierLoading =
        $http({
            method: 'POST',
            url: '/Suppliers/BindSupplier/',
            data: JSON.stringify({ supplierRawListId: supplierRawListId, supplierId: supplierId })
        }).then(function (response) {
            if (response.data === true) {
                $scope.selectedsupplierRawId.forEach(function(item, i, arr) {
                    var currentId = item;
                    $scope.supplierRawGrid.Options.data.forEach(function(item, i, arr) {
                        if (item.Id == currentId) {
                            item.SupplierId = supplierId;
                            item.SupplierName = supplierName;
                        }
                    });
                });
            }
        }, function () {
            $scope.message = 'Unexpected Error';
        });
    }


    $scope.editSupplier = function () {

        if ($scope.selectedSupplier == undefined) {
            return;
        }

        var editSupplierParameters = {
            "header": 'Редактирование поставщика',
            "selectedSupplier": JSON.parse(JSON.stringify($scope.selectedSupplier[0]))
    }

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/Suppliers/_EditSupplierView.html',
            controller: 'EditSupplierController',
            size: 'lg',
            //windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                editSupplierParameters: function() {
                    return editSupplierParameters;
                }
            }
        });

        modalInstance.result.then(function (data) {
            for (var i = 0; i < $scope.supplierGrid.Options.data.length; i++) {
                if ($scope.supplierGrid.Options.data[i].Id == data.supplier.Id) {
                    $scope.supplierGrid.Options.data[i].Name = data.supplier.Name;
                    $scope.supplierGrid.Options.data[i].INN = data.supplier.INN;
                    $scope.supplierGrid.Options.data[i].KPP = data.supplier.KPP;
                    $scope.supplierGrid.Options.data[i].LocationAddress = data.supplier.LocationAddress;
                    $scope.supplierGrid.Options.data[i].ContactMail = data.supplier.ContactMail;
                    $scope.supplierGrid.Options.data[i].PhoneNumber = data.supplier.PhoneNumber;
                    break;
                }
            }
        }, function () {
        });
    }


    $scope.addSupplier = function () {

        var editSupplierParameters = {
            "header": 'Добавление поставщика',
            "selectedSupplier": {}
        }

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/Suppliers/_EditSupplierView.html',
            controller: 'EditSupplierController',
            size: 'lg',
            //windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                editSupplierParameters: function () {
                    return editSupplierParameters;
                }
            }
        });

        modalInstance.result.then(function (data) {
            $scope.supplierGrid.Options.data.push(data.supplier);
        }, function () {
        });

    }


    $scope.deleteSupplier = function () {

        if ($scope.selectedSupplier == undefined) {
            return;
        }

        var supplierIdList = $scope.selectedSupplier.map(function (item) { return item.Id; });

        $scope.supplierLoading =
        $http({
            method: 'POST',
            url: '/Suppliers/DeleteSupplier/',
            data: JSON.stringify({ id: supplierIdList })
        }).then(function (response) {
            if (response.data === true) {
                $scope.selectedSupplier.forEach(function (item, i, arr) {
                    $scope.supplierGrid.Options.data.removeitem(item);
                });
            }
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.mergeSupplier = function() {
        
        if ($scope.selectedSupplier == undefined) {
            return;
        }
        var supplierIdList = $scope.selectedSupplier.map(function (item) { return item.Id; });

        var mergeSupplierParameters = {
            "supplierIdList": supplierIdList
        }

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/Suppliers/_MergeSupplierView.html',
            controller: 'MergeSupplierController',
            size: 'lg',
            //windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                mergeSupplierParameters: function () {
                    return mergeSupplierParameters;
                }
            }
        });

        modalInstance.result.then(function (data) {
            var resultSupplierId = data.resultSupplierId;

            $scope.supplierLoading =
            $http({
                method: 'POST',
                url: '/Suppliers/MergeSupplier/',
                data: JSON.stringify({ id: supplierIdList, resultSupplierId: resultSupplierId })
            }).then(function (response) {
                if (response.data === true) {
                    $scope.loadSupplierRawList();
                    $scope.loadSupplierList();
                }
            }, function () {
                $scope.message = 'Unexpected Error';
            });

        }, function () {
        });
    };
}