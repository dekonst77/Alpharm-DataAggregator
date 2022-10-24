angular
    .module('DataAggregatorModule')
    .controller('MaskController', ['$scope', '$http', '$uibModal', 'messageBoxService', 'uiGridCustomService', 'formatConstants', MaskController]);

function MaskController($scope, $http, $uibModal, messageBoxService, uiGridCustomService, formatConstants) {     

    $scope.uibSize = {};
    $scope.gridStyle = {};


    $scope.gridOptions = uiGridCustomService.createOptions('MaskGrid');

    $scope.gridOptions.columnDefs = [
        { name: 'Id', field: 'Id', width: 150 },
        { name: 'FromClassifierId', field: 'FromClassifierId', width: 200 },
        { name: 'ToClassifierId', field: 'ToClassifierId', width: 200 },
        {
            name: 'Дата добавления',
            field: 'DateInsert',
            type: 'date',
            width: 200,
            cellFilter: formatConstants.FILTER_DATE_TIME
        },
        {
            name: 'Дата обновления',
            field: 'DateUpdate',
            type: 'date',
            width: 200,
            cellFilter: formatConstants.FILTER_DATE_TIME
        },     
        {
            name: 'Заблокировано',
            field: 'Block',
            cellTemplate : '<input type="checkbox" ng-model="row.entity.Block" ng-change="grid.appScope.ChangeBlock(row.entity)">'
        },
        { name: 'DrugId', field: 'FromDrugId'},
        { name: 'TradeName', field: 'FromTradeName'},
        { name: 'OwnerTradeMarkId', field: 'FromOwnerTradeMarkId'},
        { name: 'DrugDescription', field: 'FromDrugDescription'},
        { name: 'OwnerTradeMark', field: 'FromOwnerTradeMark'},
        { name: 'PackerId', field: 'FromPackerId'},
        { name: 'Packer', field: 'FromPacker'},
        { name: 'DrugId_new', field: 'ToDrugId'},
        { name: 'TradeName_new', field: 'ToTradeName'},
        { name: 'OwnerTradeMarkId_new', field: 'ToOwnerTradeMarkId'},
        { name: 'DrugDescription_new', field: 'ToDrugDescription'},
        { name: 'OwnerTradeMark_new', field: 'ToOwnerTradeMark'},
        { name: 'PackerId_new', field: 'ToPackerId' },
        { name: 'Packer_new', field: 'ToPacker' },
        { name: 'ФИО провизора', field: 'ReplaceUser' },
    ]; 
    
    
    $scope.ChangeBlock = function (item) {
       
        $scope.vedLoading =
            $http({
                method: "POST",
                url: "/Mask/ChangeBlock/",
                data: JSON.stringify({ Id: item.Id, Block: item.Block })
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    item.DateUpdate = data.DateUpdate
                } else {
                    messageBoxService.showError(data.Message);
                }
            }, function () {
                $scope.message = "Unexpected Error";
            });
    }


    var gridOptions = {
        customEnableRowSelection: true,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableSelectAll: false,
        selectionRowHeaderWidth: 20,
        rowHeight: 30,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: false,
        noUnselect: false
    };

    angular.extend($scope.gridOptions, gridOptions);
  
    $scope.Refresh = function () {
        Load();      
    } 

    //Загружаем все VED
    function Load() {
        $scope.gridOptions.data = null;

        $scope.vedLoading =
        $http({
            method: "POST",
            url: "/Mask/Load/"
        }).then(function (response) {
           
         
            $scope.gridOptions.data = response.data;

        }, function () {
            $scope.message = "Unexpected Error";
        });
    }
 
    //Загружаем
    Load();
}