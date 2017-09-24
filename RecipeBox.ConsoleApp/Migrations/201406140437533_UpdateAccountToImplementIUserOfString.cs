namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAccountToImplementIUserOfString : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Accounts");
            AddColumn("dbo.Accounts", "Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Accounts", "AccountId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Accounts", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Accounts");
            DropColumn("dbo.Accounts", "AccountId");
            DropColumn("dbo.Accounts", "Id");
            AddPrimaryKey("dbo.Accounts", "UserName");
        }
    }
}
