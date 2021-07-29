namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class feedbackContent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedbacks", "FeedbackContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Feedbacks", "FeedbackContent");
        }
    }
}
