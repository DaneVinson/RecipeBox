module Controllers {
    export class RecipesController {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$scope', 'coreService', 'recipesService', 'tagsService'];
        constructor(scope: any, coreService: Services.CoreService, recipesService: Services.RecipesService, tagsService: Services.TagsService) {
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
            this.refreshRecipesCallback = () => {
                this.isBusy = false;
            };
        }


        coreService: Services.CoreService;
        isBusy: boolean;
        recipesService: Services.RecipesService;
        refreshRecipesCallback: () => void;
        searchFilter: string;
        selectedTag: ITagDto;
        tags: ITagDto[];
        tagsService: Services.TagsService;
        title: string;


        // update recipesService.recipes with current search criteria
        getRecipes(): void {
            this.isBusy = true;
            this.recipesService.getRecipesAsync(this.searchFilter, this.selectedTag)
                .success((data, status, headers, config) => {
                    var recipes: IRecipe[];
                    for (var i: number = 0; i < data.length; i++) {
                        this.coreService.initializeEntity(data[i]);
                        data[i].isBusy = false;
                        data[i].keyName = 'recipe' + data[i].Id;
                        data[i].GraphEntity = this.coreService.newEntity();
                        this.coreService.copyEntity(data[i], data[i].GraphEntity);
                    }
                    this.recipesService.recipes = data;
                })
                .error((data, status, headers, config) => {
                })
                .finally(() => {
                    this.isBusy = false;
                });
        }

        // call getTags on tagsService and handle results
        getTags(): void {
            this.isBusy = true;
            this.tags = [];
            this.tagsService.getTags()
                .success((data, status, headers, config) => {
                    this.tags = data;
                })
                .error((data, status, headers, config) => {
                    this.tags = [];
                })
                .finally(() => {
                    var tag: ITagDto = {
                        Description: "All tags",
                        Id: -1,
                        RowVersion: [],
                        AccountId: ''
                    };
                    this.tags.unshift(tag);
                    this.selectedTag = this.tags[0];
                    this.isBusy = false;
                });
        }
    }
}
