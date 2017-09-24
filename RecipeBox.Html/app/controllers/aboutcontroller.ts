module Controllers {

    // Controller for the application's About area.
    export class AboutController {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$scope', 'coreService'];
        constructor(scope: any, coreService: Services.CoreService) {
            scope.aboutController = this;

            this.coreService = coreService;
        }


        coreService: Services.CoreService;
    }
}
 