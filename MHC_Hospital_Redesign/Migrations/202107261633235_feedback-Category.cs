namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class feedbackCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedbacks", "FeedbackCategoryID", c => c.Int(nullable: false));
            CreateIndex("dbo.Feedbacks", "FeedbackCategoryID");
            AddForeignKey("dbo.Feedbacks", "FeedbackCategoryID", "dbo.FeedbackCategories", "FeedbackCategoryID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Feedbacks", "FeedbackCategoryID", "dbo.FeedbackCategories");
            DropIndex("dbo.Feedbacks", new[] { "FeedbackCategoryID" });
            DropColumn("dbo.Feedbacks", "FeedbackCategoryID");
        }
    }
}
