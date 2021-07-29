namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentID = c.Int(nullable: false, identity: true),
                        NameOnCard = c.String(),
                        CardHash = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        PostalCode = c.String(),
                        Province = c.String(),
                        Country = c.String(),
                        InvoiceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentID)
                .ForeignKey("dbo.Invoices", t => t.InvoiceID, cascadeDelete: true)
                .Index(t => t.InvoiceID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "InvoiceID", "dbo.Invoices");
            DropIndex("dbo.Payments", new[] { "InvoiceID" });
            DropTable("dbo.Payments");
        }
    }
}
