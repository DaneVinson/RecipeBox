module Services {

    // Angular service class for interfacing with the tags REST service.
    export class TagsService {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$http'];
        constructor(http: ng.IHttpService) {
            this.httpService = http;
            this.urlBase = './api/tags'
        }

        httpService: ng.IHttpService;
        urlBase: string;

        // delete a tag by id
        deleteTag(id: number): ng.IHttpPromise<any> {
            return this.httpService.delete(this.urlBase + '/' + id);
        }

        // get a tag by id
        getTag(id: number): ng.IHttpPromise<ITag> {
            return this.httpService.get(this.urlBase + '/' + id);
        }

        // get all tags
        getTags(): ng.IHttpPromise<ITag[]> {
            return this.httpService.get(this.urlBase);
        }

        // post a new tag
        postTag(tag: ITagDto): ng.IHttpPromise<ITagDto> {
            return this.httpService.post(this.urlBase, tag);
        }

        // put to update
        putTag(tag: ITagDto): ng.IHttpPromise<ITagDto> {
            return this.httpService.put(this.urlBase, tag);
        }
    }
}