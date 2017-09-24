module Services {

    // Angular service class for interfacing with the account REST service.
    export class AccountService {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$http'];
        constructor(http: ng.IHttpService) {
            this.authenticationAttempts = 0;
            this.httpService = http;
            this.tokenUrl = './token';
            this.urlBase = './api/account';
        }


        authenticationAttempts: number;
        httpService: ng.IHttpService;
        tokenUrl: string;
        urlBase: string;


        // authenticate
        authenticateAccount(userName: string, password: string): ng.IHttpPromise<IAuthResponse> {
            var authData = "grant_type=password&username=" + userName + "&password=" + password;
            var config = { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } };
            //return this.httpService.post(this.tokenUrl, data, config);

            var self = this;
            return this.httpService.post(this.tokenUrl, authData, config)
                .success(() => {
                    self.authenticationAttempts = 0;
                })
                .error(() => {
                    self.authenticationAttempts++;
                });
        }

        // delete a account by id
        deleteAccount(id: string): ng.IHttpPromise<any> {
            return this.httpService.delete(this.urlBase + '/' + id);
        }

        // get a account by user name
        getAccount(userName: string): ng.IHttpPromise<IAccount> {
            return this.httpService.get(this.urlBase + '/' + userName);
        }

        // post a new account
        postAccount(account: IAccountDto): ng.IHttpPromise<IAccountDto> {
            return this.httpService.post(this.urlBase, account);
        }

        // put to update
        putAccount(account: IAccountDto): ng.IHttpPromise<IAccountDto> {
            return this.httpService.put(this.urlBase, account);
        }
    }
}