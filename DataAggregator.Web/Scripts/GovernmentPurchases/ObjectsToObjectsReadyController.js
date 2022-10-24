angular
    .module('DataAggregatorModule')
    .controller('ObjectsToObjectsReadyController', ['messageBoxService', '$scope', '$http', 'uiGridCustomService', 'formatConstants', ObjectsToObjectsReadyController]);

function ObjectsToObjectsReadyController(messageBoxService, $scope, $http, uiGridCustomService, formatConstants) {

    //Методы
    $scope.loadLogs = function () {
        loadLogs();
    }

    // Загрузка данных
    loadLogs();

    function loadLogs() {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/ObjectsToObjectsReady/LoadLogs/"
        }).then(function (response) {
            $scope.logs.Options.data = response.data;
        }, function () {
            $scope.message = "Unexpected Error";
            messageBoxService.showError("Не удалось загрузить таблицу с логами!");
        });
    }    

    function startTransfer(filter) {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/ObjectsToObjectsReady/StartTransfer/",
            data: filter.getJson()
        }).then(function (response) {
            loadLogs();
        }, function () {
            $scope.message = "Unexpected Error";
            messageBoxService.showError("Не удалось запустить перенос данных!");
        });
    }

    //Логи переноса
    $scope.logs = uiGridCustomService.createGridClass($scope, 'ObjectsToObjectsReady_LogsGrid');
    $scope.logs.Options.showGridFooter = true;
    $scope.logs.Options.columnDefs =
    [
        { name: 'Запуск', field: 'DateStart', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME },
        { name: 'Окончание', field: 'DateEnd', type: 'date', cellFilter: formatConstants.FILTER_DATE_TIME },
        { name: 'Тип', field: 'Type' },
        { name: 'Пользователь', field: 'FullName', enableHiding: false },
        { name: 'Аргументы', field: 'Arguments' },
        { name: 'Результат', field: 'Result' }
     ];     

    //Фильтр
    var filterClass = function(loadFunction) {

        this.dateStart = new dateClass();
        this.dateStart.setTodayWithoutTime();
        this.dateEnd = new dateClass();
        this.dateEnd.setTodayWithoutTime();
        this.transferObjects = false;
        this.transferContracts = false;

        this.startTransfer = function () {
            if(!this.validate()) {
                return;
            }

            loadFunction(this);
        }

        this.validate = function () {
            if (this.dateStart.Value == undefined) {
                $scope.message = "User input error";
                messageBoxService.showError("Неверно указано начало периода!");
                return false;
            }
            if (this.dateEnd.Value == undefined) {
                $scope.message = "User input error";
                messageBoxService.showError("Неверно указан конец периода!");
                return false;
            }
            if (this.dateEnd.Value < this.dateStart.Value) {
                $scope.message = "User input error";
                messageBoxService.showError("Дата конца периода не может быть меньше даты начала!");
                return false;

            }
            if (!this.transferContracts && !this.transferObjects) {
                $scope.message = "User input error";
                messageBoxService.showError("Выберите хотя бы один тип переносимых объектов!");
                return false;
            }

            return true;
        }

        this.getJson = function () {
            var filterTransfer = {
                DateStart: this.dateStart.Value,
                DateEnd: this.dateEnd.Value,
                TransferObjects: this.transferObjects,
                TransferContracts: this.transferContracts
            }
            return JSON.stringify({ filterTransfer: filterTransfer });
        }
    }
    
    //Объект фильтр
    $scope.filter = new filterClass(startTransfer);
}



