angular
    .module('DataAggregatorModule')
    .controller('RetailTemplatesController', ['$scope', '$http', 'uiGridCustomService', '$uibModal', 'messageBoxService', 'errorHandlerService', '$translate', RetailTemplatesController]);

function RetailTemplatesController($scope, $http, uiGridCustomService, $uibModal, messageBoxService, errorHandlerService, $translate) {

    $scope.selectedSource = null;
    $scope.selectedTemplate = null;
    $scope.templateFieldName = null;

    //Источники
    $scope.RetailTemplates_SourcesGrid = uiGridCustomService.createGridClassMod($scope, 'RetailTemplates_SourcesGrid');
    //свойства грида

    $scope.RetailTemplates_SourcesGrid.Options.enableRowSelection = true;
    $scope.RetailTemplates_SourcesGrid.Options.enableFullRowSelection = false;
    $scope.RetailTemplates_SourcesGrid.Options.enableRowHeaderSelection = true;
    $scope.RetailTemplates_SourcesGrid.Options.noUnselect = false;

    $scope.RetailTemplates_SourcesGrid.Options.columnDefs =
        [
            { name: 'Id', field: 'Id', enableCellEdit: false, width: 300, filter: { condition: uiGridCustomService.condition } },
            { name: 'Источник данных', field: 'Name', enableCellEdit: false, width: 300, filter: { condition: uiGridCustomService.condition } },
            { name: 'Приритет', field: 'Priority', enableCellEdit: true, width: 100, type: 'number', filter: { condition: uiGridCustomService.condition } },
            {
                name: 'Наличие данных поставщиков в Ecom', field: 'IsPutEcomData', enableCellEdit: true, width: 20, type: 'boolean'
            }
        ];

    $scope.RetailTemplates_SourcesGrid.SetDefaults();

    $scope.RetailTemplates_SourcesGrid.Options.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
        //Выбрали Источник
        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            if (row.isSelected) {
                $scope.selectedSource = row.entity;
                $scope.selectedTemplate = null;
                loadTemplates();
                loadTemplate();
            }
        });

        gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
            if (colDef.field !== '@modify') {
                if (newValue !== oldValue) {
                    rowEntity["@modify"] = true;
                    $scope.RetailTemplates_SourcesGrid.NeedSave = true;
                }
            }
        });
    };



    //Шаблоны

    //свойства грида
    $scope.gridTemplatesOptions = uiGridCustomService.createOptions('RetailTemplates_TemplatesGrid');

    var gridTemplatesOptions = {
        customEnableRowSelection: true,
        multiSelect: false,
        enableFullRowSelection: true,
        enableRowHeaderSelection: false,
        appScopeProvider: $scope,
        enableRowSelection: true,
        showGridFooter: false,
        noUnselect: true,
        columnDefs: [
            { name: 'Шаблоны', field: 'Name', enableCellEdit: true }
        ]
    };

    angular.extend($scope.gridTemplatesOptions, gridTemplatesOptions);


    $scope.gridTemplatesOptions.onRegisterApi = function (gridApi) {

        gridApi.selection.on.rowSelectionChanged($scope, selectTemplate);

        gridApi.edit.on.afterCellEdit($scope, renameTemplate);
    };


    //Функции по логике

    //Проставляет первый и последний элемент
    function setFirstAndLastElement(nodes) {

        if (nodes == null || nodes.length === 0)
            return;
        //Если был только 1 элемент, то он был и первым и последним
        if (nodes.length >= 2) {
            nodes[nodes.length - 2].IsFirst = false;
            nodes[nodes.length - 2].IsEnd = false;
            nodes[0].IsEnd = false;
        }

        nodes[0].IsFirst = true;
        nodes[nodes.length - 1].IsEnd = true;
    }

    //Сравнивалка полей по OrderNumber
    function compareTemplateFields(field1, field2) {

        if (field1.OrderNumber > field2.OrderNumber)
            return 1;

        if (field1.OrderNumber < field2.OrderNumber)
            return -1;

        return 0;
    }

    //Сортировка полей 
    function sortTemplateFields(node) {

        if (node.Childs == null || node.Childs.length === 0)
            return;

        //Отметим первый и последний элемент
        setFirstAndLastElement(node.Childs);

        node.Childs.sort(compareTemplateFields);

        node.Childs.forEach(function (item, i, arr) {
            sortTemplateFields(item);
        });

    }

    ///Создает экземпляр Node
    function getNewNode(parent, order) {
        return {
            Id: 0,
            TemplateId: $scope.selectedTemplate.Id,
            FieldName: null,
            ColumnNameInFile: null,
            OrderNumber: order,
            ParentId: parent != null ? parent.Id : null,
            Childs: []
        }
    }
    //Получаем максимальный порядковы номер коллекции
    function getMaxOrderNumber(nodes) {
        if (nodes == null || nodes.length === 0)
            return 0;

        return Math.max.apply(Math, nodes.map(function (o) { return o.OrderNumber; }));
    }

    //Получаем элемент слева
    function getLeftNode(nodes, currentOrderNumber) {

        var leftNode = {};

        var leftNodes = nodes.filter(function (value) {
            return value.OrderNumber < currentOrderNumber;
        });

        leftNode.OrderNumber = getMaxOrderNumber(leftNodes);

        //Берем сам элемент
        leftNode.Node = nodes.find(function (value) {
            return value.OrderNumber === leftNode.OrderNumber;
        });

        return leftNode;
    }

    ///Получить элемент справа
    function getRightNode(nodes, currentOrderNumber) {

        var rightNode = {};

        var rightNodes = nodes.filter(function (value) {
            return value.OrderNumber > currentOrderNumber;
        });

        rightNode.OrderNumber = Math.min.apply(Math, rightNodes.map(function (o) { return o.OrderNumber; }));

        //Берем сам элемент
        rightNode.Node = nodes.find(function (value) {
            return value.OrderNumber === rightNode.OrderNumber;
        });

        return rightNode;
    }


    //Функции загрузки данных

    //Загрузка источников

    // загрузить все источники
    function loadSources() {
        $scope.RetailTemplates_SourcesGrid.Options.data = [];

        $scope.sourceLoading = $http({
            method: "POST",
            url: "/RetailTemplates/GetAllSources/"
        }).then(function (response) {
            $scope.RetailTemplates_SourcesGrid.Options.data = response.data;
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    ///Загрузить все шаблоны выбранного источника
    function loadTemplates() {

        $scope.gridTemplatesOptions.data = [];

        if ($scope.selectedSource == null)
            return;

        $scope.templatesLoading = $http({
            method: "POST",
            url: "/RetailTemplates/GetTemplates/",
            data: JSON.stringify({ id: $scope.selectedSource.Id })
        }).then(function (response) {

            $scope.gridTemplatesOptions.data = response.data;
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    ///Загрузка выбранного шаблона
    function loadTemplate() {
        $scope.templateData = [];

        if ($scope.selectedTemplate == null)
            return;

        $scope.templateFieldLoading = $http({
            method: "POST",
            url: "/RetailTemplates/GetTemplate/",
            data: JSON.stringify({ id: $scope.selectedTemplate.Id })
        }).then(function (response) {
            $scope.templateData = response.data;
            $scope.templateData.sort(compareTemplateFields);
            setFirstAndLastElement($scope.templateData);
            $scope.templateData.forEach(function (item, i, arr) {
                sortTemplateFields(item);
            });

        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    //Загрузка типов Полей
    function loadTemplateFieldName() {
        $scope.templateFieldName = [];

        $scope.templateFieldLoading = $http({
            method: "POST",
            url: "/RetailTemplates/GetTemplateFieldName/"
        }).then(function (response) {
            $scope.templateFieldName = response.data;
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }

    //Загрузить выбранный шаблон

    //Инициализация

    loadSources();
    loadTemplateFieldName();

    //Методы 
    //Редактирование источников
    //Добавляем источник
    $scope.addSource = function () {

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Views/Retail/RetailTemplates/NewSource.html',
            size: 'sm',
            controller: 'NewSourceController',
            windowClass: 'center-modal',
            backdrop: 'static'
        });

        modalInstance.result.then(function (name) {
            $scope.loading = $http({
                method: 'POST',
                url: '/RetailTemplates/AddSource/',
                data: JSON.stringify({
                    name: name
                })
            }).then(function (response) {
                var data = response.data;
                if (data)
                    $scope.RetailTemplates_SourcesGrid.Options.data.push(response.data);
            }, function (response) {
                errorHandlerService.showResponseError(response);
            });
        }, function () {
        });
    }

    //Удаляем источник
    $scope.removeSource = function () {

        if ($scope.selectedSource == null)
            return;

        messageBoxService.showConfirm('Удалить источник?', 'Удаление').then(
            function () {
                var json = JSON.stringify({ id: $scope.selectedSource.Id });

                $scope.sourceLoading = $http({
                    method: "POST",
                    url: "/RetailTemplates/RemoveSource/",
                    data: json
                }).then(function () {
                    var index = $scope.RetailTemplates_SourcesGrid.Options.data.indexOf($scope.selectedSource);
                    $scope.RetailTemplates_SourcesGrid.Options.data.splice(index, 1);
                    $scope.selectedSource = null;
                    $scope.selectedTemplate = null;
                    loadTemplates();
                    loadTemplate();

                }, function (response) {
                    errorHandlerService.showResponseError(response);
                });
            });
    }

    //Сохраняем источник
    $scope.saveSources = function () {
        var array_upd = $scope.RetailTemplates_SourcesGrid.GetArrayModify();
        if (array_upd.length > 0) {
            $scope.dataLoading =
                $http({
                    method: 'POST',
                    url: '/RetailTemplates/SaveSources/',
                    data: JSON.stringify({ array: array_upd })
                }).then(function (response) {
                    messageBoxService.showInfo("Сохранено");
                    loadSources();
                }, function (response) {
                    errorHandlerService.showResponseError(response);
                    $scope.RetailTemplates_SourcesGrid.ClearModify();
                    loadSources();
                });
        }
    }

    //Редатирование шаблонов

    $scope.addTemplate = function () {

        var json = JSON.stringify({ sourceId: $scope.selectedSource.Id });

        $scope.templatesLoading = $http({
            method: "POST",
            url: "/RetailTemplates/AddTemplate/",
            data: json
        }).then(function (response) {
            $scope.gridTemplatesOptions.data.push(response.data);
        }, function (response) {
            errorHandlerService.showResponseError(response);
        });
    }


    $scope.removeTemplate = function () {

        var json = JSON.stringify({ id: $scope.selectedTemplate.Id });

        $scope.templatesLoading = $http({
            method: "POST",
            url: "/RetailTemplates/RemoveTemplate/",
            data: json
        }).then(function () {
            var index = $scope.gridTemplatesOptions.data.indexOf($scope.selectedTemplate);
            $scope.gridTemplatesOptions.data.splice(index, 1);
            $scope.selectedTemplate = null;
            loadTemplate();

        }, function (response) {
            errorHandlerService.showResponseError(response);
        });

    }

    function renameTemplate(rowEntity, colDef, newValue, oldValue) {

        var json = JSON.stringify({ id: rowEntity.Id, value: newValue });
        $scope.templatesLoading = $http({
            method: "POST",
            url: "/RetailTemplates/RenameTemplate/",
            data: json
        }).then(function () {
            return true;
        }, function (response) {
            errorHandlerService.showResponseError(response);
            rowEntity[colDef.field] = oldValue;
            $scope.$apply();
            return false;
        });
    }


    function selectTemplate(row) {
        if (row.isSelected) {
            $scope.selectedTemplate = row.entity;
            loadTemplate();
        }
    }


    //Редактор шаблона
    //Сместить влево или враво
    //scope - элемент дерева
    //direction - куда перемещать
    $scope.move = function (scope, direction) {

        if (scope == null)
            return;

        var node = scope.$modelValue;

        var parent = scope.$parentNodeScope != null ? scope.$parentNodeScope.$modelValue : null;

        var nodes = null;

        if (parent == null)
            nodes = $scope.templateData;
        else
            nodes = parent.Childs;

        //Поулчаем порядок текущего элемента

        var currentOrderNumber = node.OrderNumber;

        //Находим элемент с предыдущим или следующим порядком порядком

        var nodeForChange = null;

        if (direction === 'right')
            nodeForChange = getRightNode(nodes, currentOrderNumber);
        if (direction === 'left')
            nodeForChange = getLeftNode(nodes, currentOrderNumber);

        if (nodeForChange == null)
            return;
        //Меняем порядковый номер местами

        nodeForChange.Node.OrderNumber = currentOrderNumber;
        nodeForChange.Node.IsFirst = false;
        nodeForChange.Node.IsEnd = false;
        node.OrderNumber = nodeForChange.OrderNumber;
        node.IsFirst = false;
        node.IsEnd = false;

        //Сортируем
        nodes.sort(compareTemplateFields);


        setFirstAndLastElement(nodes);

    }

    //Удалить элемент
    $scope.remove = function (scope) {
        if (scope == null)
            return;

        scope.remove();

        var parent = scope.$parentNodeScope != null ? scope.$parentNodeScope.$modelValue : null;

        //Отметим первый и последний элемент
        setFirstAndLastElement(parent.Childs);
    };


    //Добавить дочерний элемент
    $scope.addField = function (scope) {
        var nextOrder = null;
        //Добавляеям дочерний элемент
        if (scope != null) {

            nextOrder = getMaxOrderNumber(scope.$modelValue.Childs) + 1;
            scope.$modelValue.Childs.push(getNewNode(scope.$modelValue, nextOrder));
            setFirstAndLastElement(scope.$modelValue.Childs);
        }
        //Добавляем верзний уровень
        else {
            nextOrder = getMaxOrderNumber($scope.templateData) + 1;
            $scope.templateData.push(getNewNode(null, nextOrder));
            setFirstAndLastElement($scope.templateData);
        }


    };



    //Восстановить шаблон
    $scope.refreshTemplate = function () {
        loadTemplate();
    }

    //Проверяет является ли элмент первым
    $scope.checkFirst = function (scope) {
        return scope.$modelValue.IsFirst;
    }

    //Проверяет является ли эленмент последним
    $scope.checkEnd = function (scope) {
        return scope.$modelValue.IsEnd;
    }


    $scope.saveTemplate = function () {

        var json = JSON.stringify({ templateFieldsJson: $scope.templateData, templateId: $scope.selectedTemplate.Id });
        $scope.templatesLoading = $http({
            method: "POST",
            url: "/RetailTemplates/SaveTemplate/",
            data: json
        }).then(function () {
            return true;
        }, function (response) {
            errorHandlerService.showResponseError(response);
            return false;
        });
    }
}

angular
    .module('DataAggregatorModule')
    .controller('NewSourceController', [
        '$scope', 'messageBoxService', '$uibModalInstance', NewSourceController]);

function NewSourceController($scope, messageBoxService, $modalInstance) {
    $scope.name = null;

    $scope.cancel = function () {
        $modalInstance.dismiss();
    };

    $scope.save = function () {
        if ($scope.name === null || $scope.name.trim().length === 0) {
            messageBoxService.showError('Не заполнено наименование источника', 'Ошибка');
            return;
        }
        else {
            $modalInstance.close($scope.name);
        }
    };
}