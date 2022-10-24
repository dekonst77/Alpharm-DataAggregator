// Extending Array prototype with new function,
// if that function is already defined in "Array.prototype", 
// then "Object.defineProperty" will throw an exception
Object.defineProperty(Array.prototype, "removeitem", {
    // Specify "enumerable" as "false" to prevent function enumeration
    enumerable: false,

    /**
    * Removes all occurence of specified item from array
    * @this Array
    * @param itemToRemove Item to remove from array
    * @returns {Number} Count of removed items
    */
    value: function (itemToRemove) {
        // Count of removed items
        var removeCounter = 0;

        // Iterate every array item
        for (var index = 0; index < this.length; index++) {
            // If current array item equals itemToRemove then
            if (this[index] === itemToRemove) {
                // Remove array item at current index
                this.splice(index, 1);

                // Increment count of removed items
                removeCounter++;

                // Decrement index to iterate current position 
                // one more time, because we just removed item 
                // that occupies it, and next item took it place
                index--;
            }
        }

        // Return count of removed items
        return removeCounter;
    }
});




var getDateNow = function() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd;
    }

    if (mm < 10) {
        mm = '0' + mm;
    }

    return yyyy + '-' + mm + '-' + dd;
}

// It's a constructor, not just a function
function CommonGridClass($scope) {

    var selected = null;

    var selectedItem = null;

    var gridApi = null;


    var selectionChanged = null;

    this.getSelected = function() {
        return selected;
    }

    this.getSelectedItem = function () {
        return selectedItem;
    }

    this.setFirstSelected = function() {
        gridApi.grid.modifyRows(this.Options.data);
        gridApi.selection.selectRow(this.Options.data[0]);

    }
   

    this.Options = {
        customEnableRowSelection: true,
        enableColumnResizing: true,
        enableGridMenu: true,
        enableSorting: true,
        enableFiltering: true,
        enableSelectAll: false,
        enableRowSelection: true,
        enableRowHeaderSelection: false,
        enableCellEdit: false,
        appScopeProvider: $scope,
        enableFullRowSelection: true,
        enableSelectionBatchEvent: true,
        enableHighlighting: true,
        modifierKeysToMultiSelect: true,
        multiSelect: true,
        noUnselect: false,
        rowTemplate: '/Views/Static/GridRow.html'
    };

    this.clearSelection = function() {
       // selectedItem = null;
        gridApi.selection.clearSelectedRows();
    }

    Object.defineProperty(this, "selectionChanged", {
        get: function () {
            return selectionChanged;
        },
        set: function (value) {
            selectionChanged = value;
        },
        enumerable: true

    });


    this.Options.onRegisterApi = function (api) {

        //set gridApi on scope
        gridApi = api;

        //Что-то выделили
        gridApi.selection.on.rowSelectionChanged($scope, function () {

            selectedItem = gridApi.selection.getSelectedRows();

            selected = gridApi.selection.getSelectedRows().map(function (value) {
                return value.Id;
            });

            if (selectionChanged != null)
                selectionChanged();

        });

        //Что-то выделили
        gridApi.selection.on.rowSelectionChangedBatch($scope, function () {

            selectedItem = gridApi.selection.getSelectedRows();

            selected = gridApi.selection.getSelectedRows().map(function (value) {
                return value.Id;
            });
        });

        //Очищает выделенное когда изменяется фильтр
        gridApi.core.on.filterChanged($scope, function () {
            selectedItem = null;
            gridApi.selection.clearSelectedRows();
        });


    };
}


var dictTypeHead = function () {
    this.DictionaryData = null;
    this.selected = null;
    this.selectedName = null;

    this.setSelected = function (item) {
        this.selected = item;
    }

    this.textChange = function () {
        this.selected = null;
    }

    this.clear=function() {
        this.selected = null;
        this.selectedName = null;
    }
}


var dictClass = function () {
    this.DictionaryData = null;
    this.selected = null;
    this.clear = function () {
        this.selected = null;
    }
}


var dateClass = function () {

    this.Value = null;

    this.Opened = false;
    this.Open = function () {
        this.Opened = !this.Opened;
    }

    this.setToday = function () {
        this.Value = new Date();
    }

    this.setTodayWithoutTime = function () {
        this.Value = new Date();
        this.Value = new Date(this.Value.getFullYear(), this.Value.getMonth(), this.Value.getDate());
    }

   this.setNull = function() {
       this.Value = null;
   }

};
