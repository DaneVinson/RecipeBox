interface ITag extends IEditableEntity, ITagDto {
}

interface ITagDto {
    Description: string;
    Id: number;
    AccountId: string;
    RowVersion: any[];
}
 