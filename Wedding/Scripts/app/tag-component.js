angular.element(document).ready(function () {

    var app = angular.module('tag-module-component', []).directive('tagtool', function () {
        return {
            restrict: 'E',
            scope: {},
            templateUrl: '/Scripts/partials/tag.html',
            controller: function ($scope, $http, $attrs, $window) {

                $scope.editMode = true;
                $scope.tags = [];

                $scope.DisplayTextArea = function () {
                    $scope.editMode = true;
                    var textArea = $('.tag-wrapper .text-area-tags');
                    textArea.focus();
                    setTimeout(function () {
                        textArea.focus();
                    }, 1);
                }

                $scope.ParseContent = function () {
                    ConvertToTags();
                }

                function ConvertToTags() {
                    if ($scope.textEntered != null && $scope.textEntered) {
                        $scope.tags = $scope.textEntered.trim().split(" ");
                        $scope.editMode = false;
                        amplify.publish('tags', $scope.tags);
                    }
                }

                amplify.subscribe("clearTags", function () {
                    $scope.tags = [];
                    $scope.textEntered = "";
                    $scope.$apply();
                });

                amplify.subscribe("setTags", function (tags, textEntered) {
                    $scope.tags = tags;
                    $scope.textEntered = textEntered.trim();
                    $scope.editMode = false;
                    $scope.$apply();
                });
            }
        }
    });

    app.directive('ngEnter', function () {
        return function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.ngEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    });

    var docElement = document.getElementById("tag-module");

    if (docElement)
        angular.bootstrap(docElement, ['tag-module-component']);
});