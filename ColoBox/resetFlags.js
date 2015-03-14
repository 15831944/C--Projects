var app = angular.module('resetFlags', ['ngResource']);

app.factory('ResetService', function ($http, $q) {
    function post(url, data) {
        var deferred = $q.defer();
        deferred.promise = $http.post(url, data).success(function (arg) {
            if (arg.statusMessage) {
                alert(arg.statusMessage);
                deferred.reject(arg.statusMessage);
            } else {
                deferred.resolve(arg);
            }
        }).error(function (err) {
            deferred.resolve(arg);
            alert(err);
        });

        return deferred.promise;
        //return promise;
    }
    return {
        resetFirstTimeFlag: function (data) {
            return post('/api/RMTS/resetFirstTimeFlag', data);            
        },
        UpdateNeedTrainingRecord: function (data) {
            return post('/api/RMTS/UpdateNeedTrainingRecord', data);            
        },
        changeUser: function (data) {
            return post('/api/RMTS/ChangeMomentUser', data);
        },
        resetPassword: function (data) {
            return post('/api/security/resetPassword', data);
        }
    }
});
app.controller('mainCtrl', function ($scope, ResetService) {
    $scope.resetFirstTime = false;
    $scope.resetNeedTraining = false;
    
    $scope.resetSelected = function (arg1, arg2) {
        var data = {isResetingFirstTime : $scope.resetFirstTime, isResetingNeedTraining: $scope.resetNeedTraining, moment: $scope.moment, userName: $scope.userName};
        ResetService.resetFirstTimeFlag(data).then(function () {
                ResetService.UpdateNeedTrainingRecord(data).then(function () {
                    alert('reset is done.');
                });
        }).reject(function(arg){
            alert(arg);   
        });
    }

    $scope.changeUser = function (arg1, arg2) {
        var data = { moment: $scope.moment, userName: $scope.userToAssign };
        ResetService.changeUser(data).then(function () {
            alert('user name changed.');
        });
    }

    $scope.resetPassword = function (arg1, arg2) {
        var data = { userName: $scope.userToResetPassword, Password: $scope.newPassword, SkipUserMustChangePassword: true };
        ResetService.resetPassword(data).then(function () {
            alert('password changed.');
        });
    }
    
});