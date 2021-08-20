namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedlisting2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Listings", "DepartmentName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Listings", "DepartmentName", c => c.String());
        }
    }
}
