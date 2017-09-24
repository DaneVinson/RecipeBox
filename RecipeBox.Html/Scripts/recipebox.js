var Services;
(function (Services) {
    var CoreService = (function () {
        function CoreService(locationService, localStorageService) {
            this.authInfo = {
                accessToken: '',
                expirationDate: new Date(),
                tokenType: '',
                userName: ''
            };
            this.authLocalStoreName = 'IAuthInfo';
            this.authText = 'Log In';
            this.azureImagesUri = 'https://recipeboxstorage.blob.core.windows.net/';
            this.localStorageService = localStorageService;
            this.locationService = locationService;
            this.uniqueId = -1;
            // check local storage and set authorization
            var localAuthInfo = this.localStorageService.get(this.authLocalStoreName);
            // JSON data doesn't convert to dates that compare correctly with Date
            if (localAuthInfo && localAuthInfo.expirationDate) {
                localAuthInfo.expirationDate = new Date(localAuthInfo.expirationDate.toString());
            }
            // set auth
            this.setAuthenticated(localAuthInfo);
            // refresh the auth token if necessary
            this.checkAndRefreshAuthToken();
        }
        // method called from the ctor which attempts to update the auth token if it exists and will expire withing 12 hours.
        CoreService.prototype.checkAndRefreshAuthToken = function () {
            var nowPlus12Hours = new Date();
            nowPlus12Hours.setHours(nowPlus12Hours.getHours() + 12);
            if (this.isAuthenticated() && this.authInfo.expirationDate < nowPlus12Hours) {
                // TODO: refresh bearer token
            }
        };
        // copy all vales from a source entity to a target entity.
        CoreService.prototype.copyEntity = function (source, target) {
            target.isBusy = source.isBusy;
            target.isDelete = source.isDelete;
            target.isEditing = source.isEditing;
            target.isModify = source.isModify;
            target.isNew = source.isNew;
            target.isSelected = source.isSelected;
            target.isValid = source.isValid;
            target.keyName = source.keyName;
        };
        // returns a flag indicating whether or not credentials are currently saved.
        CoreService.prototype.credentialsAreSaved = function () {
            var localAuthInfo = this.localStorageService.get(this.authLocalStoreName);
            return localAuthInfo != null;
        };
        // remove any authInfo in local storage
        CoreService.prototype.deleteLocalCredentials = function () {
            this.localStorageService.remove(this.authLocalStoreName);
        };
        // method to control the "state" of editable entities based on properties sent from their form controller
        CoreService.prototype.entityChange = function (entity, controller) {
            if (!entity || !controller) {
                return;
            }
            entity.isModify = controller.$dirty;
            entity.isValid = controller.$valid;
        };
        // method to return a flag indicating whenther or not an entity's delete operation should be enabled.
        CoreService.prototype.entityDeleteEnabled = function (entity) {
            return entity && !entity.isBusy && !entity.isDelete;
        };
        // method to return a flag indicating whenther or not an entity's refresh operation should be enabled.
        CoreService.prototype.entityRefreshEnabled = function (entity) {
            return entity && !entity.isBusy && (entity.isDelete || entity.isModify);
        };
        // method to return a flag indicating whenther or not an entity's save operation should be enabled.
        CoreService.prototype.entitySaveEnabled = function (entity) {
            return entity && !entity.isBusy && (entity.isDelete || (entity.isModify && entity.isValid));
        };
        // method to get the path of an image using its name.
        CoreService.prototype.getRecipeImagePath = function (imageName) {
            var host = this.locationService.host();
            // localhost use local images otherwise use azure.
            if (host && host === 'localhost') {
                return imageName ? './RecipeImages/' + imageName : './Images/default-recipe.png';
            }
            else {
                return imageName ? this.azureImagesUri + 'recipeimages/' + imageName : this.azureImagesUri + 'siteimages/default-recipe.png';
            }
        };
        CoreService.prototype.getSiteImagePath = function (imageName) {
            var host = this.locationService.host();
            // localhost use local images otherwise use azure.
            if (host && host === 'localhost') {
                return imageName ? './Images/' + imageName : './Images/default-recipe.png';
            }
            else {
                return imageName ? this.azureImagesUri + 'siteimages/' + imageName : this.azureImagesUri + 'siteimages/' + 'default-recipe.png';
            }
        };
        // initialize an IEditableEntity
        CoreService.prototype.initializeEntity = function (entity) {
            entity.isBusy = false;
            entity.isDelete = false;
            entity.isEditing = false;
            entity.isModify = false;
            entity.isNew = false;
            entity.isSelected = false;
            entity.isValid = true;
            entity.keyName = '';
        };
        // check authentication condition
        CoreService.prototype.isAuthenticated = function () {
            return this.authInfo &&
                this.authInfo.userName &&
                this.authInfo.accessToken &&
                this.authInfo.accessToken.length > 0 &&
                new Date() < this.authInfo.expirationDate;
        };
        // log the current user out
        CoreService.prototype.logOut = function () {
            this.setAuthenticated(null);
            this.deleteLocalCredentials();
            this.locationService.path('/');
        };
        // get a new empty entity with all properties set to type defaults
        CoreService.prototype.newEntity = function () {
            return {
                isBusy: false,
                isDelete: false,
                isEditing: false,
                isModify: false,
                isNew: false,
                isSelected: false,
                isValid: false,
                keyName: ''
            };
        };
        // save the current authInfo credentials object if authenticated
        CoreService.prototype.saveCurrentCredentials = function () {
            if (this.isAuthenticated()) {
                this.localStorageService.set(this.authLocalStoreName, this.authInfo);
            }
        };
        // set the current instance authentication based on the current value of authInfo
        CoreService.prototype.setAuthenticated = function (authInfo) {
            this.authInfo = authInfo;
            this.authText = this.isAuthenticated() ? this.authInfo.userName : 'Log In';
        };
        return CoreService;
    }());
    CoreService.$inject = ['$location', 'localStorageService'];
    Services.CoreService = CoreService;
})(Services || (Services = {}));
var Services;
(function (Services) {
    // Angular service class for interfacing with the account REST service.
    var AccountService = (function () {
        function AccountService(http) {
            this.authenticationAttempts = 0;
            this.httpService = http;
            this.tokenUrl = './token';
            this.urlBase = './api/account';
        }
        // authenticate
        AccountService.prototype.authenticateAccount = function (userName, password) {
            var authData = "grant_type=password&username=" + userName + "&password=" + password;
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } };
            //return this.httpService.post(this.tokenUrl, data, config);
            var self = this;
            return this.httpService.post(this.tokenUrl, authData, config)
                .success(function () {
                self.authenticationAttempts = 0;
            })
                .error(function () {
                self.authenticationAttempts++;
            });
        };
        // delete a account by id
        AccountService.prototype.deleteAccount = function (id) {
            return this.httpService.delete(this.urlBase + '/' + id);
        };
        // get a account by user name
        AccountService.prototype.getAccount = function (userName) {
            return this.httpService.get(this.urlBase + '/' + userName);
        };
        // post a new account
        AccountService.prototype.postAccount = function (account) {
            return this.httpService.post(this.urlBase, account);
        };
        // put to update
        AccountService.prototype.putAccount = function (account) {
            return this.httpService.put(this.urlBase, account);
        };
        return AccountService;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    AccountService.$inject = ['$http'];
    Services.AccountService = AccountService;
})(Services || (Services = {}));
var Services;
(function (Services) {
    // Angular service class for interfacing with the tags REST service.
    var TagsService = (function () {
        function TagsService(http) {
            this.httpService = http;
            this.urlBase = './api/tags';
        }
        // delete a tag by id
        TagsService.prototype.deleteTag = function (id) {
            return this.httpService.delete(this.urlBase + '/' + id);
        };
        // get a tag by id
        TagsService.prototype.getTag = function (id) {
            return this.httpService.get(this.urlBase + '/' + id);
        };
        // get all tags
        TagsService.prototype.getTags = function () {
            return this.httpService.get(this.urlBase);
        };
        // post a new tag
        TagsService.prototype.postTag = function (tag) {
            return this.httpService.post(this.urlBase, tag);
        };
        // put to update
        TagsService.prototype.putTag = function (tag) {
            return this.httpService.put(this.urlBase, tag);
        };
        return TagsService;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    TagsService.$inject = ['$http'];
    Services.TagsService = TagsService;
})(Services || (Services = {}));
var Services;
(function (Services) {
    // Angular service class for interfacing with the recipes REST service.
    var RecipesService = (function () {
        function RecipesService(http, coreService) {
            this.urlBase = './api/recipes';
            this.coreService = coreService;
            this.httpService = http;
            this.recipes = [];
        }
        // create a new IRecipeDto object from an input recipe
        RecipesService.prototype.createDtoFromRecipe = function (recipe) {
            return {
                Description: recipe.Description,
                Directions: recipe.Directions,
                Id: recipe.Id,
                ImageFileName: recipe.ImageFileName,
                Ingredients: recipe.Ingredients,
                Name: recipe.Name,
                Notes: recipe.Notes,
                PreparationMinutes: recipe.PreparationMinutes,
                RowVersion: recipe.RowVersion,
                Servings: recipe.Servings,
                Source: recipe.Source,
                Tags: recipe.Tags,
                AccountId: recipe.AccountId
            };
        };
        // delete a recipe by id
        RecipesService.prototype.deleteRecipe = function (id) {
            for (var i = 0; i < this.recipes.length; i++) {
                if (this.recipes[i].Id == id) {
                    this.recipes.splice(i, 1);
                }
            }
            return this.httpService.delete(this.urlBase + '/' + id);
        };
        RecipesService.prototype.getRecipe = function (id, callback, forceRefresh) {
            var _this = this;
            if (callback === void 0) { callback = null; }
            if (forceRefresh === void 0) { forceRefresh = false; }
            var cacheIndex = -1;
            var recipe = null;
            // look for the requested recipe in the cache
            if (this.recipes.length > 0) {
                for (var i = 0; i < this.recipes.length; i++) {
                    if (this.recipes[i].Id == id) {
                        cacheIndex = i;
                        break;
                    }
                }
            }
            // if the recipe wasn't found in the cache, forceRefresh or it isn't complete fetch the full recipe
            if (cacheIndex < 0 || forceRefresh || !this.recipes[cacheIndex].Ingredients || this.recipes[cacheIndex].Ingredients.length == 0) {
                this.getRecipeAsync(id)
                    .success(function (data, status, headers, config) {
                    if (data !== null) {
                        _this.coreService.initializeEntity(data);
                        data.isBusy = false;
                        data.keyName = 'recipe' + data.Id;
                        for (var i = 0; i < data.Ingredients.length; i++) {
                            _this.coreService.initializeEntity(data.Ingredients[i]);
                            data.Ingredients[i].isBusy = false;
                            data.Ingredients[i].keyName = 'ingredient' + data.Ingredients[i].Id;
                        }
                        recipe = data;
                        if (cacheIndex < 0) {
                            _this.recipes.push(data);
                        }
                        else {
                            _this.recipes[cacheIndex] = data;
                        }
                    }
                })
                    .error(function (data, status, headers, config) {
                })
                    .finally(function () {
                    callback(recipe);
                    return recipe;
                });
            }
            else {
                callback(this.recipes[cacheIndex]);
            }
        };
        // get a recipe by id
        RecipesService.prototype.getRecipeAsync = function (id) {
            return this.httpService.get(this.urlBase + '/' + id);
        };
        RecipesService.prototype.getRecipesAsync = function (nameFilter, tag) {
            return this.httpService.get(this.urlBase + '/filtered', { params: { NameFilter: nameFilter, TagId: tag.Id } });
        };
        // create a new recipe
        RecipesService.prototype.newRecipe = function () {
            var recipe = {
                Description: '',
                Directions: '',
                GraphEntity: this.coreService.newEntity(),
                Id: this.coreService.uniqueId--,
                ImageFileName: '',
                Ingredients: [],
                isBusy: false,
                isDelete: false,
                isEditing: false,
                isModify: true,
                isNew: true,
                isSelected: false,
                isValid: false,
                keyName: 'newRecipe' + Math.abs(this.coreService.uniqueId),
                Name: '',
                Notes: '',
                PreparationMinutes: 0,
                RowVersion: [],
                Servings: 0,
                Source: '',
                Tags: [],
                AccountId: '1234567890'
            };
            this.coreService.copyEntity(recipe, recipe.GraphEntity);
            return recipe;
        };
        // post a new recipe
        RecipesService.prototype.postRecipeAsync = function (recipe) {
            return this.httpService.post(this.urlBase, recipe);
        };
        // put to update a recipe
        RecipesService.prototype.putRecipeAsync = function (recipe) {
            return this.httpService.put(this.urlBase, recipe);
        };
        return RecipesService;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    RecipesService.$inject = ['$http', 'coreService'];
    Services.RecipesService = RecipesService;
})(Services || (Services = {}));
// TODO: Why isn't this working as a .factory reference from app.ts
var Services;
(function (Services) {
    // Interceptor service to handle auth demands
    var AuthInterceptorService = (function () {
        function AuthInterceptorService(q, location, coreService) {
            this.coreService = coreService;
            this.locationService = location;
            this.qService = q;
        }
        AuthInterceptorService.prototype.request = function (config) {
            if (this.coreService.isAuthenticated()) {
                config.headers.Authorization = 'Bearer ' + this.coreService.authInfo.accessToken;
            }
            return config;
        };
        AuthInterceptorService.prototype.responseError = function (rejection) {
            if (rejection.status === 401) {
                this.locationService.path('/account');
            }
            return this.qService.reject(rejection);
        };
        return AuthInterceptorService;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    AuthInterceptorService.$inject = ['$q', '$location', 'coreService'];
    Services.AuthInterceptorService = AuthInterceptorService;
})(Services || (Services = {}));
var Directives;
(function (Directives) {
    // Directive to validate a sting "looks" like an email address
    var Email = (function () {
        function Email() {
            this.require = 'ngModel';
            this.restrict = 'A';
        }
        // TODO: I think I can delete this.
        // email validation useing regex, source: The Code Project article "Email Address Validation Using Regular Expression" by Mykola Dobrochynskyy
        Email.prototype.emailIsValid = function (value) {
            return /^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$/.test(value);
        };
        Email.prototype.link = function (scope, elem, attrs, ctrl) {
            // validity function
            var emailIsValid = function (value) {
                // source: The Code Project article "Email Address Validation Using Regular Expression" by Mykola Dobrochynskyy
                return /^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$/.test(value);
            };
            // dom to model validation
            ctrl.$parsers.unshift(function (value) {
                var valid = emailIsValid(value);
                ctrl.$setValidity('email', valid);
                return valid ? value : undefined;
            });
            // model to dom validation
            ctrl.$formatters.unshift(function (value) {
                var valid = emailIsValid(value);
                ctrl.$setValidity('email', valid);
                return value;
            });
        };
        return Email;
    }());
    Directives.Email = Email;
})(Directives || (Directives = {}));
var Directives;
(function (Directives) {
    // directive to control input focus, source: http://www.emberex.com/programmatically-setting-focus-angularjs-way/
    var HasFocus = (function () {
        function HasFocus(timeout, rootScope) {
            this.restrict = 'A';
            this.rootScopeService = rootScope;
            this.scope = { focusValue: '=hasFocus' };
            this.timeoutService = timeout;
        }
        HasFocus.prototype.link = function (scope, element, attrs, ctrl) {
            scope.$watch("focusValue", function (currentValue, previousValue) {
                if (currentValue === true) {
                    element[0].focus();
                }
                else if (currentValue === false) {
                    element[0].blur();
                }
                // TODO: figure out how to "trigger" an intial setting to make ccurrent and previousValue differ
                //if (currentValue === true && !previousValue) {
                //    element[0].focus();
                //} else if (currentValue === false && previousValue) {
                //    element[0].blur();
                //}
            });
        };
        return HasFocus;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    HasFocus.$inject = ['$timeout', '$rootScope'];
    Directives.HasFocus = HasFocus;
})(Directives || (Directives = {}));
var Directives;
(function (Directives) {
    // Directive to match two inputs, e.g. item and item confirm, source: http://ngmodules.org/modules/angular-input-match
    var Match = (function () {
        function Match() {
            this.require = 'ngModel';
            this.restrict = 'A';
            this.scope = { match: '=' };
        }
        Match.prototype.link = function (scope, elem, attrs, ctrl) {
            scope.$watch(function () {
                return (ctrl.$pristine && angular.isUndefined(ctrl.$modelValue)) || scope.match === ctrl.$modelValue;
            }, function (currentValue) {
                ctrl.$setValidity('match', currentValue);
            });
        };
        return Match;
    }());
    Directives.Match = Match;
})(Directives || (Directives = {}));
var Directives;
(function (Directives) {
    // Directive to ensure a string quantity value is an integer, decimal or fractional format.
    var Quantity = (function () {
        function Quantity() {
            this.require = 'ngModel';
            this.restrict = 'A';
        }
        Quantity.prototype.link = function (scope, elem, attrs, ngModel) {
            // validity function
            var quantityIsValid = function (value) {
                // return false if we don't have some value
                if (!value || value.length == 0) {
                    return false;
                }
                // use an any type to make use of isNaN
                var testValue = value.trim();
                // If the string is a number return true
                if (!isNaN(testValue)) {
                    return true;
                }
                // If the string has more than two space deliminted elements return false
                var parts = testValue.split(' ');
                if (parts.length > 2) {
                    return false;
                }
                // string should be a number or a number plus fractional piece
                var valid = true;
                for (var i = 0; i < parts.length; i++) {
                    var fractionParts = parts[i].split('/');
                    if (fractionParts.length == 1) {
                        if (i > 0) {
                            valid = false;
                            break;
                        }
                        else {
                            valid = valid && !isNaN(fractionParts[0]);
                        }
                    }
                    else if (fractionParts.length == 2) {
                        valid = valid && fractionParts[0].trim().length > 0 && !isNaN(fractionParts[0]) && !isNaN(fractionParts[1]) && fractionParts[1] != 0;
                    }
                    else {
                        valid = false;
                        break;
                    }
                }
                return valid;
            };
            // dom to model validation
            ngModel.$parsers.unshift(function (value) {
                var valid = quantityIsValid(value);
                ngModel.$setValidity('quantity', valid);
                return valid ? value : undefined;
            });
            // model to dom validation
            ngModel.$formatters.unshift(function (value) {
                var valid = quantityIsValid(value);
                ngModel.$setValidity('quantity', valid);
                return value;
            });
        };
        return Quantity;
    }());
    Directives.Quantity = Quantity;
})(Directives || (Directives = {}));
angular.module('AppFilters', [])
    .filter('BootstrapDangerSaveFilter', function () {
    return function (entityIsDelete) {
        return entityIsDelete ? 'danger' : 'success';
    };
})
    .filter('BootstrapErrorFilter', function () {
    return function (isError) {
        return isError ? 'has-error' : '';
    };
})
    .filter('BootstrapSelectButtonItemFilter', function () {
    return function (isSelected) {
        return isSelected ? 'btn btn-sm btn-primary' : 'btn btn-sm btn-default';
    };
})
    .filter('CollapsedGlyphFilter', function () {
    return function (isCollapsed) {
        return isCollapsed ? 'glyphicon glyphicon-chevron-down' : 'glyphicon glyphicon-chevron-up';
    };
})
    .filter('CollapsedPanelFilter', function () {
    return function (isCollapsed) {
        return isCollapsed ? 'panel-collapse collapse' : 'panel-collapse collapse in';
    };
})
    .filter('DeleteOrCancelGlyphFilter', function () {
    return function (isDelete) {
        return isDelete ? 'glyphicon glyphicon-refresh' : 'glyphicon glyphicon-remove';
    };
})
    .filter('EntityStateToBootstrapFilter', function () {
    return function (entity) {
        if (entity.isDelete) {
            return 'danger';
        }
        else if (!entity.isValid) {
            return 'warning';
        }
        else if (entity.isNew) {
            return 'success';
        }
        else if (entity.isModify) {
            return 'info';
        }
        else {
            return 'default';
        }
    };
})
    .filter('RecipesTitleFilter', function () {
    return function (value) {
        if (value == null || !(value.length > 0)) {
            return 'Search to find recipes';
        }
        else if (value.length === 1) {
            return '1 Recipe';
        }
        else {
            return value.length + ' Recipes';
        }
    };
})
    .filter('RequiredOrErrorFilter', function () {
    return function (value) {
        return (!value || value.trim().length < 1) ? 'has-error' : '';
    };
})
    .filter('ServingsPluralizationFilter', function () {
    return function (value) {
        return (value === 1) ? 'serving' : 'servings';
    };
})
    .filter('TagsReadableListFilter', function () {
    return function (tags) {
        var listString = '';
        if (tags && tags.length > 0) {
            for (var i = 0; i < tags.length - 1; i++) {
                listString += tags[i].Description + ', ';
            }
            listString += tags[tags.length - 1].Description;
        }
        return listString;
    };
});
var Controllers;
(function (Controllers) {
    // Controller for the application's navbar.
    var NavController = (function () {
        function NavController(scope, location, coreService) {
            scope.navController = this;
            this.coreService = coreService;
            this.locationService = location;
            this.navAreas = [
                { iconClass: 'glyphicon glyphicon-list-alt', name: 'recipes', requireAuth: true, toolTip: 'Your recipes.' },
                { iconClass: 'glyphicon glyphicon-tag', name: 'tags', requireAuth: true, toolTip: 'Your recipe tags.' },
                { iconClass: 'glyphicon glyphicon-question-sign', name: 'about', requireAuth: false, toolTip: 'About RecipeBox.' },
                { iconClass: 'glyphicon glyphicon-phone', name: 'contact', requireAuth: false, toolTip: '' }
            ];
        }
        NavController.prototype.areaIsActive = function (name) {
            if (!name) {
                return false;
            }
            var path = '/' + name;
            return this.locationService.path().substr(0, path.length) === path;
        };
        return NavController;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    NavController.$inject = ['$scope', '$location', 'coreService'];
    Controllers.NavController = NavController;
})(Controllers || (Controllers = {}));
var Controllers;
(function (Controllers) {
    // Controller for the account registraiton and log in area.
    var AccountController = (function () {
        function AccountController(scope, locationService, coreService, accountService) {
            scope.accountController = this;
            this.coreService = coreService;
            this.accountService = accountService;
            this.isBusy = false;
            this.isRegistering = false;
            this.locationService = locationService;
            this.loginPassword = '';
            this.loginUserName = '';
            this.remember = this.coreService.credentialsAreSaved();
            this.serverErrorMessage = '';
            // if authenticated fetch account for maintenance otherwise log in
            if (this.coreService.isAuthenticated()) {
                this.title = 'Account Details';
                this.isBusy = true;
                this.getAccount();
            }
            else {
                this.setDefaultAccount();
                this.title = 'Credentials';
            }
        }
        // method for processing account change events
        AccountController.prototype.accountChange = function (controller) {
            this.coreService.entityChange(this.account, controller);
        };
        // cancel account maintenance or registration
        AccountController.prototype.cancel = function () {
            if (this.isRegistering) {
                this.isRegistering = false;
                this.serverErrorMessage = '';
                this.title = 'Credentials';
            }
            else {
                this.getAccount();
            }
        };
        // get the user's account
        AccountController.prototype.getAccount = function () {
            var _this = this;
            this.accountService.getAccount(this.coreService.authInfo.userName)
                .success(function (data, status, headers, config) {
                _this.coreService.initializeEntity(data);
                _this.account = data;
                _this.account.PasswordConfirm = _this.account.Password;
            })
                .error(function (data, status, headers, config) {
                _this.setDefaultAccount();
            })
                .finally(function () {
                _this.serverErrorMessage = '';
                _this.isBusy = false;
            });
        };
        // log the user in
        AccountController.prototype.logIn = function () {
            var _this = this;
            this.isBusy = true;
            this.accountService.authenticateAccount(this.loginUserName, this.loginPassword)
                .success(function (data, status, headers, config) {
                // determine the expiration date
                var expirationDate = new Date();
                expirationDate.setSeconds(expirationDate.getSeconds() + data.expires_in);
                // setup session auth
                var authInfo = {
                    accessToken: data.access_token,
                    expirationDate: expirationDate,
                    tokenType: data.token_type,
                    userName: _this.loginUserName
                };
                _this.coreService.setAuthenticated(authInfo);
                // save credentials locally if requested
                if (_this.remember) {
                    _this.coreService.saveCurrentCredentials();
                }
                else {
                    _this.coreService.deleteLocalCredentials();
                }
                // reset authentication attempts to 0
                //this.accountService.authenticationAttempts = 0;
                // go home
                _this.locationService.path('/');
            })
                .error(function (data, status, headers, config) {
                _this.isRegistering = false;
                _this.coreService.setAuthenticated(null);
                _this.loginPassword = '';
            })
                .finally(function () {
                _this.isBusy = false;
            });
        };
        // initialize for registration
        AccountController.prototype.register = function () {
            this.isRegistering = true;
            this.setDefaultAccount();
            this.title = 'Create an Account';
        };
        // mark the account as deleted
        AccountController.prototype.removeAccount = function () {
            this.account.isDelete = true;
        };
        // save changes to the current account
        AccountController.prototype.saveAccount = function (formController) {
            var _this = this;
            this.isBusy = true;
            // delete the account and log out if delete is marked
            if (this.account.isDelete) {
                this.accountService.deleteAccount(this.account.Id)
                    .success(function (data, status, header, config) {
                })
                    .error(function (data, status, header, config) {
                })
                    .finally(function () {
                    _this.setDefaultAccount();
                    _this.isBusy;
                    _this.coreService.logOut();
                });
            }
            else {
                var promise = this.account.isNew ? this.accountService.postAccount(this.account) : this.accountService.putAccount(this.account);
                promise
                    .success(function (data, status, header, config) {
                    // account.Password is unencrypted if this is a registration otherwise it's encrypted
                    _this.loginPassword = _this.account.Password;
                    _this.account.AuthProvider = data.AuthProvider;
                    _this.account.EmailAddress = data.EmailAddress;
                    _this.account.Id = data.Id;
                    _this.account.isBusy = false;
                    _this.account.isDelete = false;
                    _this.account.isEditing = false;
                    _this.account.isModify = false;
                    _this.account.isNew = false;
                    _this.account.isSelected = false;
                    _this.account.isValid = true;
                    _this.account.Password = data.Password;
                    _this.account.PasswordConfirm = data.Password;
                    _this.account.RowVersion = data.RowVersion;
                    _this.account.UserName = data.UserName;
                    // handle post results if authenticatied or if registering or otherwise.
                    if (_this.coreService.isAuthenticated()) {
                        _this.isBusy = false;
                        _this.locationService.path('/');
                        _this.coreService.authInfo.userName = _this.account.UserName;
                        _this.coreService.setAuthenticated(_this.coreService.authInfo);
                        if (_this.coreService.credentialsAreSaved()) {
                            _this.coreService.saveCurrentCredentials();
                        }
                    }
                    else if (_this.isRegistering) {
                        _this.locationService.path('/');
                    }
                    else {
                        _this.loginUserName = _this.account.UserName;
                        _this.isBusy = false;
                        _this.logIn();
                    }
                })
                    .error(function (data, status, header, config) {
                    // data is not IAccountDto for error, it's an object with a Message property.
                    var response = data;
                    if (response && response.Message) {
                        _this.serverErrorMessage = response.Message;
                    }
                    else {
                        _this.serverErrorMessage = 'Unknown error encountered saving account.';
                    }
                    _this.account.isValid = false;
                    _this.isBusy = false;
                })
                    .finally(function () {
                });
            }
        };
        // initialize account with an new empty default
        AccountController.prototype.setDefaultAccount = function () {
            this.account = {
                AuthProvider: 'RecipeBox',
                EmailAddress: '',
                keyName: '',
                Id: '',
                isBusy: false,
                isDelete: false,
                isEditing: true,
                isModify: false,
                isNew: true,
                isSelected: false,
                isValid: false,
                Password: '',
                PasswordConfirm: '',
                RowVersion: [],
                UserName: ''
            };
            this.loginPassword = '';
            this.loginUserName = '';
            this.serverErrorMessage = '';
        };
        // the account view is log in (true) or registration/maintenance (false)
        AccountController.prototype.showLogIn = function () {
            return !this.coreService.isAuthenticated() && !this.isRegistering;
        };
        return AccountController;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    AccountController.$inject = ['$scope', '$location', 'coreService', 'accountService'];
    Controllers.AccountController = AccountController;
})(Controllers || (Controllers = {}));
var Controllers;
(function (Controllers) {
    var TagsController = (function () {
        function TagsController(scope, coreService, tagsService) {
            scope.tagsController = this;
            this.coreService = coreService;
            this.description = 'Tags are attached to recipes aiding in categorization and searching. Here you can view, create, edit and delete your personal tags.';
            this.emptyDescriptionText = '{Description}';
            this.isBusy = false;
            this.newTagId = 0;
            this.tags = [];
            this.tagsService = tagsService;
            this.title = 'Recipe Tags';
            this.getTags();
        }
        // create a new tag and add it to the tags array
        TagsController.prototype.addNewTag = function () {
            this.newTagId++;
            this.tags.unshift({
                Description: '',
                Id: -1 * this.newTagId,
                isBusy: false,
                isDelete: false,
                isEditing: false,
                isModify: true,
                isNew: true,
                isSelected: false,
                isValid: false,
                keyName: 'newtag' + this.newTagId,
                RowVersion: null,
                AccountId: '1234567890'
            });
        };
        // apply the 'delete' action to the input tag
        TagsController.prototype.applyDelete = function (tag) {
            tag.isDelete = true;
            if (tag.isNew) {
                this.removeTag(tag.Id);
            }
        };
        // apply the 'refresh' action to the input tag
        TagsController.prototype.applyRefresh = function (tag, formController) {
            if (tag.isNew) {
                this.removeTag(tag.Id);
            }
            else {
                this.refreshTag(tag, formController);
            }
        };
        // apply the 'save' action to the input tag
        TagsController.prototype.applySave = function (tag, formController) {
            var _this = this;
            if (formController.$invalid) {
                return;
            }
            tag.isBusy = true;
            if (tag.isDelete) {
                this.tagsService.deleteTag(tag.Id)
                    .success(function (data, status, header, config) {
                    _this.removeTag(tag.Id);
                })
                    .error(function (data, status, header, config) {
                })
                    .finally(function () {
                    tag.isBusy = false;
                });
            }
            else if (tag.isNew || formController.$dirty) {
                var promise = tag.isNew ? this.tagsService.postTag(tag) : this.tagsService.putTag(tag);
                promise
                    .success(function (tagDto, status, header, config) {
                    _this.coreService.initializeEntity(tag);
                    tag.Description = tagDto.Description;
                    tag.Id = tagDto.Id;
                    tag.keyName = 'tag' + tagDto.Id;
                    tag.RowVersion = tagDto.RowVersion;
                    tag.AccountId = tagDto.AccountId;
                    formController.$setPristine();
                })
                    .error(function (data, status, header, config) {
                })
                    .finally(function () {
                    tag.isBusy = false;
                });
            }
            else {
                tag.isBusy = false;
            }
        };
        // call getTags on tagsService and assign controller tags
        TagsController.prototype.getTags = function () {
            var _this = this;
            this.isBusy = true;
            this.tagsService.getTags()
                .success(function (data, status, headers, config) {
                for (var i = 0; i < data.length; i++) {
                    _this.coreService.initializeEntity(data[i]);
                    data[i].isBusy = false;
                    data[i].keyName = 'tag' + data[i].Id;
                }
                _this.tags = data;
            })
                .error(function (data, status, headers, config) {
                _this.tags = [];
            })
                .finally(function () {
                _this.isBusy = false;
            });
        };
        // refresh the input tag
        TagsController.prototype.refreshTag = function (tag, formController) {
            var _this = this;
            tag.isBusy = true;
            this.tagsService.getTag(tag.Id)
                .success(function (data, status, headers, config) {
                var isEditing = tag.isEditing;
                _this.coreService.initializeEntity(tag);
                tag.Description = data.Description;
                tag.isEditing = isEditing;
                tag.keyName = 'tag' + tag.Id;
                formController.$setPristine();
            })
                .error(function (data, status, headers, config) {
            })
                .finally(function () {
                tag.isBusy = false;
            });
        };
        // remove the tag with the input Id value from the array
        TagsController.prototype.removeTag = function (id) {
            for (var i = 0; i < this.tags.length; i++) {
                if (this.tags[i].Id === id) {
                    this.tags.splice(i, 1);
                    return;
                }
            }
        };
        // toggle the isEditing property on the input tag
        TagsController.prototype.toggleTagEditing = function (tag) {
            tag.isEditing = !tag.isEditing;
            // TODO: get rid of dom reference with angular behavior
            $('#' + tag.keyName).collapse('toggle');
        };
        return TagsController;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    TagsController.$inject = ['$scope', 'coreService', 'tagsService'];
    Controllers.TagsController = TagsController;
})(Controllers || (Controllers = {}));
var Controllers;
(function (Controllers) {
    var RecipesController = (function () {
        function RecipesController(scope, coreService, recipesService, tagsService) {
            var _this = this;
            scope.recipesController = this;
            this.coreService = coreService;
            this.isBusy = false;
            this.recipesService = recipesService;
            this.searchFilter = "";
            this.tags = [];
            this.tagsService = tagsService;
            this.title = 'Recipes';
            this.getTags();
            // callback function needs to be defined in the constructor to preserve lexical scoping when called
            this.refreshRecipesCallback = function () {
                _this.isBusy = false;
            };
        }
        // update recipesService.recipes with current search criteria
        RecipesController.prototype.getRecipes = function () {
            var _this = this;
            this.isBusy = true;
            this.recipesService.getRecipesAsync(this.searchFilter, this.selectedTag)
                .success(function (data, status, headers, config) {
                var recipes;
                for (var i = 0; i < data.length; i++) {
                    _this.coreService.initializeEntity(data[i]);
                    data[i].isBusy = false;
                    data[i].keyName = 'recipe' + data[i].Id;
                    data[i].GraphEntity = _this.coreService.newEntity();
                    _this.coreService.copyEntity(data[i], data[i].GraphEntity);
                }
                _this.recipesService.recipes = data;
            })
                .error(function (data, status, headers, config) {
            })
                .finally(function () {
                _this.isBusy = false;
            });
        };
        // call getTags on tagsService and handle results
        RecipesController.prototype.getTags = function () {
            var _this = this;
            this.isBusy = true;
            this.tags = [];
            this.tagsService.getTags()
                .success(function (data, status, headers, config) {
                _this.tags = data;
            })
                .error(function (data, status, headers, config) {
                _this.tags = [];
            })
                .finally(function () {
                var tag = {
                    Description: "All tags",
                    Id: -1,
                    RowVersion: [],
                    AccountId: ''
                };
                _this.tags.unshift(tag);
                _this.selectedTag = _this.tags[0];
                _this.isBusy = false;
            });
        };
        return RecipesController;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    RecipesController.$inject = ['$scope', 'coreService', 'recipesService', 'tagsService'];
    Controllers.RecipesController = RecipesController;
})(Controllers || (Controllers = {}));
var Controllers;
(function (Controllers) {
    var RecipeEditController = (function () {
        function RecipeEditController(scope, location, routeParams, coreService, recipesService, tagsService, uploadService) {
            var _this = this;
            scope.recipeEditController = this;
            if (routeParams.id === undefined || isNaN(routeParams.id)) {
                location.path('/recipes');
            }
            this.activeCalls = 0;
            this.coreService = coreService;
            this.emptyIngredientDescriptionText = 'Description, e.g. flour';
            this.emptyIngredientQuantityText = 'Quantity, e.g. 1 2/3';
            this.emptyIngredientUnitsText = 'Units, e.g. cups';
            this.imageFile = null;
            this.isBusy = false;
            this.locationService = location;
            this.recipesService = recipesService;
            this.recipe = this.recipesService.newRecipe();
            this.tags = [];
            this.tagsService = tagsService;
            this.title = 'Loading Recipe';
            this.uploadService = uploadService;
            // callback function needs to be defined in the constructor to preserve lexical scoping when called
            this.getRecipeCallback = function (recipe) {
                if (recipe) {
                    _this.imageFile = null;
                    _this.title = 'Edit Recipe';
                    _this.recipe = recipe;
                    _this.recipe.GraphEntity = _this.coreService.newEntity();
                    _this.coreService.copyEntity(_this.recipe, _this.recipe.GraphEntity);
                    _this.updateTagsSelected();
                    _this.recipeGraphChange();
                    _this.endCall();
                }
                else {
                    _this.locationService.path('/recipes');
                }
            };
            this.getTags();
            if (routeParams.id > 0) {
                this.getRecipe(routeParams.id);
            }
            else {
                this.title = 'New Recipe';
            }
        }
        // method to increment the active calls count and set busy state
        RecipeEditController.prototype.beginCall = function () {
            this.activeCalls++;
            this.isBusy = true;
        };
        // cancel the edit
        RecipeEditController.prototype.cancel = function () {
            if (this.recipe.isNew) {
                this.locationService.path('/recipes');
            }
            else {
                this.beginCall();
                this.recipesService.getRecipe(this.recipe.Id, this.getRecipeCallback, true);
            }
        };
        // is the cancel action enabled?
        RecipeEditController.prototype.canCancel = function () {
            return !this.isBusy && !this.recipe.isBusy && (this.recipe.GraphEntity.isModify || this.recipe.GraphEntity.isDelete);
        };
        // remove the image
        RecipeEditController.prototype.deleteImage = function (controller) {
            this.recipe.ImageFileName = '';
            this.recipe.isModify = true;
            this.recipe.GraphEntity.isModify = true;
        };
        // mark the current recipe as delete
        RecipeEditController.prototype.deleteRecipe = function () {
            this.recipe.isDelete = true;
            this.recipe.GraphEntity.isDelete = true;
        };
        // method to decrement the active calls count and set busy state appropriately
        RecipeEditController.prototype.endCall = function () {
            this.activeCalls--;
            this.isBusy = this.activeCalls > 0;
        };
        // use the recipesService to get the current recipe
        RecipeEditController.prototype.getRecipe = function (id) {
            this.beginCall();
            this.recipesService.getRecipe(id, this.getRecipeCallback);
        };
        // get the current available tags
        RecipeEditController.prototype.getTags = function () {
            var _this = this;
            this.beginCall();
            this.tagsService.getTags()
                .success(function (data, status, headers, config) {
                for (var i = 0; i < data.length; i++) {
                    _this.coreService.initializeEntity(data[i]);
                    data[i].keyName = 'tag' + data[i].Id;
                }
                _this.tags = data;
            })
                .error(function (data, status, headers, config) {
                _this.tags = [];
            })
                .finally(function () {
                _this.updateTagsSelected();
                _this.endCall();
            });
        };
        // method to process recipe's image file change
        RecipeEditController.prototype.imageFileChanged = function (files) {
            if (!files || files.length == 0) {
                this.imageFile = null;
                this.imageFileName = '';
            }
            else {
                this.imageFile = files[0];
                this.imageFileName = this.imageFile.name;
            }
            this.recipe.ImageFileName = this.imageFileName;
            this.recipe.isModify = true;
            this.recipeGraphChange();
        };
        // method for processing ingredient change events
        RecipeEditController.prototype.ingredientChange = function (ingredient, controller) {
            this.coreService.entityChange(ingredient, controller);
            this.recipeGraphChange();
        };
        // get the tooltip for the ingredient's delete/cancel button
        RecipeEditController.prototype.ingredientDeleteTooltip = function (ingredient) {
            return ingredient.isDelete ? 'Unmark ingredient as deleted.' : 'Mark ingredient as deleted.';
        };
        // add a new ingredient
        RecipeEditController.prototype.newIngredient = function () {
            this.recipe.Ingredients.unshift({
                Description: '',
                Id: this.coreService.uniqueId--,
                isBusy: false,
                isDelete: false,
                isEditing: true,
                isModify: true,
                isNew: true,
                isSelected: false,
                isValid: false,
                keyName: 'newIngredient' + Math.abs(this.coreService.uniqueId),
                Quantity: '',
                RecipeId: this.recipe.Id,
                RowVersion: [],
                Units: ''
            });
            this.recipeGraphChange();
        };
        // method for processing recipe change events
        RecipeEditController.prototype.recipeChange = function (controller) {
            this.coreService.entityChange(this.recipe, controller);
            this.recipeGraphChange();
        };
        // called after change to both recipe and ingredients to update the recipe object graph properties
        RecipeEditController.prototype.recipeGraphChange = function () {
            // Set graph isModify
            var modify = this.recipe.isModify || this.recipe.GraphEntity.isModify;
            if (!modify) {
                for (i = 0; i < this.recipe.Ingredients.length; i++) {
                    if (this.recipe.Ingredients[i].isModify) {
                        modify = true;
                        break;
                    }
                }
            }
            if (this.recipe.GraphEntity.isModify != modify) {
                this.recipe.GraphEntity.isModify = modify;
            }
            // set isValid to false if recipe or any ingredident is invalid
            var valid = this.recipe.isValid;
            if (valid) {
                for (var i = 0; i < this.recipe.Ingredients.length; i++) {
                    if (!this.recipe.Ingredients[i].isValid) {
                        valid = false;
                        break;
                    }
                }
            }
            if (this.recipe.GraphEntity.isValid != valid) {
                this.recipe.GraphEntity.isValid = valid;
            }
        };
        // save changes to the current recipe
        RecipeEditController.prototype.saveRecipe = function (formController) {
            // solves scoping issue with "this" and promises
            var self = this;
            self.beginCall();
            if (self.recipe.isDelete) {
                self.recipesService.deleteRecipe(self.recipe.Id)
                    .success(function (data, status, header, config) {
                })
                    .error(function (data, status, header, config) {
                })
                    .finally(function () {
                    self.endCall();
                    self.locationService.path('/recipes');
                });
            }
            else if (self.recipe.isNew || formController.$dirty || self.recipe.GraphEntity.isModify) {
                // remove children children marked for delete
                for (var i = self.recipe.Ingredients.length - 1; i >= 0; i--) {
                    if (self.recipe.Ingredients[i].isDelete) {
                        self.recipe.Ingredients.splice(i, 1);
                    }
                }
                // save with image file
                if (self.imageFile !== null) {
                    var action = self.recipe.isNew ? 'post' : 'put';
                    var url = self.recipesService.urlBase + '/' + action + 'withimage';
                    self.uploadService.upload({
                        url: url,
                        method: action,
                        // headers: {'header-key': 'header-value'},
                        // withCredentials: true,
                        data: { recipe: self.recipesService.createDtoFromRecipe(self.recipe) },
                        file: self.imageFile,
                    })
                        .success(function (recipeDto, status, headers, config) {
                        self.updateRecipeAfterSave(recipeDto, formController);
                    })
                        .error(function (data, status, header, config) {
                        self.locationService.path('/recipes');
                    })
                        .then(function (success, error, progress) {
                        self.endCall();
                    });
                }
                else {
                    var promise = self.recipe.isNew ? self.recipesService.postRecipeAsync(self.recipe) : self.recipesService.putRecipeAsync(self.recipe);
                    promise
                        .success(function (recipeDto, status, header, config) {
                        self.updateRecipeAfterSave(recipeDto, formController);
                    })
                        .error(function (data, status, header, config) {
                        self.locationService.path('/recipes');
                    })
                        .finally(function () {
                        self.endCall();
                    });
                }
            }
            else {
                self.endCall();
            }
        };
        // toggle isDelete on the input ingredient
        RecipeEditController.prototype.toggleIngredientDelete = function (ingredient) {
            ingredient.isDelete = !ingredient.isDelete;
            // if the ingredient is delete and new just remove it
            if (ingredient.isDelete && ingredient.isNew) {
                for (var i = 0; i < this.recipe.Ingredients.length; i++) {
                    if (this.recipe.Ingredients[i].Id == ingredient.Id) {
                        this.recipe.Ingredients.splice(i, 1);
                        break;
                    }
                }
            }
            if (!this.recipe.GraphEntity.isModify) {
                this.recipe.GraphEntity.isModify = true;
            }
            this.recipeGraphChange();
        };
        // toggle selected on the input tag
        RecipeEditController.prototype.toggleTagSelected = function (id, form) {
            form.$dirty = true;
            this.recipe.isModify = true;
            this.recipe.GraphEntity.isModify = true;
            var tag = null;
            // toggle the tag's isSelected property
            for (var i = 0; i < this.tags.length; i++) {
                if (this.tags[i].Id == id) {
                    tag = this.tags[i];
                    this.tags[i].isSelected = !this.tags[i].isSelected;
                    break;
                }
            }
            // remove the tag from the recipe if it exists there
            for (var i = 0; i < this.recipe.Tags.length; i++) {
                if (this.recipe.Tags[i].Id == id) {
                    this.recipe.Tags.splice(i, 1);
                    return;
                }
            }
            // otherwise add it to the recipe
            this.recipe.Tags.push(tag);
        };
        RecipeEditController.prototype.updateRecipeAfterSave = function (recipeDto, formController) {
            if (formController === void 0) { formController = null; }
            var newRecipe = this.recipe.isNew;
            this.coreService.initializeEntity(this.recipe);
            this.recipe.Description = recipeDto.Description;
            this.recipe.Directions = recipeDto.Directions;
            this.recipe.Id = recipeDto.Id;
            this.imageFile = null;
            this.recipe.ImageFileName = recipeDto.ImageFileName;
            this.recipe.Ingredients = recipeDto.Ingredients;
            this.recipe.keyName = 'recipe' + recipeDto.Id;
            this.recipe.Name = recipeDto.Name;
            this.recipe.Notes = recipeDto.Notes;
            this.recipe.PreparationMinutes = recipeDto.PreparationMinutes;
            this.recipe.RowVersion = recipeDto.RowVersion;
            this.recipe.Servings = recipeDto.Servings;
            this.recipe.Tags = recipeDto.Tags;
            this.recipe.AccountId = recipeDto.AccountId;
            for (var i = 0; i < this.recipe.Ingredients.length; i++) {
                this.coreService.initializeEntity(this.recipe.Ingredients[i]);
            }
            this.coreService.copyEntity(this.recipe, this.recipe.GraphEntity);
            if (newRecipe) {
                this.recipesService.recipes.unshift(this.recipe);
                this.recipesService.recipes.sort(function (recipeA, recipeB) {
                    var a = recipeA.Name.toLowerCase();
                    var b = recipeB.Name.toLowerCase();
                    if (a > b) {
                        return 1;
                    }
                    else if (a < b) {
                        return -1;
                    }
                    else {
                        return 0;
                    }
                });
            }
            formController.$setPristine();
        };
        // once tags, recipe and recipe's tags are available update tag's isSelected
        RecipeEditController.prototype.updateTagsSelected = function () {
            if (this.recipe && this.recipe.Tags && this.recipe.Tags.length > 0 && this.tags && this.tags.length > 0) {
                for (var i = 0; i < this.tags.length; i++) {
                    this.tags[i].isSelected = false;
                    for (var j = 0; j < this.recipe.Tags.length; j++) {
                        if (this.recipe.Tags[j].Id == this.tags[i].Id) {
                            this.tags[i].isSelected = true;
                        }
                    }
                }
            }
        };
        return RecipeEditController;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    RecipeEditController.$inject = ['$scope', '$location', '$routeParams', 'coreService', 'recipesService', 'tagsService', '$upload'];
    Controllers.RecipeEditController = RecipeEditController;
})(Controllers || (Controllers = {}));
var Controllers;
(function (Controllers) {
    var RecipeDisplayController = (function () {
        function RecipeDisplayController(scope, location, routeParams, coreService, recipesService) {
            scope.recipeDisplayController = this;
            if (routeParams.id === undefined || isNaN(routeParams.id)) {
                location.path('/recipes');
            }
            this.coreService = coreService;
            this.locationService = location;
            this.recipesService = recipesService;
            this.recipe = this.recipesService.newRecipe();
            this.getRecipe(routeParams.id);
        }
        // use the recipesService to get the current recipe
        RecipeDisplayController.prototype.getRecipe = function (id) {
            var _this = this;
            this.isBusy = true;
            this.recipesService.getRecipeAsync(id)
                .success(function (data, status, headers, config) {
                if (data !== null) {
                    _this.coreService.initializeEntity(data);
                    data.isBusy = false;
                    data.keyName = 'recipe' + data.Id;
                    for (var i = 0; i < data.Ingredients.length; i++) {
                        _this.coreService.initializeEntity(data.Ingredients[i]);
                        data.Ingredients[i].isBusy = false;
                        data.Ingredients[i].keyName = 'ingredient' + data.Ingredients[i].Id;
                    }
                    _this.recipe = data;
                }
                else {
                    _this.locationService.path('/recipes');
                }
            })
                .error(function (data, status, headers, config) {
                _this.locationService.path('/recipes');
            })
                .finally(function () {
                _this.isBusy = false;
            });
        };
        return RecipeDisplayController;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    RecipeDisplayController.$inject = ['$scope', '$location', '$routeParams', 'coreService', 'recipesService'];
    Controllers.RecipeDisplayController = RecipeDisplayController;
})(Controllers || (Controllers = {}));
var Controllers;
(function (Controllers) {
    // Controller for the application's About area.
    var AboutController = (function () {
        function AboutController(scope, coreService) {
            scope.aboutController = this;
            this.coreService = coreService;
        }
        return AboutController;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    AboutController.$inject = ['$scope', 'coreService'];
    Controllers.AboutController = AboutController;
})(Controllers || (Controllers = {}));
var Controllers;
(function (Controllers) {
    // Controller for the application's Home area.
    var HomeController = (function () {
        function HomeController(scope, coreService) {
            scope.homeController = this;
            this.coreService = coreService;
        }
        return HomeController;
    }());
    // $inject is a pattern to handle variable renaming during minification.
    HomeController.$inject = ['$scope', 'coreService'];
    Controllers.HomeController = HomeController;
})(Controllers || (Controllers = {}));
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
    .factory('authInterceptorService', ['$q', '$location', 'coreService', function ($q, $location, coreService) {
        var self = this;
        // add authorization header
        self.request = function (config) {
            if (coreService.isAuthenticated()) {
                config.headers.Authorization = 'Bearer ' + coreService.authInfo.accessToken;
            }
            return config;
        };
        // watch for failures due 410 unauthorized
        self.responseError = function (rejection) {
            if (rejection.status === 401) {
                $location.path('/account');
            }
            return $q.reject(rejection);
        };
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
//# sourceMappingURL=recipebox.js.map