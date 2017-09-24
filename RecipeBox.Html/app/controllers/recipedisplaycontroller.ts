module Controllers {
    export class RecipeDisplayController {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$scope', '$location', '$routeParams', 'coreService', 'recipesService'];
        constructor(scope: any, location: ng.ILocationService, routeParams: any, coreService: Services.CoreService, recipesService: Services.RecipesService) {
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


        coreService: Services.CoreService;
        getRecipeCallback: (recipe: IRecipe) => void;
        isBusy: boolean;
        locationService: ng.ILocationService;
        recipe: IRecipe;
        recipesService: Services.RecipesService;


        // use the recipesService to get the current recipe
        getRecipe(id: number) {
            this.isBusy = true;
            this.recipesService.getRecipeAsync(id)
                .success((data, status, headers, config) => {
                    if (data !== null) {
                        this.coreService.initializeEntity(data);
                        data.isBusy = false;
                        data.keyName = 'recipe' + data.Id;
                        for (var i = 0; i < data.Ingredients.length; i++) {
                            this.coreService.initializeEntity(data.Ingredients[i]);
                            data.Ingredients[i].isBusy = false;
                            data.Ingredients[i].keyName = 'ingredient' + data.Ingredients[i].Id;
                        }
                        this.recipe = data;
                    }
                    else {
                        this.locationService.path('/recipes');
                    }
                })
                .error((data, status, headers, config) => {
                    this.locationService.path('/recipes');
                })
                .finally(() => {
                    this.isBusy = false;
                });

        }
    }
}
