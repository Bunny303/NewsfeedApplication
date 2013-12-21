/// <reference path="../libs/angular.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/sha1.js" />

var gameControllers = angular.module('gameControllers', []);

gameControllers.controller('LoginController', ['$scope', '$http', '$location', 
function($scope, $http, $location) {
        $scope.user = {
            username: "",
            password: ""
        };
        $scope.errorMessage = "";
        
        //Get and save avatars url into array
        $http.get('Scripts_user/libs/avatars.js')
                .success(function (pictures) {
                    var avatars = [];
                    _.each(pictures, function (pic) {
                        avatars.push(pic);
                    })
                    $scope.avatars = avatars;
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });
        //Choose avatar
        $scope.selectAvatar = function (index) {
            localStorage.setItem("avatar", $scope.avatars[index]);
            //console.log($scope.user.avatar);
        };

        $scope.loginUser = function () {
            var hash = CryptoJS.SHA1($scope.user.password).toString();
            $http.post('http://localhost:39878/api/Users/login', { username: $scope.user.username, authCode: hash })
                .success(function (data) {
                    localStorage.setItem("sessionKey", data.sessionKey);
                    $location.url('/menu');
                })
                .error(function (data) {
                    $scope.errorMessage = data.Message;
                });

            $scope.user = {
                username: "",
                password: ""
            };
        };

        $scope.registerUser = function () {
            var hash = CryptoJS.SHA1($scope.user.password).toString();
            $http.post('http://localhost:39878/api/Users/register', { username: $scope.user.username, authCode: hash, avatar: localStorage.getItem("avatar")})
                .success(function (data) {
                    localStorage.setItem("sessionKey", data.sessionKey);
                    localStorage.removeItem("avatar");
                    $location.url('/menu');
                })
                .error(function (data) {
                    $scope.errorMessage = data.Message;
                });

            $scope.user = {
                username: "",
                password: ""
            };
        };
}]);

gameControllers.controller('MenuController', ['$scope', '$http', '$location', '$route',
    function ($scope, $http, $location, $route) {
        $scope.loggedUser = {
            //sessionKey
            username: "",
            avatar: "",
            wall: [],
            friends: [],
            receiveRequests: []
        };

        var sessionKey = localStorage.getItem("sessionKey");

        $http.get('http://localhost:39878/api/Users/getUserBySessionKey/' + sessionKey)
            .success(function (user) {
                var currUser = {
                    username: user.username,
                    avatar: user.avatar,
                    wall: [],
                    friends: [],
                    receiveRequests: []
                };
                _.each(user.wall, function (post) {
                    currUser.wall.push({
                        title: post.Title,
                        content: post.Content,
                        date: post.PublicDate,
                        author: post.Author,
                        avatar: post.Avatar
                    });
                });
                _.each(user.friends, function (friend) {
                    currUser.friends.push({
                        username: friend.username,
                        avatar: friend.avatar
                    });
                });
                _.each(user.receiveRequests, function (request) {
                    currUser.receiveRequests.push({
                        id: request.Id,
                        title: request.Title,
                        senderId: request.SenderId,
                        senderName: request.SenderName,
                        senderAvatar: request.SenderAvatar,
                        answer: request.Answer
                    });
                });
                $scope.loggedUser = currUser;
            })
            .error(function (er) {
                console.log(JSON.stringify(er));
            });
        
        //Add single post
        $scope.addPost = function () {
            var url = 'api/Posts/addPost/' + sessionKey + '?title=' + $scope.post.title + '&content=' + $scope.post.content;
            $http.post(url)
                .success(function (data) {
                    $route.reload();
                    console.log(JSON.stringify(data));
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });

            //$scope.post = {
            //    title: "",
            //    content: ""
            //};
        };

        //Search user
        //May be use $watch???
        $scope.searchUser = function () {
            $http.get('http://localhost:39878/api/Users/getUser?username=' + $scope.searchedUser.username)
            .success(function (data) {
                if (data !== "null"){
                    var user = {
                    username: data.username,
                    avatar: data.avatar
                    };
                    $scope.resultUser = user;
                }
                else {
                    var user = {
                        username: 'No such user',
                        avatar: 'No avatar'
                    };
                    $scope.resultUser = user;
                }
            })
            .error(function (er) {
                console.log(JSON.stringify(er));
            });
        };

        $scope.makeRequest = function () {
            $http.post('http://localhost:39878/api/Users/sendrequest/' + sessionKey, { username: $scope.resultUser.username, avatar: $scope.resultUser.avatar })
                .success(function (data) {
                    //show message
                    console.log(JSON.stringify(data));
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });
        };

        $scope.addFriend = function (senderId, id) {
            $http.post('http://localhost:39878/api/Users/addFriend/' + sessionKey + '?senderId=' + senderId)
                .success(function (data) {
                    $scope.deleteRequest(id);
                    $route.reload();
                    console.log(JSON.stringify(data));
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });
        };

        $scope.deleteRequest = function (id) {
            $http({ method: 'DELETE', url: 'http://localhost:39878/api/Users/deleteRequest?id=' + id })
                .success(function (data) {
                    $route.reload();
                    console.log(JSON.stringify(data));
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });
        };
        
}]);

