module Controllers {

    // Controller for the application's navbar.
    export class NavController {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$scope', '$location', 'coreService'];
        constructor(scope: any, location: ng.ILocationService, coreService: Services.CoreService) {
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

        coreService: Services.CoreService;
        locationService: ng.ILocationService;
        navAreas: INavArea[];

        areaIsActive(name: string) {
            if (!name) {
                return false;
            }
            var path = '/' + name;
            return this.locationService.path().substr(0, path.length) === path;
        }
    }
}