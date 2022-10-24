angular
    .module('DataAggregatorModule')
    .controller('PeriodsSettingsController', ['$sce', 'messageBoxService', '$scope', '$http', 'uiGridCustomService', PeriodsSettingsController]);

function PeriodsSettingsController($sce, messageBoxService, $scope, $http, uiGridCustomService) {
    document.getElementById("msgOK").style["display"] = "none";

    var changedSettings = [];

    function loadUserSourceSettings() {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/PeriodsSettings/GetUserSourceSettings/"
        }).then(function (response) {
            $scope.userSourceSettings = response.data;

            fillGridHeader();
            fillGridBody();
        }, function () {
            $scope.message = "Unexpected Error";
        });
    }

    loadUserSourceSettings();

    function fillGridHeader() {
        $scope.settingsGrid.Options.columnDefs = [];
        $scope.settingsGrid.Options.columnDefs.push({ name: 'Пользователь', field: 'userFullName', width: 150, pinnedLeft: true, enableColumnMoving: false, filter: { condition: uiGridCustomService.condition } });
        $scope.settingsGrid.Options.columnDefs.push({ name: 'Отдел', field: 'departmentShortName', width: 150, pinnedLeft: true, enableColumnMoving: false, filter: { condition: uiGridCustomService.condition } });

        angular.forEach($scope.userSourceSettings.Periods, function (period, k) {
            $scope.settingsGrid.Options.columnDefs.push({
                name: period.DisplayName,
                displayName: period.DisplayName,
                displayNameInfo: period,
                wordwrap: true,
                width: 70,
                enableSorting: false,
                headerCellTemplate: '_headerCellTemplate.html',
                cellTemplate: '<div class="ui-grid-cell-contents"><input type="checkbox" ng-checked="COL_FIELD" ng-click="grid.appScope.checkboxClick($event, row, col)"></div>'
            });
        });
    }

    function fillGridBody() {
        $scope.settingsGrid.Options.data = [];
        angular.forEach($scope.userSourceSettings.UserSources, function (userSource, key) {
            var row = [];
            row["userFullName"] = userSource.UserFullName;
            row["userId"] = userSource.UserId;
            row["departmentShortName"] = userSource.DepartmentShortName;

            angular.forEach($scope.userSourceSettings.Periods, function (period, k) {
                if (userSource.PeriodId === period.Id) {
                    row[period.DisplayName] = true;
                } else {
                    row[period.DisplayName] = false;
                }
            });

            $scope.settingsGrid.Options.data.push(row);
        });
    }

    $scope.checkboxClick = function (event, row, col) {
        var value = row.entity[col.field];
        if (value)
            event.preventDefault();

        var oldPeriodId;
        angular.forEach($scope.userSourceSettings.Periods, function (period, k) {
            if (col.field !== period.DisplayName && row.entity[period.DisplayName]) {
                row.entity[period.DisplayName] = false;
                oldPeriodId = period.Id;
            }
        });
        row.entity[col.field] = true;
        var newPeriodId = $scope.userSourceSettings.Periods.find(p => p.DisplayName === col.field).Id;
        addToChangedUserSources(row.entity["userId"], newPeriodId);
    }

    function addToChangedUserSources(userId, newPeriodId) {
        var dbPeriodId = $scope.userSourceSettings.UserSources.find(us => us.UserId === userId).PeriodId;

        var k = -1;
        for (var i = 0; i < changedSettings.length; i++) {
            if (changedSettings[i].UserId === userId) {
                k = i;
                break;
            }
        }

        if (k >= 0) {
            //если чекбокс сначала переставили, а потом опять вернули на старое место, то удаляем
            if (newPeriodId === dbPeriodId) {
                changedSettings.splice(k, 1);
            } else {
                changedSettings[k].NewPeriodId = newPeriodId;
                changedSettings[k].OldDbPeriodId = dbPeriodId;
            }
        } else {
            changedSettings.push({ "UserId": userId, "NewPeriodId": newPeriodId, "OldDbPeriodId": dbPeriodId });
        }
    }

    $scope.save = function (event) {
        $http({
                method: "POST",
                url: "/PeriodsSettings/SaveUserSourceSettings/",
                data: {
                    "changedSettingsString": JSON.stringify(changedSettings)
                }
            })
            .then(
                function(result) {
                    switch (result.data.serverMessage) {
                    case "OK":
                        loadUserSourceSettings();
                        break;
                    case "Error":
                        messageBoxService.showError(result.data.serverData);
                        break;
                    case "Conflict":
                        changedSettings = result.data.serverData;
                        var msg = "Обнаружен конфликт! Для следующих пользователей на сервере уже изменены периоды:";
                        angular.forEach(changedSettings, function(item) {
                            var userName = $scope.userSourceSettings.UserSources.find(us => us.UserId === item.UserId).UserFullName;
                            var newDbPeriod = $scope.userSourceSettings.Periods.find(p => p.Id === item.NewDbPeriodId).DisplayName;
                            var lastEditorName = item.LastEditorName;
                            msg = msg + "<br>" + userName + " (" + newDbPeriod + (lastEditorName !== "" ? " назначил(а) " + lastEditorName : "") + ")";
                        });
                        msg = msg + "<br><br><b>Перезаписать данные?</b>";

                        var msgAsHtml = $sce.trustAsHtml(msg);
                        messageBoxService.showConfirm(msgAsHtml)
                            .then(
                                function() { //yes
                                    angular.forEach(changedSettings, function(item) {
                                        item.OldDbPeriodId = item.NewDbPeriodId;
                                    });
                                    $scope.save();
                                },
                                function() { //no
                                });
                        break;
                    default:
                        messageBoxService.showError("Неизвестный код ответа от сервера!");
                    }
                }, function() {
                    messageBoxService.showError("Ошибка! Не удалось сохранить изменения.");
                }
                ).finally(function() {
                    $(event.toElement).removeAttr('disabled');
                });
    };

    $scope.settingsGrid = uiGridCustomService.createGridClass($scope, 'PeriodsSettings_Grid');
    $scope.settingsGrid.Options.enableSorting = true;
    $scope.settingsGrid.Options.excessRows = 100;
    $scope.settingsGrid.Options.excessColumns = 50;
    $scope.settingsGrid.Options.enableRowSelection = false;
    $scope.settingsGrid.Options.showGridFooter = true;
    $scope.settingsGrid.Options.enableColumnResizing = false; //иначе тормоза при фильтрации
}



