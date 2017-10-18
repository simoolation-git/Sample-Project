angular.element(document).ready(function () {

    angular.module('group-error-component', []).directive('grouperror', function () {
        return {
            restrict: 'E',
            scope: {},
            templateUrl: '/Scripts/partials/grouperror.html',
            controller: function ($scope, $http, $attrs) {

                $scope.errors = [];

             
                amplify.subscribe("errors", function (errors) {
                    $scope.errors = errors;
                    $scope.$apply();
                });

            }
        }
    });


    var docElement = document.getElementById("group-error-module");

    angular.bootstrap(docElement, ['group-error-component']);


});