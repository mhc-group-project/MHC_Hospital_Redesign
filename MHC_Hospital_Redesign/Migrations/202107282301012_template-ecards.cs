namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class templateecards : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ecards",
                c => new
                    {
                        EcardId = c.Int(nullable: false, identity: true),
                        SenderName = c.String(),
                        PatientName = c.String(),
                        Message = c.String(),
                        TemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EcardId)
                .ForeignKey("dbo.Templates", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId);
            
            CreateTable(
                "dbo.Templates",
                c => new
                    {
                        TemplateId = c.Int(nullable: false, identity: true),
                        TemplateName = c.String(),
                        TemplateHasPic = c.Boolean(nullable: false),
                        TemplatePicExtension = c.String(),
                        TemplateStyle = c.String(),
                    })
                .PrimaryKey(t => t.TemplateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ecards", "TemplateId", "dbo.Templates");
            DropIndex("dbo.Ecards", new[] { "TemplateId" });
            DropTable("dbo.Templates");
            DropTable("dbo.Ecards");
        }
    }
}
