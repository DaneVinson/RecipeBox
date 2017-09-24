// TODO: Why isn't this working as a .factory reference from app.ts
module Services {

    // Interceptor service to handle auth demands
    export class AuthInterceptorService {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$q', '$location', 'coreService'];
        constructor(q: ng.IQService, location: ng.ILocationService, coreService: Services.CoreService) {
            this.coreService = coreService
            this.locationService = location;
            this.qService = q;
        }


        coreService: Services.CoreService;
        locationService: ng.ILocationService;
        qService: ng.IQService;


        request(config: ng.IRequestConfig): ng.IRequestConfig {
            if (this.coreService.isAuthenticated()) {
                config.headers.Authorization = 'Bearer ' + this.coreService.authInfo.accessToken;
            }
            return config;
        }

        responseError(rejection: any) {
            if (rejection.status === 401) {
                this.locationService.path('/account');
            }
            return this.qService.reject(rejection);
        }
    }
}
