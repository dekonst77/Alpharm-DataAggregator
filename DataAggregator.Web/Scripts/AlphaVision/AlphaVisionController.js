var loginModule = angular.module('DataAggregatorModule')
    .controller('AlphaVisionController', ['$scope', '$route', '$http', '$uibModal', 'commonService', 'messageBoxService', 'hotkeys', '$timeout', 'errorHandlerService', 'uiGridCustomService', 'uiGridConstants', 'formatConstants', 'userService', AlphaVisionController])

loginModule.constant('USERCONSTANTS', (function () {
    return {
        PASSWORD_LENGTH: 7
    }
})());

loginModule.factory('myfactory', [function () {
    return {
        score: function () {
            //console.log('arguments List : ', arguments);
            var score = 0, value = arguments[0], passwordLength = arguments[1];
            var containsLetter = /[a-zA-Z]/.test(value), containsDigit = /\d/.test(value), containsSpecial = /[^a-zA-Z\d]/.test(value);
            var containsAll = containsLetter && containsDigit && containsSpecial;

            //console.log(" containsLetter - ", containsLetter,
            //    " : containsDigit - ", containsDigit,
            //    " : containsSpecial - ", containsSpecial);

            if (value.length == 0) {
                score = 0;
            } else {
                if (containsAll) {
                    score += 3;
                } else {
                    if (containsLetter) score += 1;
                    if (containsDigit) score += 1;
                    if (containsSpecial) score += 1;
                }
                if (value.length >= passwordLength) score += 1;
            }
            /*console.log('Factory Arguments : ', value, " « Score : ", score);*/
            return score;
        }
    };
}]);

loginModule.directive('okPasswordDirective', ['myfactory', 'USERCONSTANTS', function (myfactory, USERCONSTANTS) {
    return {
        // restrict to only attribute and class [AC]
        restrict: 'AC',
        priority: 2000,
        // use the NgModelController
        require: 'ngModel',

        // add the NgModelController as a dependency to your link function
        link: function ($scope, $element, $attrs, ngPasswordModel) {
            //console.log('Directive - USERCONSTANTS.PASSWORD_LENGTH : ', USERCONSTANTS.PASSWORD_LENGTH);

            $element.on('blur change keydown', function (evt) {
                $scope.$evalAsync(function ($scope) {
                    var pwd = $scope.password = $element.val();
                    // update the $scope.password with the element's value

                    /*Password Strength Meter Conditions:
                        valid password must be more than 7 characters
                        Factory score must have a minimum score of 3. [Letter, Digit, Special Char, CharLength > 7]*/
                  
                    $scope.myModulePasswordMeter = pwd ? (pwd.length > USERCONSTANTS.PASSWORD_LENGTH
                        && myfactory.score(pwd, USERCONSTANTS.PASSWORD_LENGTH) || 0) : null;

                    ngPasswordModel.$setValidity('okPasswordController', $scope.myModulePasswordMeter > 3);

                    if (ngPasswordModel.$touched && pwd.length == 0 && !ngPasswordModel.$valid && !ngPasswordModel.$$attr.required) {
                        ngPasswordModel.$$attr.$removeClass('ng-invalid');
                        ngPasswordModel.$valid = true;
                        ngPasswordModel.$invalid = false;
                        $scope.registerForm.$valid = true;
                        $scope.registerForm.$invalid = false;
                    }
                });

                if (ngPasswordModel.$valid) {
                    $scope.passwordVal = ngPasswordModel.$viewValue;
                    //console.log('Updated Val : ', $scope.passwordVal);
                    //$scope.updatePass();
                }
            });
        }
    };
}]);

loginModule.filter('passwordCountFilter', [function () {
    var passwordLengthDefault = 7;
    return function (passwordModelVal) {
        passwordModelVal = angular.isString(passwordModelVal) ? passwordModelVal : '';
        var retrunVal = passwordModelVal &&
            (passwordModelVal.length > passwordLengthDefault ? passwordLengthDefault + '+' : passwordModelVal.length);
        return retrunVal;
    };
}]);

