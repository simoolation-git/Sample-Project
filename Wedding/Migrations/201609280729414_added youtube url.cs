namespace Wedding.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedyoutubeurl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostedItems", "YoutubeVideoUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostedItems", "YoutubeVideoUrl");
        }
    }
}
