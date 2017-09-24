module Services {

    // Angular service class for interfacing with the recipes REST service.
    export class RecipesService {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$http', 'coreService'];
        constructor(http: ng.IHttpService, coreService: Services.CoreService) {
            this.coreService = coreService;
            this.httpService = http;
            this.recipes = [];
        }


        coreService: Services.CoreService;
        httpService: ng.IHttpService;
        recipes: IRecipe[];
        urlBase: string = './api/recipes';

        // create a new IRecipeDto object from an input recipe
        createDtoFromRecipe(recipe: IRecipeDto): IRecipeDto {
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
            }
        }

        // delete a recipe by id
        deleteRecipe(id: number): ng.IHttpPromise<any> {
            for (var i = 0; i < this.recipes.length; i++) {
                if (this.recipes[i].Id == id) {
                    this.recipes.splice(i, 1);
                }
            }
            return this.httpService.delete(this.urlBase + '/' + id);
        }

        getRecipe(id: number, callback: (recipe: IRecipe) => void = null, forceRefresh: boolean = false): void {
            var cacheIndex: number = -1;
            var recipe: IRecipe = null;

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
                            recipe = data;
                            if (cacheIndex < 0) {
                                this.recipes.push(data);
                            }
                            else {
                                this.recipes[cacheIndex] = data;
                            }
                        }
                    })
                    .error((data, status, headers, config) => {
                    })
                    .finally(() => {
                        callback(recipe);
                        return recipe;
                    });
            }

            // otherwise the recipe is ready to go now
            else {
                callback(this.recipes[cacheIndex]);
            }
        }

        // get a recipe by id
        getRecipeAsync(id: number): ng.IHttpPromise<IRecipe> {
            return this.httpService.get(this.urlBase + '/' + id);
        }

        getRecipesAsync(nameFilter: string, tag: ITagDto): ng.IHttpPromise<IRecipe[]> {
            return this.httpService.get(this.urlBase + '/filtered', { params: { NameFilter: nameFilter, TagId: tag.Id } });
        }

        // create a new recipe
        newRecipe(): IRecipe {
            var recipe: IRecipe = {
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
        }

        // post a new recipe
        postRecipeAsync(recipe: IRecipeDto): ng.IHttpPromise<IRecipeDto> {
            return this.httpService.post(this.urlBase, recipe);
        }

        // put to update a recipe
        putRecipeAsync(recipe: IRecipeDto): ng.IHttpPromise<IRecipeDto> {
            return this.httpService.put(this.urlBase, recipe);
        }
    }
}
