namespace Wedding.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameVideoSource : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostedItems", "VideoSourceUrl", c => c.String());
            DropColumn("dbo.PostedItems", "YoutubeSourceUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PostedItems", "YoutubeSourceUrl", c => c.String());
            DropColumn("dbo.PostedItems", "VideoSourceUrl");
        }
    }
}
