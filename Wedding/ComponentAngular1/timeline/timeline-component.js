angular.element(document).ready(function () {

    angular.module('timeline-module-component', []).directive('timeline', function () {
        return {
            restrict: 'E',
            scope: {},
            templateUrl: '/ComponentAngular1/timeline/timeline-component.html',
            controller: function ($scope, $http, $attrs, $window) {

             
            }
        }
    });


    var docElement = document.getElementById("timeline-module");

    angular.bootstrap(docElement, ['timeline-module-component']);


});