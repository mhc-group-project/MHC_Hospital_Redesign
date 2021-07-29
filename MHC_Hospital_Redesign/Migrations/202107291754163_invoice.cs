namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        InvoiceID = c.Int(nullable: false, identity: true),
                        InvoiceNumber = c.Int(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        Amount = c.Int(nullable: false),
                        Currency = c.String(),
                    })
                .PrimaryKey(t => t.InvoiceID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Invoices");
        }
    }
}
