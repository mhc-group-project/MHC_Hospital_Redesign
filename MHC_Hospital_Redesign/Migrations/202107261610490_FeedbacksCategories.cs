namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeedbacksCategories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeedbackCategories",
                c => new
                    {
                        FeedbackCategoryID = c.Int(nullable: false, identity: true),
                        FeedbackCategoryName = c.String(),
                        FeedbackCategoryColor = c.String(),
                    })
                .PrimaryKey(t => t.FeedbackCategoryID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FeedbackCategories");
        }
    }
}
