interface IIngredient extends IEditableEntity, IIngredientDto {
}

interface IIngredientDto {
    Description: string;
    Id: number;
    Quantity: string;
    RecipeId: number;
    RowVersion: any[];
    Units: string;
}