loginModule.directive("compareTo", [function () {
    return {
        require: "ngModel",
        priority: 2000,
        // directive defines an isolate scope property (using the = mode) two-way data-binding
        scope: {
            passwordEleWatcher: "=compareTo"
        },

        link: function (scope, element, attributes, ngModel) {
            //console.log('Confirm Password Link Function call.');

            var pswd = scope.passwordEleWatcher;

            ngModel.$validators.compareTo = function (compareTo_ModelValue) {
                //console.log('scope:',scope);

                if ((pswd != 'undefined' && pswd.$$rawModelValue != 'undefined') && (pswd.$touched)) {
                    var pswdModelValue = pswd.$modelValue;
                    var isVlauesEqual = ngModel.$viewValue == pswdModelValue;
                    return isVlauesEqual;
                } else {
                    //console.log('Please enter valid password, before conforming the password.');
                    return false;
                }
            };

            scope.$watch("passwordEleWatcher", function () {
                //console.log('$watch « Confirm-Password Element Watcher.')
                ngModel.$validate();
            });

            scope.$parent.updatePass = function () {
                //console.log('$watch « Password Element Watcher.')
                //console.log('Pswd: ', scope.$parent.passwordVal, '\t Cnfirm:', ngModel.$modelValue);
                //scope.registerForm.confirm.$invalid = true;
            }
        },
    };
}]);

