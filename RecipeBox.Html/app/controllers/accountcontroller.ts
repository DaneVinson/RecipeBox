module Controllers {

    // Controller for the account registraiton and log in area.
    export class AccountController {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$scope', '$location', 'coreService', 'accountService'];
        constructor(scope: any, locationService: ng.ILocationService, coreService: Services.CoreService, accountService: Services.AccountService) {
            scope.accountController = this;

            this.coreService = coreService;
            this.accountService = accountService;
            this.isBusy = false;
            this.isRegistering = false;
            this.locationService = locationService;
            this.loginPassword = '';
            this.loginUserName = '';
            this.remember = this.coreService.credentialsAreSaved();
            this.serverErrorMessage = '';

            // if authenticated fetch account for maintenance otherwise log in
            if (this.coreService.isAuthenticated()) {
                this.title = 'Account Details';
                this.isBusy = true;
                this.getAccount();
            }
            else {
                this.setDefaultAccount();
                this.title = 'Credentials';
            }
        }


        account: IAccount;
        accountService: Services.AccountService;
        coreService: Services.CoreService;
        isBusy: boolean;
        isRegistering: boolean;
        locationService: ng.ILocationService;
        loginPassword: string;
        loginUserName: string;
        remember: boolean;
        serverErrorMessage: string;
        title: string;


        // method for processing account change events
        accountChange(controller: ng.IFormController) {
            this.coreService.entityChange(this.account, controller);
        }

        // cancel account maintenance or registration
        cancel() {
            if (this.isRegistering) {
                this.isRegistering = false;
                this.serverErrorMessage = '';
                this.title = 'Credentials';
            }
            else {
                this.getAccount();
            }
        }

        // get the user's account
        getAccount(): void {
            this.accountService.getAccount(this.coreService.authInfo.userName)
                .success((data, status, headers, config) => {
                    this.coreService.initializeEntity(data);
                    this.account = data;
                    this.account.PasswordConfirm = this.account.Password;
                })
                .error((data, status, headers, config) => {
                    this.setDefaultAccount();
                })
                .finally(() => {
                    this.serverErrorMessage = '';
                    this.isBusy = false;
                });
        }

        // log the user in
        logIn() {
            this.isBusy = true;
            this.accountService.authenticateAccount(this.loginUserName, this.loginPassword)
                .success((data, status, headers, config) => {
                    // determine the expiration date
                    var expirationDate = new Date();
                    expirationDate.setSeconds(expirationDate.getSeconds() + data.expires_in);

                    // setup session auth
                    var authInfo: IAuthInfo = {
                        accessToken: data.access_token,
                        expirationDate: expirationDate,
                        tokenType: data.token_type,
                        userName: this.loginUserName
                    };
                    this.coreService.setAuthenticated(authInfo);

                    // save credentials locally if requested
                    if (this.remember) {
                        this.coreService.saveCurrentCredentials();
                    }
                    else {
                        this.coreService.deleteLocalCredentials();
                    }

                    // reset authentication attempts to 0
                    //this.accountService.authenticationAttempts = 0;

                    // go home
                    this.locationService.path('/');
                })
                .error((data, status, headers, config) => {
                    this.isRegistering = false;
                    this.coreService.setAuthenticated(null);
                    this.loginPassword = '';
                })
                .finally(() => {
                    this.isBusy = false;
                });
        }

        // initialize for registration
        register() {
            this.isRegistering = true;
            this.setDefaultAccount();
            this.title = 'Create an Account';
        }

        // mark the account as deleted
        removeAccount() {
            this.account.isDelete = true;
        }

        // save changes to the current account
        saveAccount(formController: ng.IFormController) {
            this.isBusy = true;

            // delete the account and log out if delete is marked
            if (this.account.isDelete) {
                this.accountService.deleteAccount(this.account.Id)
                    .success((data, status, header, config) => {
                    })
                    .error((data, status, header, config) => {
                    })
                    .finally(() => {
                        this.setDefaultAccount();
                        this.isBusy;
                        this.coreService.logOut();
                    });
            }

            // otherwise create or update
            else {
                var promise: ng.IHttpPromise<IAccountDto> = this.account.isNew ? this.accountService.postAccount(this.account) : this.accountService.putAccount(this.account);
                promise
                    .success((data, status, header, config) => {

                        // account.Password is unencrypted if this is a registration otherwise it's encrypted
                        this.loginPassword = this.account.Password;

                        this.account.AuthProvider = data.AuthProvider;
                        this.account.EmailAddress = data.EmailAddress;
                        this.account.Id = data.Id;
                        this.account.isBusy = false;
                        this.account.isDelete = false;
                        this.account.isEditing = false;
                        this.account.isModify = false;
                        this.account.isNew = false;
                        this.account.isSelected = false;
                        this.account.isValid = true;
                        this.account.Password = data.Password;
                        this.account.PasswordConfirm = data.Password;
                        this.account.RowVersion = data.RowVersion;
                        this.account.UserName = data.UserName;

                        // handle post results if authenticatied or if registering or otherwise.
                        if (this.coreService.isAuthenticated()) {
                            this.isBusy = false;
                            this.locationService.path('/');
                            this.coreService.authInfo.userName = this.account.UserName;
                            this.coreService.setAuthenticated(this.coreService.authInfo);
                            if (this.coreService.credentialsAreSaved()) {
                                this.coreService.saveCurrentCredentials();
                            }
                        }
                        else if (this.isRegistering) {
                            this.locationService.path('/');
                        }
                        else {
                            this.loginUserName = this.account.UserName;
                            this.isBusy = false;
                            this.logIn();
                        }
                    })
                    .error((data, status, header, config) => {
                        // data is not IAccountDto for error, it's an object with a Message property.
                        var response: any = data;
                        if (response && response.Message) {
                            this.serverErrorMessage = response.Message;
                        }
                        else {
                            this.serverErrorMessage = 'Unknown error encountered saving account.';
                        }
                        this.account.isValid = false;
                        this.isBusy = false;
                    })
                    .finally(() => {
                    });
            }
        }

        // initialize account with an new empty default
        setDefaultAccount(): void {
            this.account = {
                AuthProvider: 'RecipeBox',
                EmailAddress: '',
                keyName: '',
                Id: '',
                isBusy: false,
                isDelete: false,
                isEditing: true,
                isModify: false,
                isNew: true,
                isSelected: false,
                isValid: false,
                Password: '',
                PasswordConfirm: '',
                RowVersion: [],
                UserName: ''
            };
            this.loginPassword = '';
            this.loginUserName = '';
            this.serverErrorMessage = '';
        }

        // the account view is log in (true) or registration/maintenance (false)
        showLogIn(): boolean {
            return !this.coreService.isAuthenticated() && !this.isRegistering;
        }
    }
}