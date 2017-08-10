// bootstrap angular
var app = angular.module("app", []);

app.controller("UserController", ["$scope", "$http", function ($scope, $http) {
    
    // error msg
    $scope.status = {
        validationErrors : ""
    };
    
    // define user model
    $scope.model = {};
    
    // add user
    $scope.addUser = function () {
        
        // reset errors
        $scope.status.validationErrors = "";
        
        var model = $scope.model;
        
        // check if all fields all filled
        if ($scope.frm.$valid) {
            
            // use FormData to upload image
            var fd = new FormData();
            fd.append("firstName", model.firstName);
            fd.append("lastName", model.lastName);
            fd.append("age", model.age);
            fd.append("email", model.email);
            var image = $('#image')[0].files[0];
            fd.append("image", image);
            
            // save user
            $http.post("/api/users", fd, {
                headers: {'Content-Type': undefined },
                transformRequest: angular.identity
            }).success(function (res) {
                if (res && res.code)
                {
                    $scope.status.validationErrors = res.error;
                }
                else
                {
                    $scope.refreshUserList();
                    // reset model
                    model.firstName = "";
                    model.lastName = "";
                    model.age = "";
                    model.email = "";
                    $('#image')[0].value = "";
                }
            });
        }
    };
    
    // funciton to refresh user listing
    $scope.refreshUserList = function () {
        $http.get("/api/users")
            .success(function (res) {
                if (res && !res.code)
                {
                    $scope.userList =  res;
                }
            });
    };
    
}]);