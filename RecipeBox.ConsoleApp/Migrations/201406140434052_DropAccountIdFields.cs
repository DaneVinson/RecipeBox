namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropAccountIdFields : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Accounts");
            AddColumn("dbo.Accounts", "UserName", c => c.String(nullable: false, maxLength: 50));
            AddPrimaryKey("dbo.Accounts", "UserName");
            DropColumn("dbo.Accounts", "Id");
            DropColumn("dbo.Accounts", "ClientId");
            DropColumn("dbo.Accounts", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "Name", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Accounts", "ClientId", c => c.String(nullable: false));
            AddColumn("dbo.Accounts", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Accounts");
            DropColumn("dbo.Accounts", "UserName");
            AddPrimaryKey("dbo.Accounts", "Id");
        }
    }
}
