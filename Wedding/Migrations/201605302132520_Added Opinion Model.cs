namespace Wedding.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOpinionModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Opinions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OpinionType = c.Int(nullable: false),
                        Inserted = c.DateTime(),
                        Updated = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        PostedItem_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.PostedItems", t => t.PostedItem_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.PostedItem_Id);
            
            AddColumn("dbo.PostedItems", "OpinionsTokenized", c => c.String());
            AddColumn("dbo.PostedItems", "TotalLike", c => c.Int(nullable: false));
            AddColumn("dbo.PostedItems", "TotalDislike", c => c.Int(nullable: false));
            AddColumn("dbo.PostedItems", "Source", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Opinions", "PostedItem_Id", "dbo.PostedItems");
            DropForeignKey("dbo.Opinions", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Opinions", new[] { "PostedItem_Id" });
            DropIndex("dbo.Opinions", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.PostedItems", "Source");
            DropColumn("dbo.PostedItems", "TotalDislike");
            DropColumn("dbo.PostedItems", "TotalLike");
            DropColumn("dbo.PostedItems", "OpinionsTokenized");
            DropTable("dbo.Opinions");
        }
    }
}
