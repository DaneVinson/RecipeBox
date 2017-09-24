module Controllers {
    export class TagsController {

        // $inject is a pattern to handle variable renaming during minification.
        static $inject = ['$scope', 'coreService', 'tagsService'];
        constructor(scope: any, coreService: Services.CoreService, tagsService: Services.TagsService) {
            scope.tagsController = this;
            this.coreService = coreService;
            this.description = 'Tags are attached to recipes aiding in categorization and searching. Here you can view, create, edit and delete your personal tags.';
            this.emptyDescriptionText = '{Description}';
            this.isBusy = false;
            this.newTagId = 0;
            this.tags = [];
            this.tagsService = tagsService;
            this.title = 'Recipe Tags';
            this.getTags();
        }

        // properties
        coreService: Services.CoreService;
        description: string;
        emptyDescriptionText: string;
        isBusy: boolean;
        newTagId: number;
        scope: any;
        tags: ITag[];
        tagsService: Services.TagsService;
        title: string;

        // create a new tag and add it to the tags array
        addNewTag() {
            this.newTagId++;
            this.tags.unshift({
                Description: '',
                Id: -1 * this.newTagId,
                isBusy: false,
                isDelete: false,
                isEditing: false,
                isModify: true,
                isNew: true,
                isSelected: false,
                isValid: false,
                keyName: 'newtag' + this.newTagId, 
                RowVersion: null,
                AccountId: '1234567890'
            });
        }

        // apply the 'delete' action to the input tag
        applyDelete(tag: ITag) {
            tag.isDelete = true;
            if (tag.isNew) {
                this.removeTag(tag.Id);
            }
        }

        // apply the 'refresh' action to the input tag
        applyRefresh(tag: ITag, formController: ng.IFormController) {
            if (tag.isNew) {
                this.removeTag(tag.Id);
            }
            else {
                this.refreshTag(tag, formController);
            }
        }

        // apply the 'save' action to the input tag
        applySave(tag: ITag, formController: ng.IFormController) {
            if (formController.$invalid) {
                return;
            }
            tag.isBusy = true;
            if (tag.isDelete) {
                this.tagsService.deleteTag(tag.Id)
                    .success((data, status, header, config) => {
                        this.removeTag(tag.Id);
                    })
                    .error((data, status, header, config) => {
                    })
                    .finally(() => {
                        tag.isBusy = false;
                    });
            }
            else if (tag.isNew || formController.$dirty) {
                var promise: ng.IHttpPromise<ITagDto> = tag.isNew ? this.tagsService.postTag(tag) : this.tagsService.putTag(tag);
                promise
                    .success((tagDto, status, header, config) => {
                        this.coreService.initializeEntity(tag);
                        tag.Description = tagDto.Description;
                        tag.Id = tagDto.Id;
                        tag.keyName = 'tag' + tagDto.Id;
                        tag.RowVersion = tagDto.RowVersion;
                        tag.AccountId = tagDto.AccountId;
                        formController.$setPristine();
                    })
                    .error((data, status, header, config) => {
                    })
                    .finally(() => {
                        tag.isBusy = false;
                    });
            }
            else {
                tag.isBusy = false;
            }
        }

        // call getTags on tagsService and assign controller tags
        getTags() {
            this.isBusy = true;
            this.tagsService.getTags()
                .success((data, status, headers, config) => {
                    for (var i: number = 0; i < data.length; i++) {
                        this.coreService.initializeEntity(data[i]);
                        data[i].isBusy = false;
                        data[i].keyName = 'tag' + data[i].Id;
                    }
                    this.tags = data;
                })
                .error((data, status, headers, config) => {
                    this.tags = [];
                })
                .finally(() => {
                    this.isBusy = false;
                });
        }

        // refresh the input tag
        refreshTag(tag: ITag, formController: ng.IFormController) {
            tag.isBusy = true;
            this.tagsService.getTag(tag.Id)
                .success((data, status, headers, config) => {
                    var isEditing = tag.isEditing;
                    this.coreService.initializeEntity(tag);
                    tag.Description = data.Description;
                    tag.isEditing = isEditing;
                    tag.keyName = 'tag' + tag.Id;
                    formController.$setPristine();
                })
                .error(function (data, status, headers, config) {
                })
                .finally(() => {
                    tag.isBusy = false;
                });
        }

        // remove the tag with the input Id value from the array
        removeTag(id: number) {
            for (var i = 0; i < this.tags.length; i++) {
                if (this.tags[i].Id === id) {
                    this.tags.splice(i, 1);
                    return;
                }
            }
        }

        // toggle the isEditing property on the input tag
        toggleTagEditing(tag: ITag) {
            tag.isEditing = !tag.isEditing;
            // TODO: get rid of dom reference with angular behavior
            $('#' + tag.keyName).collapse('toggle')
        }
    }
}