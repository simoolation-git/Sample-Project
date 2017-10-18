$(document).ready(function () {

    //$('.dropdown.nav-item').on({
    //    "shown.bs.dropdown": function () { this.closable = false; },
    //    "click": function () { this.closable = true; },
    //    "hide.bs.dropdown": function () { return this.closable; }
    //});
    amplify.subscribe("addButtonIsAdded", function () {
        $('.add-button a').tooltip();
    });


});