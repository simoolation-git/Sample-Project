namespace Wedding.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedOpinion : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Opinions", "OpinionType", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Opinions", "OpinionType", c => c.Int(nullable: false));
        }
    }
}
