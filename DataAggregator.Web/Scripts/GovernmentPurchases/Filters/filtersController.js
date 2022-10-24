angular.module('DataAggregatorModule')
    .controller('filtersController', ['messageBoxService', '$scope', '$http', '$uibModalInstance', 'dialogParams', filtersController]);

function filtersController(messageBoxService, $scope, $http, $modalInstance, dialogParams) {
    $scope.format = 'dd.MM.yyyy';
    $scope.filterarray = {
        federationSubjects:[],
        category :[],
        nature :[],
        nature_L2:[],
        funding :[],
        PurchaseClass :[],
        OrganizationType :[],
        DeliveryTimePeriod :[],
        LotStatus :[]
    };
    $scope.Purchase_DateBegin_start= new dateClass();
    $scope.Purchase_DateBegin_end= new dateClass();
    $scope.Purchase_DateEnd_start= new dateClass();
    $scope.Purchase_DateEnd_end = new dateClass();

    $scope.TriggerLog_When_start = new dateClass();
    $scope.TriggerLog_When_end = new dateClass();

    $scope.TriggerLogNames = ["Nature", "Category", "PurchaseClass"];

    $scope.Init = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/filters/InitClassifiers'
        }).then(function (response) {
            
            $scope.filterarray.FederationSubject = response.data.FederationSubject;
            $scope.filterarray.FederalDistrict = response.data.FederalDistrict;
            $scope.filterarray.category = response.data.Category;
            $scope.filterarray.nature = response.data.Nature;
            $scope.filterarray.nature_L2 = response.data.Nature_L2;
            $scope.filterarray.funding = response.data.Funding;
            $scope.filterarray.PurchaseClass = response.data.PurchaseClass;
            $scope.filterarray.OrganizationType = response.data.OrganizationType;
            $scope.filterarray.DeliveryTimePeriod = response.data.DeliveryTimePeriod;
            $scope.filterarray.LotStatus = response.data.LotStatus;
           
        }, function () {
            messageBoxService.showError('Не удалось загрузить справочники');
        });
    };
    

    $scope.setDate = function (val_name, value, event) {
        $scope.filter[val_name] = value;
    };

    $scope.setDateEndEnd = function () {
        //if (typeof $scope.filter.DateEndStart.Value !== 'undefined' &&
        //    $scope.filter.DateEndStart.Value !== null &&
        //    !$scope.filter.DateEndStart.Value.$invalid &&
        //    $scope.filter.DateEndEnd.Value === null) {
        //    $scope.filter.DateEndEnd.setTodayWithoutTime();
        //}
    };

    $scope.setDateValueNullIfEmpty = function (event) {
        if (event.target.value === '') {
            $scope.filter[event.target.name] = null;
        }
    };

    $scope.ok = function () {
        $scope.filterLoading = $http({
            method: 'POST',
            url: '/filters/createFilter',
            data: {
                filter: $scope.filter
        }
        }).then(function (response) {
            $modalInstance.close({
                filterwhere: response.data,
                filterall: $scope.filter
            });
        }, function () {
            messageBoxService.showError('Не удалось составить условие');
        });

    };
    $scope.clear = function (val) {
        if (val===0 && dialogParams.filterall !== undefined) {
            $scope.filter = dialogParams.filterall;
            if ($scope.filter.Purchase_DateBegin_start !== null) {
                $scope.Purchase_DateBegin_start.Value = new Date($scope.filter.Purchase_DateBegin_start);
            }
            if ($scope.filter.Purchase_DateBegin_end !== null) {
                $scope.Purchase_DateBegin_end.Value = new Date($scope.filter.Purchase_DateBegin_end);
            }
            if ($scope.filter.Purchase_DateEnd_start !== null) {
                $scope.Purchase_DateEnd_start.Value = new Date($scope.filter.Purchase_DateEnd_start);
            }
            if ($scope.filter.Purchase_DateEnd_end !== null) {
                $scope.Purchase_DateEnd_end.Value = new Date($scope.filter.Purchase_DateEnd_end);
            }
        }
        else
            {
            $scope.filter = {
                Not_Is_Recipient: false,
                isPurchaseObjectReady:false,
                Purchase_Id: "",
                Purchase_PurchaseClass: {Id:2, Name: "ЛС"},
                Purchase_Number: "",
                Purchase_Customer_FederationSubject: "",
                Purchase_Customer_FederalDistrict: "",
                Lot_Id: "",
                Purchase_Customer_name: "",
                Purchase_Customer_INN: "",
                Purchase_Customer_OrganizationType: "",
                Purchase_Name: "",
                Purchase_CustomerId: "",
                Purchase_Category: null,
                PurchaseObjectReady_Receiver_FederationSubject: "",
                Purchase_Nature: null,
                Purchase_Nature_L2: null,
                PurchaseObjectReady_Receiver_Name: "",
                LotFunding_Funding: null,
                PurchaseObjectReady_Receiver_INN: "",
                DeliveryTimeInfo_DeliveryTimePeriod: null,
                PurchaseObjectReady_Receiver_OrganizationType: null,
                Payment_KBK: "",
                PurchaseObjectReady_ReceiverId: "",
                SupplierResult_LotStatus: null,
                Purchase_DateBegin_start: null,
                Purchase_DateBegin_end: null,
                Purchase_DateEnd_start: null,
                Purchase_DateEnd_end: null,

                TriggerLog_What:'',
                TriggerLog_When_start: null,
                TriggerLog_When_end: null
            };
        }
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };
    $scope.clear(0);   
    $scope.Init();    
}