/// <reference path="libs/angular.js" />

var newsFeedModule = angular.module("newsFeedModule", ['newsFeedControllers','ngRoute']);

newsFeedModule.config(["$routeProvider", function ($routeProvider) {
    $routeProvider
        .when("/", {
            templateUrl: "Scripts_user/partials/login.html",
            controller: 'LoginController'
        })
        .when("/register", {
            templateUrl: "Scripts_user/partials/register.html",
            controller: 'RegisterController'
        })
        .when("/menu", {
            templateUrl: "Scripts_user/partials/front.html",
            controller: 'MenuController'
        })
        .otherwise({
            redirectTo: "/"
        });
}])
.run(function ($rootScope, $location) {
    $rootScope.$on("$locationChangeStart", function () {
        if (localStorage.getItem("sessionKey") != null) {
            $location.path("/menu");
        }
    });
});

