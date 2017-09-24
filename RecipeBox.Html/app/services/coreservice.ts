module Services {
    export class CoreService {

        static $inject = ['$location', 'localStorageService'];
        constructor(locationService: ng.ILocationService, localStorageService: ng.localStorage.ILocalStorageService) {
            this.authInfo = {
                accessToken: '',
                expirationDate: new Date(),
                tokenType: '',
                userName: ''
            }
            this.authLocalStoreName = 'IAuthInfo';
            this.authText = 'Log In';
            this.azureImagesUri = 'https://recipeboxstorage.blob.core.windows.net/';
            this.localStorageService = localStorageService;
            this.locationService = locationService;
            this.uniqueId = -1;

            // check local storage and set authorization
            var localAuthInfo: IAuthInfo = this.localStorageService.get(this.authLocalStoreName);

            // JSON data doesn't convert to dates that compare correctly with Date
            if (localAuthInfo && localAuthInfo.expirationDate) {
                localAuthInfo.expirationDate = new Date(localAuthInfo.expirationDate.toString());
            }

            // set auth
            this.setAuthenticated(localAuthInfo);

            // refresh the auth token if necessary
            this.checkAndRefreshAuthToken();
        }


        authInfo: IAuthInfo;
        private authLocalStoreName: string;
        authText: string;
        private azureImagesUri: string;
        localStorageService: ng.localStorage.ILocalStorageService;
        locationService: ng.ILocationService;
        uniqueId: number;


        // method called from the ctor which attempts to update the auth token if it exists and will expire withing 12 hours.
        private checkAndRefreshAuthToken() {
            var nowPlus12Hours = new Date();
            nowPlus12Hours.setHours(nowPlus12Hours.getHours() + 12);
            if (this.isAuthenticated() && this.authInfo.expirationDate < nowPlus12Hours) {
                // TODO: refresh bearer token
            }
        }

        // copy all vales from a source entity to a target entity.
        copyEntity(source: IEditableEntity, target: IEditableEntity) {
            target.isBusy = source.isBusy;
            target.isDelete = source.isDelete;
            target.isEditing = source.isEditing;
            target.isModify = source.isModify;
            target.isNew = source.isNew;
            target.isSelected = source.isSelected;
            target.isValid = source.isValid;
            target.keyName = source.keyName;
        }

        // returns a flag indicating whether or not credentials are currently saved.
        credentialsAreSaved(): boolean {
            var localAuthInfo: IAuthInfo = this.localStorageService.get(this.authLocalStoreName);
            return localAuthInfo != null;
        }

        // remove any authInfo in local storage
        deleteLocalCredentials() {
            this.localStorageService.remove(this.authLocalStoreName);
        }

        // method to control the "state" of editable entities based on properties sent from their form controller
        entityChange(entity: IEditableEntity, controller: ng.IFormController) {
            if (!entity || !controller) {
                return;
            }
            entity.isModify = controller.$dirty;
            entity.isValid = controller.$valid;
        }

        // method to return a flag indicating whenther or not an entity's delete operation should be enabled.
        entityDeleteEnabled(entity: IEditableEntity): boolean {
            return entity && !entity.isBusy && !entity.isDelete
        }

        // method to return a flag indicating whenther or not an entity's refresh operation should be enabled.
        entityRefreshEnabled(entity: IEditableEntity): boolean {
            return entity && !entity.isBusy && (entity.isDelete || entity.isModify);
        }

        // method to return a flag indicating whenther or not an entity's save operation should be enabled.
        entitySaveEnabled(entity: IEditableEntity): boolean {
            return entity && !entity.isBusy && (entity.isDelete || (entity.isModify && entity.isValid));
        }

        // method to get the path of an image using its name.
        getRecipeImagePath(imageName: string): string {
            var host = this.locationService.host();

            // localhost use local images otherwise use azure.
            if (host && host === 'localhost') {
                return imageName ? './RecipeImages/' + imageName : './Images/default-recipe.png';
            }
            else {
                return imageName ? this.azureImagesUri + 'recipeimages/' + imageName : this.azureImagesUri + 'siteimages/default-recipe.png';
            }
        }

        getSiteImagePath(imageName: string): string {
            var host = this.locationService.host();

            // localhost use local images otherwise use azure.
            if (host && host === 'localhost') {
                return imageName ? './Images/' + imageName : './Images/default-recipe.png';
            }
            else {
                return imageName ? this.azureImagesUri + 'siteimages/' + imageName : this.azureImagesUri + 'siteimages/' + 'default-recipe.png';
            }
        }

        // initialize an IEditableEntity
        initializeEntity(entity: IEditableEntity) {
            entity.isBusy = false;
            entity.isDelete = false;
            entity.isEditing = false;
            entity.isModify = false;
            entity.isNew = false;
            entity.isSelected = false;
            entity.isValid = true;
            entity.keyName = '';
        }

        // check authentication condition
        isAuthenticated(): boolean {
            return this.authInfo &&
                this.authInfo.userName &&
                this.authInfo.accessToken &&
                this.authInfo.accessToken.length > 0 &&
                new Date() < this.authInfo.expirationDate;
        }

        // log the current user out
        logOut() {
            this.setAuthenticated(null);
            this.deleteLocalCredentials();
            this.locationService.path('/');
        }

        // get a new empty entity with all properties set to type defaults
        newEntity(): IEditableEntity {
            return {
                isBusy: false,
                isDelete: false,
                isEditing: false,
                isModify: false,
                isNew: false,
                isSelected: false,
                isValid: false,
                keyName: ''
            };
        }

        // save the current authInfo credentials object if authenticated
        saveCurrentCredentials() {
            if (this.isAuthenticated()) {
                this.localStorageService.set(this.authLocalStoreName, this.authInfo);
            }
        }

        // set the current instance authentication based on the current value of authInfo
        setAuthenticated(authInfo: IAuthInfo) {
            this.authInfo = authInfo;
            this.authText = this.isAuthenticated() ? this.authInfo.userName : 'Log In';
        }
    }
}