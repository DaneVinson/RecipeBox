angular.module('AppFilters', [])
    .filter('BootstrapDangerSaveFilter', function () {
        return function (entityIsDelete: boolean): string {
            return entityIsDelete ? 'danger' : 'success';
        };
    })
    .filter('BootstrapErrorFilter', function () {
        return function (isError: boolean): string {
            return isError ? 'has-error' : '';
        };
    })
    .filter('BootstrapSelectButtonItemFilter', function () {
        return function (isSelected) {
            return isSelected ? 'btn btn-sm btn-primary' : 'btn btn-sm btn-default';
        };
    })
    .filter('CollapsedGlyphFilter', function () {
        return function (isCollapsed: boolean): string {
            return isCollapsed ? 'glyphicon glyphicon-chevron-down' : 'glyphicon glyphicon-chevron-up';
        };
    })
    .filter('CollapsedPanelFilter', function () {
        return function (isCollapsed: boolean): string {
            return isCollapsed ? 'panel-collapse collapse' : 'panel-collapse collapse in';
        };
    })
    .filter('DeleteOrCancelGlyphFilter', function () {
        return function (isDelete: boolean): string {
            return isDelete ? 'glyphicon glyphicon-refresh' : 'glyphicon glyphicon-remove';
        };
    })
    .filter('EntityStateToBootstrapFilter', function () {
        return function (entity: IEditableEntity): string {
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
        return function (value: IRecipeDto[]): string {
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
        return function (value: string): string {
            return (!value || value.trim().length < 1) ? 'has-error' : '';
        };
    })
    .filter('ServingsPluralizationFilter', function () {
        return function (value: number): string {
            return (value === 1) ? 'serving' : 'servings';
        };
    })
    .filter('TagsReadableListFilter', function () {
        return function (tags: ITagDto[]): string {
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
 