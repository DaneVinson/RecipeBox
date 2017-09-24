interface IAccount extends IEditableEntity, IAccountDto {
}

interface IAccountDto {
    EmailAddress: string;
    AuthProvider: string;
    Id: string;
    Password: string;
    PasswordConfirm: string;
    RowVersion: any[];
    UserName: string;
}
 