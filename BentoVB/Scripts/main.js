
var main = angular.module('main', [
    'ngResource',
    'ngRoute',
    'ngTable',
    'ngDialog',
    'ui.bootstrap'
]);

main.config(['$routeProvider', function ($routeProvider) {

    $routeProvider.when('/', {
        templateUrl: 'Templates/root.html',
        controller: 'UserCtrl'
    })

    .when('/User/:_id', {
        templateUrl: 'Templates/user.html',
        controller: 'OrderCtrl'
    })

    .when('/Summary', {
        templateUrl: 'Templates/summary.html',
        controller: 'SummaryCtrl'
    })

    .when('/Holiday', {
        templateUrl: 'Templates/holiday.html',
        controller: 'HolidayCtrl'
    })

    .otherwise({ redirectTo: '/' });
}]);

main.factory('User', ['$resource', function ($resource) {
    return $resource('/api/User/:id');
}]);
main.factory('UsersOrder', ['$resource', function ($resource) {
    return $resource('/api/User/:id/Order/:year/:month/:day');
}]);
main.factory('Calendar', ['$resource', function ($resource) {
    return $resource('/api/Calendar/:year/:month/:day');
}]);
main.factory('Summary', ['$resource', function ($resource) {
    return $resource(
        '/api/Summary/:action/:year/:month/:day',
        {},
        {
            byDate: {
                method: 'GET',
                params: { action: 'Date' },
                isArray: true
            },
            byUser: {
                method: 'GET',
                params: { action: 'User' },
                isArray: true
            },
            today: {
                method: 'GET',
                params: { action: 'Today' }
            }
        });
}]);

main.controller('UserCtrl', function ($scope, ngDialog, User) {
    $scope.users = User.query();
    $scope.openDialog = function () {
        $scope.newUser = {};
        $scope.dialog = ngDialog.open({ template: 'Templates/new_user.html', scope: $scope });
    };
    $scope.addUser = function () {
        User.save($scope.newUser, function (data, header) {
            $scope.users = User.query();
            $scope.dialog.close();
        }, function (err) {
            alert('error');
            $scope.dialog.close();
        });
    };
});

main.controller('OrderCtrl', function ($scope, $routeParams, User, UsersOrder) {
    var now = new Date();
    var year = now.getFullYear();
    var month = now.getMonth();
    var day = now.getDate();
    var today = new Date(year, month, day);
    var userId = $routeParams._id;
    var startDate = new Date(year, month, 21);
    if (startDate > today) {
        startDate = new Date(year, month - 1, 21);
    }
    $scope.userId = userId;
    $scope.user = User.get({ id: userId });
    $scope.startDate = startDate;
    $scope.endDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 20);
    $scope.orders = UsersOrder.query({ id: userId, year: startDate.getFullYear(), month: startDate.getMonth() + 1 });
    $scope.orders.$promise.then(function (orders) {
        var t = today.toDateString();
        for (i = 0; i < orders.length; i++) {
            if (new Date(orders[i].OrderDate).toDateString() === t) {
                $scope.todaysOrder = orders[i];
                break;
            }
        }
    });
    $scope.orderChange = function (order) {
        order.$save({ id: userId });
    };
    $scope.orderChangeToday = function () {
        $scope.todaysOrder.IsOrdered = !$scope.todaysOrder.IsOrdered;
        $scope.todaysOrder.$save({ id: userId });
    };
    $scope.previewMonth = function () {
        $scope.startDate = new Date($scope.startDate.getFullYear(), $scope.startDate.getMonth() - 1, 21);
        $scope.endDate = new Date($scope.startDate.getFullYear(), $scope.startDate.getMonth() + 1, 20);
        $scope.orders = UsersOrder.query({ id: userId, year: $scope.startDate.getFullYear(), month: $scope.startDate.getMonth() + 1 });
    };
    $scope.nextMonth = function () {
        $scope.startDate = new Date($scope.startDate.getFullYear(), $scope.startDate.getMonth() + 1, 21);
        $scope.endDate = new Date($scope.startDate.getFullYear(), $scope.startDate.getMonth() + 1, 20);
        $scope.orders = UsersOrder.query({ id: userId, year: $scope.startDate.getFullYear(), month: $scope.startDate.getMonth() + 1 });
    };
});

main.controller('HolidayCtrl', ['$scope', '$window', 'Calendar', function ($scope, $window, Calendar) {
    var year = new Date().getFullYear();
    $scope.year = year;
    $scope.years = [year - 1, year, year + 1, year + 2, year + 3];
    $scope.changeYear = function () {
        Calendar.query({ year: $scope.year }).$promise.then(function (calendar) {
            $scope.holidays = calendar.filter(function (d) { return d.Flag == 2; });
            $scope.working = calendar.filter(function (d) { return d.Flag != 2; });
        });
    };
    $scope.holiday = new Date(new Date().toDateString());
    $scope.working = $scope.holiday;
    $scope.addHoliday = function () {
        var w = $scope.holiday.getDay();
        if (w == 0 || w == 6) {
            $window.alert($scope.holiday + " is week day.");
            return;
        }
        Calendar.save({ Target: $scope.holiday, Flag: 2 }, function (data, header) {
            $scope.changeYear();
        });
    };
    $scope.addWorking = function () {
        var w = $scope.holiday.getDay();
        if (w != 0 && w != 6) {
            $window.alert($scope.holiday + " is week end.");
            return;
        }
        Calendar.save({ Target: $scope.holiday, Flag: 0 }, function (data, header) {
            $scope.changeYear();
        });
    };
    $scope.deleteCalendar = function (calendar) {
        console.log(calendar);
        var d = new Date(calendar.Target);
        Calendar.delete({ year: d.getFullYear(), month: d.getMonth() + 1, day: d.getDate() }, function (data, header) { $scope.changeYear(); });
    };
    $scope.changeYear();
}]);

main.controller('SummaryCtrl', function ($scope, Summary, Calendar) {
    var d = new Date();
    var year = d.getFullYear();
    var month = d.getMonth();
    var day = d.getDate();
    $scope.startDate = new Date(year, month, 21);
    if (day < 21) {
        $scope.startDate = new Date(year, month - 1, 21);
    }
    $scope.startYear = $scope.startDate.getFullYear();
    $scope.startMonth = $scope.startDate.getMonth() + 1;
    $scope.today = Summary.today({ year: year, month: month + 1, day: day });
    $scope.previewMonth = function () {
        $scope.startMonth--;
        if ($scope.startMonth < 1) {
            $scope.startMonth = 12;
            $scope.startYear--;
        }
        $scope.getSummary();
    }
    $scope.nextMonth = function () {
        $scope.startMonth++;
        if ($scope.startMonth > 12) {
            $scope.startMonth = 1;
            $scope.startYear++;
        }
        $scope.getSummary();
    }
    $scope.getSummary = function () {
        $scope.daysSummary = Summary.byDate({ year: $scope.startYear, month: $scope.startMonth });
        $scope.userSummary = Summary.byUser({ year: $scope.startYear, month: $scope.startMonth });
    }
    $scope.changeStatus = function () {
        var c = {};
        c.Target = $scope.today.Target;
        c.Flag = $scope.today.Flag == 0 ? 1 : 0;
        Calendar.save(c, function (d, e) {
            var d = new Date($scope.today.Target);
            $scope.today = Summary.today({ year: d.getFullYear(), month: d.getMonth() + 1, day: d.getDate() });
        });
    }
    $scope.getSummary();
});
