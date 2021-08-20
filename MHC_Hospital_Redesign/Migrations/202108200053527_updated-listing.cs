namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedlisting : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Listings", "ListTitle", c => c.String(nullable: false));
            AlterColumn("dbo.Listings", "ListDescription", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Listings", "ListDescription", c => c.String());
            AlterColumn("dbo.Listings", "ListTitle", c => c.String());
        }
    }
}
