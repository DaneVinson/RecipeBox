///<reference path='./services/coreservice.ts'/>
///<reference path='./services/accountservice.ts'/>
///<reference path='./services/tagsservice.ts'/>
///<reference path='./services/recipesservice.ts'/>
///<reference path='./services/authinterceptorservice.ts'/>
///<reference path='./directives/emaildirective.ts'/>
///<reference path='./directives/hasfocusdirective.ts'/>
///<reference path='./directives/matchdirective.ts'/>
///<reference path='./directives/quantitydirective.ts'/>
///<reference path='./filters/filters.ts'/>
///<reference path='./controllers/navcontroller.ts'/>
///<reference path='./controllers/accountcontroller.ts'/>
///<reference path='./controllers/tagscontroller.ts'/>
///<reference path='./controllers/recipescontroller.ts'/>
///<reference path='./controllers/recipeeditcontroller.ts'/>
///<reference path='./controllers/recipedisplaycontroller.ts'/>
///<reference path='./controllers/aboutcontroller.ts'/>
///<reference path='./controllers/homecontroller.ts'/>

angular.module('AppServices', ['LocalStorageModule'])
    .service('coreService', ['$location', 'localStorageService', Services.CoreService])
    .service('accountService', ['$http', Services.AccountService])
    .service('tagsService', ['$http', Services.TagsService])
    .service('recipesService', ['$http', 'coreService', Services.RecipesService])
    .factory('authInterceptorService', ['$q', '$location', 'coreService', function ($q: ng.IQService, $location: ng.ILocationService, coreService: Services.CoreService) {
        var self = this;

        // add authorization header
        self.request = function (config: ng.IRequestConfig) {
            if (coreService.isAuthenticated()) {
                config.headers.Authorization = 'Bearer ' + coreService.authInfo.accessToken;
            }
            return config;
        }

        // watch for failures due 410 unauthorized
        self.responseError = function (rejection) {
            if (rejection.status === 401) {
                $location.path('/account');
            }
            return $q.reject(rejection);
        }
        return self;
}]);
    // TODO: why doesn't this work? Because it's a .factory?
    //.factory('authInterceptorService', ['$q', '$location', 'coreService', Services.AuthInterceptorService]);

angular.module('AppDirectives', [])
    .directive('email', function () {
        return new Directives.Email();
    })
    .directive('hasFocus', function ($timeout, $rootScope) {
        return new Directives.HasFocus($timeout, $rootScope);
    })
    .directive('match', function () {
        return new Directives.Match();
    })
    .directive('quantity', function () {
        return new Directives.Quantity();
    });

angular.module('AppControllers', ['AppServices', 'AppFilters', 'AppDirectives'])
    .controller(Controllers);

angular.module('AppConfig', ['AppControllers', 'AppServices'])
    .config(function ($routeProvider) {
        $routeProvider
            .when('/about', { controller: Controllers.AboutController, templateUrl: './app/views/about.html' })
            .when('/account', { controller: Controllers.AccountController, templateUrl: './app/views/account.html' })
            .when('/activationfail', { controller: null, templateUrl: './app/views/accountactivationfail.html' })
            .when('/contact', { controller: null, templateUrl: './app/views/contact.html' })
            .when('/home', { controller: Controllers.HomeController, templateUrl: './app/views/home.html' })
            .when('/recipes/display/:id', { controller: Controllers.RecipeDisplayController, templateUrl: './app/views/recipedisplay.html' })
            .when('/recipes/edit/:id', { controller: Controllers.RecipeEditController, templateUrl: './app/views/recipeedit.html' })
            .when('/recipes', { controller: Controllers.RecipesController, templateUrl: './app/views/recipes.html' })
            .when('/tags', { controller: Controllers.TagsController, templateUrl: './app/views/tags.html' })
            .otherwise({ redirectTo: '/home' });
    })
    .config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push('authInterceptorService');
    }]);

// bring the clinet into scope
var RecipeBoxClient = angular.module('RecipeBoxClient', ['ngRoute', 'ngAnimate', 'AppConfig', 'angularFileUpload']);

// enable bootstrap tooltips.
$('[data-toggle="tooltip"]').tooltip({ 'placement': 'top' });

// fix bootstrap navbar collapse issue for small form factor
$(document).on('click', '.navbar-collapse.in', function (e) {
    if ($(e.target).is('a') || $(e.target).is('span')) {
        $(this).collapse('hide');
    }
});