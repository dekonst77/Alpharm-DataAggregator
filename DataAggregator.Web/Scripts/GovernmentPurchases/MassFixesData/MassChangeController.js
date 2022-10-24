angular.module('DataAggregatorModule')
    .controller('MassChangeController', ['messageBoxService', '$scope', '$http', '$uibModal', '$uibModalInstance', 'dialogParams', MassChangeController]);

function MassChangeController(messageBoxService, $scope, $http, $uibModal, $modalInstance, dialogParams) {
    $scope.format = 'dd.MM.yyyy';
    $scope.natureNames = [];
    $scope.natureNames_L2 = [];
    $scope.deliveryTimePeriodList = [];
    $scope.FundingList = [];
    $scope.Comment = "";
    $scope.Init = function () {
        $scope.changeLoading = $http({
            method: 'POST',
            url: '/MassFixesData/InitMiniClassifiers'
        }).then(function (response) {
            $scope.natureNames = response.data.Nature;
            $scope.natureNames_L2 = response.data.Nature_L2;
            $scope.deliveryTimePeriodList = response.data.deliveryTimePeriodList;
            $scope.FundingList = response.data.FundingList;
        }, function () {
            messageBoxService.showError('Не удалось загрузить справочники');
        });
    };
    $scope.getDateTime = function (dateStingddMMyyy) {
        if (dateStingddMMyyy.length > 0) {
            var dateF = dateStingddMMyyy.split('.');
            if (dateF.length === 3)
                var date = new Date(dateF[2], dateF[1] - 1, dateF[0]);
            date = new Date(date.getTime() - 60000 * date.getTimezoneOffset());
            return date;
        }
        return null;
    };
    $scope.changeMinAndMaxDates = function (item) {
        var DateStart = item.DateStart;

        item.DateEndOptions.minDate = DateStart;


        if (item.DateEnd !== undefined && item.DateEnd !== null) {
            var DateEnd = item.DateEnd;
            if (DateStart >= DateEnd) {
                alert("дата завершения меньше даты начала.");
                item.DateEnd = null;
            }
        }

    };

    //$scope.setDate = function (val_name, value, event) {
    //    $scope.filter[val_name] = value;
    //}


    //$scope.setDateValueNullIfEmpty = function (event) {
    //    if (event.target.value === '') {
    //        $scope.filter[event.target.name] = null;
    //    }
    //};
    $scope.setId = function (dictionaryItem, item) {
        item = dictionaryItem.Id;        
    };
    $scope.ok = function () {
        if ($scope.Comment !== null && $scope.Comment!=="")
            $scope.change.Comment = $scope.Comment;
        else
            $scope.change.Comment = "";

        if ($scope.Nature !== null && $scope.Nature.Id > 0)
            $scope.change.NatureId = $scope.Nature.Id;
        else
            $scope.change.NatureId = 0;

        if ($scope.Nature_L2 !== null && $scope.Nature_L2.Id > 0)
            $scope.change.Nature_L2Id = $scope.Nature_L2.Id;
        else
            $scope.change.Nature_L2Id = 0;


        if ($scope.Receiver !== null && $scope.Receiver.Id > 0)
            $scope.change.ReceiverId = $scope.Receiver.Id;
        else
            $scope.change.ReceiverId = 0;

        $modalInstance.close({change: $scope.change});
    };
    $scope.clear = function () {
        $scope.Nature = { Id: 0, Value: "без Изменений" };
        $scope.Nature_L2 = { Id: 0, Value: "без Изменений" };
        $scope.Receiver = { Id: 0, Value: "без Изменений" };
        $scope.change = {
            NatureId: 0,
            Nature_L2Id: 0,
            ReceiverId: 0,
            DeliveryTimeInfo: [],
            FundingIds: [],
            PurchaseIds: [],
            LotIds: [],
            PurchaseObjectReadyIds:[]
        };
    };

    $scope.searchReceiver = function (value) {
        //$scope.classifier[dictionary + 'Id'] = null;
        return $http({
            method: 'POST',
            url: '/MassFixesData/searchReceiver/',
            data: JSON.stringify({ Value: value })
        }).then(function (response) {
            return response.data;
        });
    };
    $scope.changeObjectReadyReceiver = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_SearchOrganizationView.html',
            controller: 'SearchOrganizationController',
            size: 'lg',
            windowClass: 'wide-dialog',
            backdrop: 'static',
            resolve: {
                Is_Customer: false, Is_Recipient: false
            }
        });

        modalInstance.result.then(function (v) {
            $scope.Receiver.Id = v.Id;
            $scope.change.ReceiverId = v.Id;
            $scope.Receiver.Value = v.ShortName;
        }, function () {
            $scope.Receiver.Id = 0;
            $scope.change.ReceiverId = 0;
            $scope.Receiver.Value = "без Изменений";
        });
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('dismiss');
    };

    $scope.removeDeliveryTimeInfo = function (index) {
        $scope.change.DeliveryTimeInfo.splice(index, 1);
    };
    $scope.addDeliveryTimeInfo = function () {
        var deliveryTimeInfo = new DeliveryTimeInfoClass();
        if ($scope.change.DeliveryTimeInfo !== null && $scope.change.DeliveryTimeInfo.DateEnd !== null && $scope.change.DeliveryTimeInfo.DateEnd !== undefined) {

            var dateStart = $scope.getDateTime($scope.change.DeliveryTimeInfo.DateEnd);

            if (dateStart !== null) {
                dateStart.setDate(dateStart.getDate() + 10);
                deliveryTimeInfo.DateStart = dateStart;
                deliveryTimeInfo.changeMinAndMaxDates();
            }
        }
        $scope.change.DeliveryTimeInfo.push(deliveryTimeInfo);
    };
    var DeliveryTimeInfoClass = function (object) {


        var deliveryTimeInfoClass = {};

        deliveryTimeInfoClass.Id = null;
        deliveryTimeInfoClass.Count = null;
        deliveryTimeInfoClass.DeliveryTimePeriod = null;
        deliveryTimeInfoClass.DateStart = null;
        deliveryTimeInfoClass.DateEnd = null;
        deliveryTimeInfoClass.StartOpened = false;
        deliveryTimeInfoClass.EndOpened = false;
        deliveryTimeInfoClass.DateStartOptions = { minDate: null, maxDate: null };
        deliveryTimeInfoClass.DateEndOptions = { minDate: null };


        deliveryTimeInfoClass.StartOpen = function () {
            this.StartOpened = !this.StartOpened;
        };

        deliveryTimeInfoClass.EndOpen = function () {
            this.EndOpened = !this.EndOpened;
        };

        if (object !== null && object !== undefined) {
            deliveryTimeInfoClass.Id = object.Id;
            deliveryTimeInfoClass.Count = object.Count;
            deliveryTimeInfoClass.DeliveryTimePeriod = object.DeliveryTimePeriod;
            deliveryTimeInfoClass.DateStart = new Date(object.DateStart).getUTCTime();
            deliveryTimeInfoClass.DateEnd = new Date(object.DateEnd).getUTCTime();
        }

        deliveryTimeInfoClass.changeMinAndMaxDates = function () {


            if (deliveryTimeInfoClass.DateEnd !== null) {
                deliveryTimeInfoClass.DateStartOptions.maxDate = new Date(deliveryTimeInfoClass.DateEnd).getUTCTime();
            } else {
                deliveryTimeInfoClass.DateStartOptions.maxDate = null;
            }


            if ($scope.change.DeliveryTimeInfo.DateEndFormat !== null && $scope.change.DeliveryTimeInfo.DateEndFormat !== undefined) {
                deliveryTimeInfoClass.DateStartOptions.minDate = new Date($scope.change.DeliveryTimeInfo.DateEndFormat).getUTCTime();
            }

            if (deliveryTimeInfoClass.DateStart !== null) {
                deliveryTimeInfoClass.DateEndOptions.minDate = new Date(deliveryTimeInfoClass.DateStart).getUTCTime();
            } else {
                deliveryTimeInfoClass.DateEndOptions.minDate = null;
            }
        };

        deliveryTimeInfoClass.changeMinAndMaxDates();

        return deliveryTimeInfoClass;

    };

    Date.prototype.getUTCTime = function () {
        var userTimezoneOffset = this.getTimezoneOffset() * 60000;
        return new Date(this.getTime() - userTimezoneOffset);
    };

    $scope.editFunding = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/GovernmentPurchases/GovernmentPurchases/_FundingView.html',
            controller: 'FundingController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                fundingList: function () {
                    return $scope.FundingList;
                },
                sourceOfFinancing: function () {
                    return $scope.SourceOfFinancing;
                }
            }
        });

        modalInstance.result.then(function (v) {
            $scope.change.Funding = v;
            formFundingString();
        }, function () {
        });
    };
    function formFundingString() {
        var result = "";
        $scope.change.FundingIds = [];
        $scope.change.Funding.forEach(function (item, i, arr) {
            if (item.CheckedList[0].Checked === true)
            {
                result += ", " + item.InternalName;
                $scope.change.FundingIds.push(item.Id);
            }
        });
        $scope.FundingD = result;
    }
    $scope.clear();
    $scope.Init();
}