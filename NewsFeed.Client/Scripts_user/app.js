/// <reference path="libs/angular.js" />

var gameApp = angular.module("gameApp", ['gameControllers']);

gameApp.config(["$routeProvider", function ($routeProvider) {
	    $routeProvider
			.when("/", {
			    templateUrl: "Scripts_user/partials/login.html",
			    controller: 'LoginController'
			})
            .when("/register", {
                templateUrl: "Scripts_user/partials/register.html",
                controller: 'LoginController'
            })
            .when("/menu", {
                templateUrl: "Scripts_user/partials/front.html",
                controller: 'MenuController'
            })
			//.when("/post/:id", { templateUrl: "scripts/partials/single-post.html", controller: SinglePostController })
			.otherwise({
			    redirectTo: "/"
			});
	}]);
