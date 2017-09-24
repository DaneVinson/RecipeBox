module Controllers {
    export class RecipeEditController {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$scope', '$location', '$routeParams', 'coreService', 'recipesService', 'tagsService', '$upload'];
        constructor(scope: any, location: ng.ILocationService, routeParams: any, coreService: Services.CoreService, recipesService: Services.RecipesService, tagsService: Services.TagsService, uploadService: any) {
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
            this.getRecipeCallback = (recipe: IRecipe) => {
                if (recipe) {
                    this.imageFile = null;
                    this.title = 'Edit Recipe';
                    this.recipe = recipe;
                    this.recipe.GraphEntity = this.coreService.newEntity();
                    this.coreService.copyEntity(this.recipe, this.recipe.GraphEntity);
                    this.updateTagsSelected();
                    this.recipeGraphChange();
                    this.endCall();
                }
                else {
                    this.locationService.path('/recipes');
                }
            };

            this.getTags();
            if (routeParams.id > 0) {
                this.getRecipe(routeParams.id);
            }
            else {
                this.title = 'New Recipe'
            }
        }


        activeCalls: number;
        coreService: Services.CoreService;
        emptyIngredientDescriptionText: string;
        emptyIngredientQuantityText: string;
        emptyIngredientUnitsText: string;
        getRecipeCallback: (recipe: IRecipe) => void;
        imageFile: any;
        imageFileName: string;
        isBusy: boolean;
        locationService: ng.ILocationService;
        recipe: IRecipe;
        recipesService: Services.RecipesService;
        tags: ITag[];
        tagsService: Services.TagsService;
        title: string;
        uploadService: any;


        // method to increment the active calls count and set busy state
        beginCall() {
            this.activeCalls++;
            this.isBusy = true;
        }

        // cancel the edit
        cancel() {
            if (this.recipe.isNew) {
                this.locationService.path('/recipes');
            }
            else {
                this.beginCall();
                this.recipesService.getRecipe(this.recipe.Id, this.getRecipeCallback, true);
            }
        }

        // is the cancel action enabled?
        canCancel(): boolean {
            return !this.isBusy && !this.recipe.isBusy && (this.recipe.GraphEntity.isModify || this.recipe.GraphEntity.isDelete);
        }

        // remove the image
        deleteImage(controller: ng.IFormController) {
            this.recipe.ImageFileName = '';
            this.recipe.isModify = true;
            this.recipe.GraphEntity.isModify = true;
        }

        // mark the current recipe as delete
        deleteRecipe() {
            this.recipe.isDelete = true;
            this.recipe.GraphEntity.isDelete = true;
        }

        // method to decrement the active calls count and set busy state appropriately
        endCall() {
            this.activeCalls--;
            this.isBusy = this.activeCalls > 0;
        }

        // use the recipesService to get the current recipe
        getRecipe(id: number) {
            this.beginCall();
            this.recipesService.getRecipe(id, this.getRecipeCallback);
        }

        // get the current available tags
        getTags() {
            this.beginCall();
            this.tagsService.getTags()
                .success((data, status, headers, config) => {
                    for (var i: number = 0; i < data.length; i++) {
                        this.coreService.initializeEntity(data[i]);
                        data[i].keyName = 'tag' + data[i].Id;
                    }
                    this.tags = data;
                })
                .error((data, status, headers, config) => {
                    this.tags = [];
                })
                .finally(() => {
                    this.updateTagsSelected();
                    this.endCall();
                });
        }

        // method to process recipe's image file change
        imageFileChanged(files: any[]) {
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
        }

        // method for processing ingredient change events
        ingredientChange(ingredient: IIngredient, controller: ng.IFormController) {
            this.coreService.entityChange(ingredient, controller);
            this.recipeGraphChange();
        }

        // get the tooltip for the ingredient's delete/cancel button
        ingredientDeleteTooltip(ingredient: IIngredient): string {
            return ingredient.isDelete ? 'Unmark ingredient as deleted.' : 'Mark ingredient as deleted.';
        }

        // add a new ingredient
        newIngredient() {
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
        }

        // method for processing recipe change events
        recipeChange(controller: ng.IFormController) {
            this.coreService.entityChange(this.recipe, controller);
            this.recipeGraphChange();
        }

        // called after change to both recipe and ingredients to update the recipe object graph properties
        recipeGraphChange() {

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
        }

        // save changes to the current recipe
        saveRecipe(formController: ng.IFormController) {
            // solves scoping issue with "this" and promises
            var self = this;

            self.beginCall();
            if (self.recipe.isDelete) {
                self.recipesService.deleteRecipe(self.recipe.Id)
                    .success((data, status, header, config) => {
                    })
                    .error((data, status, header, config) => {
                    })
                    .finally(() => {
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
                    var action: string = self.recipe.isNew ? 'post' : 'put';
                    var url: string = self.recipesService.urlBase + '/' + action + 'withimage';
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
                        .error((data, status, header, config) => {
                            self.locationService.path('/recipes');
                        })
                        .then((success, error, progress) => {
                            self.endCall();
                        });
                }

                // save without image file
                else {
                    var promise: ng.IHttpPromise<IRecipeDto> = self.recipe.isNew ? self.recipesService.postRecipeAsync(self.recipe) : self.recipesService.putRecipeAsync(self.recipe);
                    promise
                        .success((recipeDto, status, header, config) => {
                            self.updateRecipeAfterSave(recipeDto, formController);
                        })
                        .error((data, status, header, config) => {
                            self.locationService.path('/recipes');
                        })
                        .finally(() => {
                            self.endCall();
                        });
                }
            }
            else {
                self.endCall();
            }
        }

        // toggle isDelete on the input ingredient
        toggleIngredientDelete(ingredient: IIngredient) {
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
        }

        // toggle selected on the input tag
        toggleTagSelected(id: number, form: ng.IFormController) {
            form.$dirty = true;
            this.recipe.isModify = true;
            this.recipe.GraphEntity.isModify = true;
            var tag: ITag = null;

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
        }

        updateRecipeAfterSave(recipeDto: IRecipeDto, formController: ng.IFormController = null) {
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
                this.recipesService.recipes.sort(function (recipeA: IRecipe, recipeB: IRecipe) {
                    var a = recipeA.Name.toLowerCase();
                    var b = recipeB.Name.toLowerCase();
                    if (a > b) {
                        return 1;
                    }
                    else if (a < b) {
                                    return -1
                                }
                    else {
                        return 0;
                    }
                });
            }
            formController.$setPristine();
        }

        // once tags, recipe and recipe's tags are available update tag's isSelected
        updateTagsSelected() {
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
        }
    }
}
