namespace Wedding.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedslug : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostedItems", "Slug", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostedItems", "Slug");
        }
    }
}
