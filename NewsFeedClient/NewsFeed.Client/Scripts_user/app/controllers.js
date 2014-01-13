/// <reference path="../libs/angular.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/sha1.js" />

var newsFeedControllers = angular.module('newsFeedControllers', []);

newsFeedControllers.controller('LoginController', ['$scope', '$http', '$location',
    function ($scope, $http, $location) {
        $scope.loginUser = function () {
            var hash = CryptoJS.SHA1($scope.user.password).toString();
            $http.post('http://newsfeed.apphb.com/api/Users/login', { username: $scope.user.username, authCode: hash })
                .success(function (data) {
                    localStorage.setItem("sessionKey", data.sessionKey);
                    $location.url('/menu');
                })
                .error(function (data) {
                    $scope.errorMessage = data.Message;
                });
        };
}]);

newsFeedControllers.controller('RegisterController', ['$scope', '$http', '$location', '$rootScope',
    function ($scope, $http, $location, $rootScope) {
        //Get and save avatars url into array
        $http.get('Scripts_user/libs/avatars.js')
                .success(function (pictures) {
                    var avatars = [];
                    _.each(pictures, function (pic) {
                        avatars.push(pic);
                    });
                    $scope.avatars = avatars;
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });
        //Choose avatar
        $scope.selectAvatar = function (index) {
            $scope.userAvatar = $scope.avatars[index];
        };

        $scope.registerUser = function () {
            var hash = CryptoJS.SHA1($scope.user.password).toString();
            $http.post('http://newsfeed.apphb.com/api/Users/register', { username: $scope.user.username, authCode: hash, avatar: $scope.userAvatar })
                .success(function (data) {
                    //$rootScope.sessionKey = data.sessionKey;
                    localStorage.setItem("sessionKey", data.sessionKey);
                    $location.url('/menu');
                })
                .error(function (data) {
                    $scope.errorMessage = data.Message;
                });

        };
    }]);

