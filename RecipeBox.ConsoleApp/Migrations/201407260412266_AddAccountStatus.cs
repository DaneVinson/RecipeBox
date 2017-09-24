namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Status", c => c.String());

            // Any accounts before Status were added should be considered Active
            Sql("UPDATE Accounts SET Status = 'Active'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Status");
        }
    }
}
