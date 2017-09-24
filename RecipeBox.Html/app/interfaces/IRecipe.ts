interface IRecipe extends IEditableEntity, IRecipeDto {
    GraphEntity: IEditableEntity;
}

interface IRecipeDto {
    Description: string;
    Directions: string;
    Id: number;
    ImageFileName: string;
    Ingredients: IIngredient[];
    Name: string;
    Notes: string;
    PreparationMinutes: number;
    RowVersion: any[];
    Servings: number;
    Source: string;
    Tags: ITagDto[];
    AccountId: string;
}
 