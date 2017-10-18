namespace Wedding.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVideoSourceName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostedItems", "VideoSourceName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostedItems", "VideoSourceName");
        }
    }
}
