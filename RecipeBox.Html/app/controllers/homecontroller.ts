module Controllers {

    // Controller for the application's Home area.
    export class HomeController {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$scope', 'coreService'];
        constructor(scope: any, coreService: Services.CoreService) {
            scope.homeController = this;

            this.coreService = coreService;
        }


        coreService: Services.CoreService;
    }
}
 