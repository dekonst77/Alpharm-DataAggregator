angular
    .module('DataAggregatorModule')
    .controller('GoodsClassifierEditorController', [
        '$scope', '$route', '$http', '$uibModal', '$timeout', 'messageBoxService', 'uiGridCustomService', 'errorHandlerService', 'formatConstants', GoodsClassifierEditorController]);


function GoodsClassifierEditorController($scope, $route, $http, $uibModal, $timeout, messageBoxService, uiGridCustomService, errorHandlerService, formatConstants) {

    // хранит значение бренда из БД для текущей связки TN+Ownner, если такой уже существует,
    // чтобы предотвратить изменение бренда при добавлении новой записи
    $scope.ExistingGoodsBrandValue = null;

    $scope.isEditor = $route.current.isEditor;

    $scope.clearKey = function (fieldKey) {
        $scope.InnerCods[fieldKey] = null;
    };

    $scope.InnerCods = { OwnerTradeMakrId: '', PackerId: '', l: '', OnwerRegistrationCertificateKey: '' };

    var getGoodsCategoryList = function () {
        $scope.loadingDictionary = $http({
            method: "POST",
            url: "/GoodsClassifierEditor/GetGoodsCategoryList/"
        }).then(function (response) {
            $scope.goodsCategoryList = response.data.Data;
        }, function () {
            messageBoxService.showError("Не удалось загрузить список категорий!");
        });
    };

    getGoodsCategoryList();

    $scope.classifierGrid = uiGridCustomService.createGridClass($scope, 'GoodsClassifierEditor_ClassifierGrid');

    $scope.classifierGrid.Options.rowHeight = 20,
        $scope.classifierGrid.Options.columnDefs = [
            { name: ' ', cellTemplate: '_icon.html', enableFiltering: false, enableSorting: false, width: 25 },
            { name: 'ClassifierId', field: 'ClassifierId', filter: { condition: uiGridCustomService.numberCondition }, cellFilter: formatConstants.FILTER_INT_COUNT, cellTemplate: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="/#/Classifier/ClassifierEditor/Edit?ClassifierId={{COL_FIELD}}" target="_blank">{{COL_FIELD}}</a></div>' },
            { name: 'GoodsId', field: 'GoodsId', type: 'number', filter: { condition: uiGridCustomService.numberCondition } },
            { name: 'Торговое наименование', field: 'GoodsTradeName', filter: { condition: uiGridCustomService.condition } },
            { name: 'Форма выпуска', field: 'GoodsDescription', filter: { condition: uiGridCustomService.condition } },
            { name: 'OwnerTradeMarkId', field: 'OwnerTradeMarkId', filter: { condition: uiGridCustomService.condition } },
            { name: 'Правообладатель', field: 'OwnerTradeMark', filter: { condition: uiGridCustomService.condition } },
            { name: 'PackerId', field: 'PackerId', filter: { condition: uiGridCustomService.condition } },
            { name: 'Упаковщик', field: 'Packer', filter: { condition: uiGridCustomService.condition } },
            { name: 'Категория', field: 'GoodsCategoryName', filter: { condition: uiGridCustomService.condition } },
            { name: 'Comment', field: 'Comment', filter: { condition: uiGridCustomService.condition } }
        ];

    $scope.classifierGrid.Options.multiSelect = false;
    $scope.classifierGrid.Options.modifierKeysToMultiSelect = false;

    $scope.classifierGrid.Options.rowTemplate = '_rowClassifierTemplate.html';
    $scope.classifierGrid.selectionChanged = function () {
        var item = $scope.classifierGrid.getSelectedItem();
        if (item.length === 1)
            loadSelectedClassifier(item[0]);
    };


    function initializeClassifier() {
        $scope.classifier = {
            GoodsId: null,
            GoodsTradeName: { Id: null, Value: null },
            GoodsDescription: null,
            OwnerTradeMark: { Id: null, Value: null },
            Packer: { Id: null, Value: null },
            GoodsCategory: null,
            Used: false,
            ToRetail: false
        };
        $scope.InnerCods.GoodKey = null;
        $scope.InnerCods.OwnerTradeMarkId = null;
        $scope.InnerCods.PackerId = null;

        $scope.parameterGroupsList = [];
    }

    initializeClassifier();

    $scope.initializeClassifier = function () {
        initializeClassifier();
    }

    $scope.searchDictionary = function (value, dictionary) {
        return $http({
            method: 'POST',
            url: '/Dictionary/GetDictionary/',
            data: JSON.stringify({ Value: value, Dictionary: dictionary })
        }).then(function (response) {
            return response.data;
        });
    };

    $scope.getGoodsBrandList = function () {
        $scope.classifierLoading = $http({
            method: 'POST',
            url: '/GoodsClassifierEditor/GetGoodsBrandList/'
        }).then(function (response) {
            $scope.goodsBrandList = response.data.Data;
        });
    };
    $scope.getGoodsBrandList();

    $scope.changeGoodsBrand = function () {
        $scope.classifierLoading = $http({
            method: 'POST',
            url: '/GoodsClassifierEditor/GetGoodsBrand/',
            data: JSON.stringify({ goodsTradeNameValue: $scope.classifier.GoodsTradeName.Value, ownerTradeMarkValue: $scope.classifier.OwnerTradeMark.Value })
        }).then(function (response) {
            $scope.ExistingGoodsBrandValue = response.data.Data != null ? response.data.Data.Value : null; //запоминаем отдельно бренд, соответствующий наименованию и производителю
            $scope.classifier.GoodsBrand = response.data.Data;
        });
    }
    $scope.setId = function (dictionaryItem, item) {
        item.Id = dictionaryItem.Id;
        item.Value = dictionaryItem.Value;
    };

    $scope.clearId = function (item) {
        item.Id = null;
    };

    $scope.setKey = function (fieldKeyName, fieldId) {
        if (fieldId == null) {
            $scope.classifier[fieldKeyName].Key = null;
        } else {
            $scope.classifierLoading =
                $http({
                    method: 'POST',
                    url: '/GoodsClassifierEditor/GetKey/',
                    data: { fieldId: fieldId }
                }).then(function (response) {
                    $scope.classifier[fieldKeyName].Key = response.data.Data;
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    };

    $scope.classifierFilter = {
        goodsCategory: { Id: 0, Name: "" }
    };

    $scope.loadParametersList = function () {
        $scope.parameterGroupsList = [];
        if ($scope.classifier.GoodsCategory !== null) {
            $scope.classifierLoading = $http.post("/GoodsClassifierEditor/GetParameterGroupsList/", {
                goodsCategoryId: $scope.classifier.GoodsCategory.Id,
                goodsId: $scope.classifier.GoodsId,
                ownerTrademarkId: $scope.classifier.OwnerTradeMarkId,
                packerId: $scope.classifier.PackerId
            })
                .then(function (response) {
                    $scope.parameterGroupsList = response.data;
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
        }
    }
    $scope.loadParametersList();

    function fillClassifierParameters() {
        $scope.classifier.ParameterIds = [];

        angular.forEach($scope.parameterGroupsList, function (parametersGroup) {
            var parameter = parametersGroup.ParametersList.filter(function (parameter) {
                return parameter.Value === parametersGroup.SelectedParameterValue;
            });

            if (parameter.length > 0) {
                $scope.classifier.ParameterIds.push(parameter[0].Id);
            }
        });
    }

    // результат фильтра - получить список данных классификатора
    function loadFilteredClassifier(afterAdd) {

        // Если вызвали функцию после добавления нового элемента, то в список брендов могло добавиться новое значение. 
        // Поэтому обновляем список.
        if (afterAdd) {
            $scope.getGoodsBrandList();
        }

        var json = JSON.stringify($scope.classifierFilter);

        // если вызвали функцию после добавления нового элемента, при этом грид с результатами поиска был пустой и в фильтре все поля пустые,
        // то выходим
        if (afterAdd &&
            $scope.classifierGrid.Options.data.length === 0 &&
            !$scope.classifierFilter.goodsTradeName &&
            !$scope.classifierFilter.ownerTradeMark &&
            !$scope.classifierFilter.goodsDescription &&
            !$scope.classifierFilter.packer &&
            !$scope.classifierFilter.goodsId &&
            !$scope.classifierFilter.classifierId &&
            !$scope.classifierFilter.ownerTradeMarkId &&
            $scope.classifierFilter.goodsCategory.Id === 0 &&
            !$scope.classifierFilter.packerId) {
            return;
        }

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/GoodsClassifierEditor/GetClassifierEditorView/',
                data: json
            }).then(function (response) {
                var data = response.data;
                if (data.Success) {
                    $scope.classifierGrid.Options.data = data.Data;
                    initializeClassifier();
                    if ($scope.classifierGrid.Options.data.length > 0) {
                        loadSelectedClassifier($scope.classifierGrid.Options.data[0]);
                    }
                } else {
                    messageBoxService.showError(data.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }


    //Загружаем выбранный классификатор
    function loadSelectedClassifier(item) {

        var json = JSON.stringify({ goodsId: item.GoodsId, ownerTradeMarkId: item.OwnerTradeMarkId, packerId: item.PackerId });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/GoodsClassifierEditor/LoadClassifier/',
                data: json
            }).then(function (response) {
                var data = response.data;
                $scope.classifier = data;

                $scope.loadParametersList();

                $scope.InnerCods.GoodKey = $scope.classifier.GoodKey;
                $scope.InnerCods.OwnerTradeMarkId = $scope.classifier.OwnerTradeMark.Id;
                $scope.InnerCods.PackerId = $scope.classifier.Packer.Id;

                $scope.ExistingGoodsBrandValue = $scope.classifier.GoodsBrand != null ? $scope.classifier.GoodsBrand.Value : null;

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    // клик по кнопке "Поиск по справочнику"
    $scope.searchClassifier = function () {

        var modalClassifieInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Views/Classifier/GoodsClassifierEditor/_GoodsClassifierEditorFilterView.html',
            controller: 'GoodsClassifierEditorFilterController',
            size: 'lg',
            windowClass: 'center-modal',
            backdrop: 'static',
            resolve: {
                classifierFilter: function () {
                    return $scope.classifierFilter;
                },
                goodsCategoryList: function () {
                    return $scope.goodsCategoryList;
                }
            }
        });

        modalClassifieInstance.result.then(function (classifierFilter) {
            $scope.classifierFilter = classifierFilter;
            loadFilteredClassifier();
        }, function (classifierFilter) {
        });
    };


    //Проверяем классификатор на изменения
    $scope.CheckClassifier = function () {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/GoodsClassifierEditor/CheckClassifier/',
                data: json
            }).then(function (response) {
                var responseData = response.data;

                if (responseData.Success) {
                    var data = responseData.Data;

                    $scope.InnerCods.GoodKey = data.GoodKey;
                    $scope.InnerCods.OwnerTradeMarkId = data.OwnerTradeMarkId;
                    $scope.InnerCods.PackerId = data.PackerId;
                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    };


    //Провереяем классификатор на изменения
    $scope.AddClassifierAsk = function () {
        messageBoxService.showConfirm('Вы уверены, что хотите добавить новую запись?', 'Добавление')
            .then(function () { addClassifier(true) }, function () { });

    };

    $scope.MergeClassifierAsk = function (drugDesription) {
        messageBoxService.showConfirm('Объединить с существующим Good?\n' + drugDesription, 'Изменение')
            .then(MergeClassifier, function () { });
    };

    function showMessage(text) {
        messageBoxService.showInfo(text, 'Изменение');
    }

    //Проверяем можем ли мы пересоздать препарат, и не потребуется ли объединение с другим препаратом
    function checkRecreate() {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/GoodsClassifierEditor/CheckRecreate/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {

                    var data = responseData.Data;

                    if (!data.CanRecreate) {
                        showMessage('Нельзя объединять в пределах одного Good');
                        return;
                    }

                    if (data.NeedMerge) {
                        $scope.MergeClassifierAsk(data.DrugDescription);
                    } else {
                        ReCreateClassifier();
                    }
                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    //Объединяем с другим препаратом
    function MergeClassifier() {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/GoodsClassifierEditor/MergeClassifier/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {
                    var data = responseData.Data;
                    loadSelectedClassifier(data);
                    loadFilteredClassifier();
                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    //Пересоздаем препарат с новым LKCU
    function ReCreateClassifier() {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/GoodsClassifierEditor/ReCreateClassifier/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {
                    var data = responseData.Data;

                    loadSelectedClassifier(data);
                    loadFilteredClassifier();

                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    //Пробное добавление классификатора, чтобы узнать что изменилось
    function addClassifier(tryMode) {

        if ($scope.ExistingGoodsBrandValue !== null && $scope.ExistingGoodsBrandValue !== $scope.classifier.GoodsBrand.Value) {
            messageBoxService.showError('Внимание! Для данного сочетания наименования и правообладателя уже существует другой бренд!');
            return;
        }

        fillClassifierParameters();

        var json = JSON.stringify({ model: $scope.classifier, tryMode: tryMode });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/GoodsClassifierEditor/AddClassifier/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {
                    var data = responseData.Data;
                    ShowAddResult(data, tryMode);

                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }

    //Показать результат добавления
    function ShowAddResult(data, isTry) {

        var modalInstance =
            $uibModal.open({
                animation: true,
                templateUrl: 'Views/Classifier/GoodsClassifierEditor/_AddGoodsClassifierInfoView.html',
                controller: 'AddGoodsClassifierInfoController',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    classifierInfo: data,
                    canCancel: isTry
                }
            });

        modalInstance.result.then(function () {
            //Если это была пробная попытка, и пользователь выбрал ок, то вызываем добавление
            if (isTry)
                addClassifier(false);
            else {
                //Если это финальное добавление, то загружаем изменения
                loadFilteredClassifier(true);
            }
        }, function () {
            //Отменили выполнение
        });
    }


    //Запуск процесса изменения в классификаторе
    $scope.EditClassifier = function () {

        fillClassifierParameters();

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/GoodsClassifierEditor/GetChanges/',
                data: json
            }).then(function (response) {
                var responseData = response.data;

                if (responseData.Success) {

                    var data = responseData.Data;

                    if (data.Items.length > 0)
                        ShowChanges(data, true);
                    else {
                        messageBoxService.showInfo('Изменений не обнаружено', 'Изменение');
                    }
                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }

            }).catch(function (response) {
                errorHandlerService.showResponseError(response);
            });
    };

    //Показываем что изменилось и спрашиваем продолжить или нет
    function ShowChanges(data, isTry) {
        var modalInstance =
            $uibModal.open({
                animation: true,
                templateUrl: 'Views/Classifier/GoodsClassifierEditor/_GoodsChangeInfoView.html',
                controller: 'GoodsChangeInfoController',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    classifierInfo: data,
                    canCancel: isTry
                }
            });

        modalInstance.result.then(function () {
            //Если это была пробная попытка, и пользователь выбрал ок, то вызываем добавление
            if (isTry)
                editClassifierAsk();
            else {
                //Если это финальное добавление, то загружаем изменения
                loadSelectedClassifier(data);
            }
        }, function () {
            //Отменили выполнение
        });
    }


    function editClassifierAsk() {
        messageBoxService.showConfirm('Изменить с новым кодом?', 'Изменение')
            .then(checkRecreate, function (result) {
                if (result === 'no')
                    changeClassifier();
            });
    }

    //Запуск процесса изменения классификатора
    function changeClassifier() {

        var json = JSON.stringify({ model: $scope.classifier });

        $scope.classifierLoading =
            $http({
                method: 'POST',
                url: '/GoodsClassifierEditor/ChangeClassifier/',
                data: json
            }).then(function (response) {

                var responseData = response.data;

                if (responseData.Success) {
                    loadSelectedClassifier(responseData.Data);
                    loadFilteredClassifier();

                } else {
                    messageBoxService.showError(responseData.ErrorMessage);
                }
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
    }


    $scope.LoadInit = function () {
        var F_ClassifierId = $route.current.params["ClassifierId"];
        if (F_ClassifierId > 0) {
            $scope.classifierFilter.classifierId = F_ClassifierId;
            loadFilteredClassifier();
            return;
        }
        var F_Id = $route.current.params["Id"];
        if (F_Id > 0) {
            $scope.classifierFilter.classifierId = F_Id;
            loadFilteredClassifier();
            return;
        }
    };

    $scope.LoadInit();
}