angular.module('DataAggregatorModule').directive('ckeditor', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ngModel) {
            var editorOptions;
            if (attr.ckeditor === 'minimal') {
                // minimal editor
                editorOptions = {
                    height: 500,
                    toolbar: [
                        { name: 'basic', items: ['Bold', 'Italic', 'Underline'] },
                        { name: 'links', items: ['Link', 'Unlink'] },
                        { name: 'tools', items: ['Maximize'] },
                        { name: 'document', items: ['Source'] },
                    ],
                    removePlugins: 'elementspath,exportpdf,easyimage,cloudservices',
                    resize_enabled: false
                };
            } else {
                // regular editor
                editorOptions = {
                    filebrowserImageUploadUrl: $rootScope.globals.apiUrl + '/upload',
                    removeButtons: 'About,Form,Checkbox,Radio,TextField,Textarea,Select,Button,ImageButton,HiddenField,Save,CreateDiv,Language,BidiLtr,BidiRtl,Flash,Iframe,addFile,Styles',
                    extraPlugins: 'simpleuploads,imagesfromword'
                };
            }

            // enable ckeditor
            //var CKEDITOR_BASEPATH = '/ckeditor/';
            var ckEditEl = CKEDITOR.replace(element[0], editorOptions);
           // var ckeditor = element.ckeditor(editorOptions);

            // update ngModel on change
            ckEditEl.on('change', function () {
                ngModel.$setViewValue(this.getData());
            });

            scope.$watch('ngModel',
                function (Value, oldV, sc) {
                    var jdata = "";
                    jdata = sc;
                   // this.setData(ngModel.$getViewValue);
                });

            scope.$watch(function () {
                if (ngModel.$modelValue === undefined)
                    return undefined;
                if (ngModel.$modelValue === ckEditEl.getData())
                {
                    return true;
                }
                return false;
            }, function (newVal, oldVal) {
                ckEditEl.setData(ngModel.$getViewValue);
                scope.triggered = false;
            }, true);
        }
    };
});