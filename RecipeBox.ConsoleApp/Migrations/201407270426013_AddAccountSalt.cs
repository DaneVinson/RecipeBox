namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountSalt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Salt", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Salt");
        }
    }
}