newsFeedControllers.controller('MenuController', ['$scope', '$http', '$location', '$route', '$rootScope',
    function ($scope, $http, $location, $route, $rootScope) {
        $scope.loggedUser = {
            username: "",
            avatar: "",
            wall: [],
            friends: [],
            receiveRequests: []
        };
        //var sessionKey = $rootScope.sessionKey;
        var sessionKey = localStorage.getItem("sessionKey");
        $scope.resultSearchShow = false;
        $scope.errorSearchShow = false;
        $scope.requestShow = false;
        $scope.friendsShow = false;
        $scope.wallShow = false;
                
        $http.get('http://newsfeed.apphb.com/api/Users/getUserBySessionKey/' + sessionKey)
            .success(function (user) {
                var currUser = {
                    username: user.username,
                    avatar: user.avatar,
                    wall: [],
                    friends: [],
                    receiveRequests: []
                };
                $scope.loggedUser = currUser;

                if ($scope.loggedUser.receiveRequests.length != 0) {
                    $scope.requestShow = true;
                }
                if ($scope.loggedUser.friends.length != 0) {
                    $scope.friendsShow = true;
                }
                if ($scope.loggedUser.wall.length != 0) {
                    $scope.wallShow = true;
                }
            })
            .error(function (er) {
                console.log(JSON.stringify(er));
            }).then(function () {
                $http.get('http://newsfeed.apphb.com/api/Users/getUserRequestsBySessionKey/' + sessionKey)
                   .success(function (requests) {
                       _.each(requests, function (request) {
                           $scope.loggedUser.receiveRequests.push({
                               id: request.Id,
                               title: request.Title,
                               senderId: request.SenderId,
                               senderName: request.SenderName,
                               senderAvatar: request.SenderAvatar,
                               answer: request.Answer
                           });
                       });
                                              
                       if ($scope.loggedUser.receiveRequests.length != 0) {
                           $scope.requestShow = true;
                       }
                   })
                   .error(function (er) {
                       console.log(JSON.stringify(er));
                   });

                $http.get('http://newsfeed.apphb.com/api/Users/getUserFriendsBySessionKey/' + sessionKey)
                    .success(function (friends) {
                        _.each(friends, function (friend) {
                            $scope.loggedUser.friends.push({
                                username: friend.username,
                                avatar: friend.avatar
                            });
                            _.each(friend.wall, function (post) {
                                $scope.loggedUser.wall.push({
                                    title: post.Title,
                                    content: post.Content,
                                    date: post.PublicDate,
                                    author: post.Author,
                                    avatar: post.Avatar
                                });
                            });
                        });
                                              
                        if ($scope.loggedUser.friends.length != 0) {
                            $scope.friendsShow = true;
                        }
                    })
                    .error(function (er) {
                        console.log(JSON.stringify(er));
                    });

                $http.get('http://newsfeed.apphb.com/api/Users/getUserWallBySessionKey/' + sessionKey)
                   .success(function (wall) {
                       _.each(wall, function (post) {
                           $scope.loggedUser.wall.push({
                               title: post.Title,
                               content: post.Content,
                               date: post.PublicDate,
                               author: post.Author,
                               avatar: post.Avatar
                           });
                       });
                       
                       if ($scope.loggedUser.wall.length != 0) {
                           $scope.wallShow = true;
                       }
                   })
                   .error(function (er) {
                       console.log(JSON.stringify(er));
                   });
                }
            );

        //Add single post
        $scope.addPost = function () {
            var url = 'http://newsfeed.apphb.com/api/Posts/addPost/' + sessionKey + '?title=' + $scope.post.title + '&content=' + $scope.post.content;
            $http.post(url)
                .success(function (data) {
                    $route.reload();
                    console.log(JSON.stringify(data));
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });
        };

        //Search user
        $scope.searchUser = function () {
            if ($scope.searchedUser) {
                $http.get('http://newsfeed.apphb.com/api/Users/getUser', { params: { username: $scope.searchedUser.username } })
                .success(function (data) {
                    if (data.length != 0) {
                        var users = [];
                        _.each(data, function (user) {
                            var found = _.findWhere($scope.loggedUser.friends, { username: user.username })
                            if ((found == undefined) && (user.username != $scope.loggedUser.username)) {
                                users.push({
                                    username: user.username,
                                    avatar: user.avatar,
                                })
                                $scope.resultUsers = users;
                                $scope.resultSearchShow = true;
                            }
                            else {
                                $scope.errorSearchShow = true;
                                $scope.errorMessage = found.username +" is already your friend";
                            }
                        });
                    }
                    else {
                        $scope.errorSearchShow = true;
                        $scope.errorMessage = "No such user";
                    }
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });
            }
            else {
                $scope.errorSearchShow = true;
                $scope.errorMessage = "No such user";
            }
        };

        $scope.makeRequest = function (index) {
            $http.post('http://newsfeed.apphb.com/api/Users/sendrequest/' + sessionKey, { username: $scope.resultUsers[index].username, avatar: $scope.resultUsers[index].avatar })
                .success(function (data) {
                    $scope.resultUsers.splice(index, 1);
                    if ($scope.resultUsers.length == 0) {
                        $scope.resultSearchShow = false;
                    }
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });
        };

        $scope.addFriend = function (senderId, id) {
            $http.post('http://newsfeed.apphb.com/api/Users/addFriend/' + sessionKey + '?senderId=' + senderId)
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
            $http({ method: 'DELETE', url: 'http://newsfeed.apphb.com/api/Users/deleteRequest?id=' + id })
                .success(function (data) {
                    $route.reload();
                    console.log(JSON.stringify(data));
                })
                .error(function (er) {
                    console.log(JSON.stringify(er));
                });
        };
        
        $scope.logout = function () {
            localStorage.clear();
            $location.path("/");
        };
}]);

