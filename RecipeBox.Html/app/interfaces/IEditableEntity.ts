interface IEditableEntity {
    isBusy: boolean;
    isDelete: boolean;
    isEditing: boolean;
    isModify: boolean;
    isNew: boolean;
    isSelected: boolean;
    isValid: boolean;
    keyName: string;    // for html elements "name" attribute value
}
