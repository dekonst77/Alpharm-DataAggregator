angular.module('DataAggregatorModule').constant('formatConstants',
    {
        FILTER_PRICE: 'number:2',
        FILTER_SUM: 'number:0',

        FILTER_Procent: 'number:2',

        FILTER_double: 'number:6',

        FILTER_INT_COUNT: '',//просто число не надо разделителей, применять для Id полей
        FILTER_FLOAT_COUNT: 'number:2',
        FILTER_FLOAT3_COUNT: 'number:3',

        FILTER_COEFFICIENT: 'number:2',
        FILTER_PERIOD_YMD: 'date:\'yyyy-MM-dd\'',
        FILTER_PERIOD_DATE: 'date:\'MM.yyyy\'',
        FILTER_PERIOD_YEAR: 'date:\'yyyy\'',

        FILTER_DATE: 'date:\'dd.MM.yyyy\'',
        FILTER_DATE_TIME: 'date:\'dd.MM.yyyy HH:mm:ss\'',
        FILTER_DATE_TIME_SM: 'date:\'dd.MM.yyyy HH:mm\'',

        cellTemplateURL: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}"><a href="{{COL_FIELD}}" target="_blank">ссылка</a></div>',
        //cellTemplateURL: '<div class="ui-grid-cell-contents">D{{COL_FIELD}}</div>',
        cellTemplateHint: '<div class="ui-grid-cell-contents" title="{{COL_FIELD}}">{{COL_FIELD}}</div>',
        FILTER_Bool: '<div style="height:100%;text-align:center" ng-class="{\'btn-danger\' : \'{{COL_FIELD}}\'===\'false\',\'btn-warning\' : \'{{COL_FIELD}}\'===\'\',\'btn-success\' : \'{{COL_FIELD}}\'===\'true\'}">{{COL_FIELD}}</div>'

        //cellTemplateCheck3: 
           //< button ng-class="{\'btn-danger\' : row.entity.isExists==false,\'btn-warning\' : row.entity.isExists==null,\'btn-info\' : row.entity.isExists==true}" style = "width:100%" class= "btn btn-sm" ng - click="grid.appScope.GS_periods(row.entity.Id)" > <span ng-class="{\'glyphicon glyphicon-usd\' : row.entity.Summa>0}" class=""></span> <span ng-class="{\'glyphicon glyphicon-random\' : row.entity.isExists!=row.entity.isExists_p1}" class=""></span>{{ row.entity.Id }}</button > '
    }
);