function AlphaVisionController($scope, $route, $http, $uibModal, commonService, messageBoxService, hotkeys, $timeout, errorHandlerService, uiGridCustomService, uiGridConstants, formatConstants, userServic)
{
    $scope.Grid = uiGridCustomService.createGridClassMod($scope, 'AlphaVisionUsers_Grid');
    $scope.Grid.Options.showGridFooter = true;
    $scope.Grid.Options.multiSelect = false;
    $scope.Grid.Options.modifierKeysToMultiSelect = true;

    $scope.Suppliers = [];/*{ "Id": null, "Name": "" }*/
    $scope.Posts = [];
    $scope.Roles = [];

    $scope.GetSuppliers = function () {
        $http({
            method: "GET",
            url: "/AlphaVision/Suppliers/"
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
                Array.prototype.push.apply($scope.Suppliers, data.Data);
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    };

    $scope.GetPosts = function () {
        $http({
            method: "GET",
            url: "/AlphaVision/Posts/"
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
                Array.prototype.push.apply($scope.Posts, data.Data);
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }
        }, function () {
            $scope.message = "Unexpected Error";
        });
    };

    $scope.GetRoles = function () {
        $scope.dataLoading = $http({
            method: 'GET',
            url: '/AlphaVision/roles/'
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
                Array.prototype.push.apply($scope.Roles, data.Data.map(function (item) { return item.Name; }));
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.checkedRoles = [];
    $scope.toggleCheck = function (role) {
        if ($scope.checkedRoles.indexOf(role) === -1) {
            $scope.checkedRoles.push(role);
            
        } else {
            $scope.checkedRoles.splice($scope.checkedRoles.indexOf(role), 1);
        }
        $scope.CreateUserForm.Roles = JSON.parse(JSON.stringify($scope.checkedRoles));
    };

    $scope.LoadData = function () {
        $scope.GetSuppliers();
        $scope.GetPosts();
        $scope.GetRoles();
    }

    $scope.LoadData();
   
    $scope.Grid.Options.columnDefs = [
        { headerTooltip: true, cellTooltip: false, enableCellEdit: false, width: 100, name: 'Id', field: 'Id', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 200, name: 'Email', field: 'Email', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Имя', field: 'Name', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Фамилия', field: 'Surname', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'Отчество', field: 'Patronymic', filter: { condition: uiGridCustomService.numberCondition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 100, name: 'День рождения', field: 'Birthday', type: 'Date', cellFilter: formatConstants.FILTER_DATE, filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 200, name: 'Организация', field: 'SupplierName', filter: { condition: uiGridCustomService.conditionList } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 200, name: 'Должность', field: 'PostName', filter: { condition: uiGridCustomService.conditionList } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 50, name: 'Доступ к API', field: 'ApiEnabled', type: 'boolean', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 150, name: 'Роли', field: 'Roles', filter: { condition: uiGridCustomService.condition } },
        { headerTooltip: true, cellTooltip: true, enableCellEdit: false, width: 120, name: 'Дата создания', field: 'CreatedDate', type: 'Date', cellFilter: formatConstants.FILTER_DATE_TIME, filter: { condition: uiGridCustomService.condition } },
    ];
 
    $scope.GetUsers = function () {
        $scope.dataLoading = $http({
            method: 'GET',
            url: '/AlphaVision/users/'
        }).then(function (response) {
            var data = response.data;
            if (data.Success) {
                $scope.Grid.Options.data = data.Data;
            } else {
                messageBoxService.showError(data.ErrorMessage);
            }

        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    $scope.GetUsers();

    $scope.CreateUserForm = {};

    $scope.CreateUser = function () {

        var user = JSON.stringify($scope.CreateUserForm);

        if (user && user.length > 0) {

            //$('#CreateUserModal').modal('hide');

            $scope.dataLoading = $http({
                method: 'POST',
                url: '/AlphaVision/CreateUser/',
                data: user
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.Grid.Options.data.push(data.Data[0]);
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
        }
        else
            alert('Вы не заполнили ни одного поля. Пользователь не будет добавлен!');
    }

    $scope.openPopupBirthday = function () {
        $scope.popupBirthday.opened = true;
    };

    $scope.popupBirthday = {
        opened: false
    };

    $scope.UpdateUser = function () {

        var user = JSON.stringify($scope.CreateUserForm);

        if (user && user.length > 0) {
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/AlphaVision/UpdateUser/',
                data: user
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    var index = $scope.Grid.Options.data.map(function (item) {
                        return item.Email
                    }).indexOf($scope.CreateUserForm.Email);
                    if (index !== -1) {
                        $scope.Grid.Options.data[index] = data.Data[0];
                    }
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
        }
        else
            alert('Вы не заполнили ни одного поля. Пользователь не будет обновлен!');
    }

    $scope.RevokeUser = function () {

        if (!window.confirm("Вы уверены, что хотите отключить пользователя от API?")) {
            return;
        }
        var user = $scope.Grid.getSelectedItem()[0];

        if (user) {
            $scope.dataLoading = $http({
                method: 'POST',
                url: '/AlphaVision/RevokeUser/',
                data: { Email: user.Email}
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    var index = $scope.Grid.Options.data.map(function (item) {
                        return item.Email
                    }).indexOf($scope.CreateUserForm.Email);
                    if (index !== -1) {
                        $scope.Grid.Options.data[index] = data.Data[0];
                    }
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
        }
        else
            alert('Вы не заполнили ни одного поля. Пользователь не будет добавлен!');
    }


    $scope.LoadDataForModal = function (action) {
        $scope.CreateUserForm = {};
        $scope.checkedRoles = []
        $scope.action = action;
        if (action == 1) {
            $scope.UserModalLabel = "Регистрация нового пользователя";
        }
        else {
            $scope.UserModalLabel = "Изменить текущего пользователя";
            var row = $scope.Grid.getSelectedItem()[0];
            $scope.CreateUserForm.Email = row.Email;
            $scope.CreateUserForm.Name = row.Name;
            $scope.CreateUserForm.SurName = row.Surname;
            $scope.CreateUserForm.Patronymic = row.Patronymic;
            $scope.CreateUserForm.Birthday = row.Birthday ? new Date(row.Birthday) : null;
            $scope.CreateUserForm.SupplierId = row.SupplierId;
            $scope.CreateUserForm.PostId = row.PostId;
            $scope.CreateUserForm.ApiEnabled = row.ApiEnabled;

            $scope.checkedRoles = JSON.parse(JSON.stringify(row.Roles));
            $scope.CreateUserForm.Roles = JSON.parse(JSON.stringify(row.Roles));

            if (!$scope.registerForm.$valid && $scope.registerForm.$error.length == 0) {
                $scope.registerForm.$valid = true;
                $scope.registerForm.$invalid = false;
            }
        }
    }


}