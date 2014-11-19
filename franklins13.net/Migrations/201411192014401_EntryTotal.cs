namespace franklins13.net.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntryTotal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entries", "Total", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Entries", "Total");
        }
    }
}
