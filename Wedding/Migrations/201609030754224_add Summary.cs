namespace Wedding.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSummary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostedItems", "Summary", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostedItems", "Summary");
        }
    }
}
