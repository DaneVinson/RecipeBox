namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplaceUserWithAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorizationProvider = c.String(nullable: false),
                        ClientId = c.String(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Recipes", "AccountId", c => c.Int(nullable: false));
            AddColumn("dbo.Tags", "AccountId", c => c.Int(nullable: false));
            DropColumn("dbo.Recipes", "UserId");
            DropColumn("dbo.Tags", "UserId");
            DropTable("dbo.Users");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountName = c.String(nullable: false, maxLength: 50),
                        EmailAddress = c.String(nullable: false, maxLength: 100),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Tags", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.Recipes", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.Tags", "AccountId");
            DropColumn("dbo.Recipes", "AccountId");
            DropTable("dbo.Accounts");
        }
    }
}
