namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPasswordToAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "AuthProvider", c => c.String(nullable: false));
            AddColumn("dbo.Accounts", "Password", c => c.String(nullable: false));
            DropColumn("dbo.Accounts", "AuthorizationProvider");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "AuthorizationProvider", c => c.String(nullable: false));
            DropColumn("dbo.Accounts", "Password");
            DropColumn("dbo.Accounts", "AuthProvider");
        }
    }
}
