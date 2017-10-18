namespace Wedding.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSearchedTerm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SearchedTerms",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Term = c.String(),
                        Count = c.Int(nullable: false),
                        Inserted = c.DateTime(),
                        Updated = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SearchedTerms");
        }
    }
}
