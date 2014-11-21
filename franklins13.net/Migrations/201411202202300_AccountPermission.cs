namespace franklins13.net.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountPermission : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Permission = c.String(),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountPermissions", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.AccountPermissions", new[] { "UserID" });
            DropTable("dbo.AccountPermissions");
        }
    }
}
