namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDepartmentmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.DId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Departments");
        }
    }
}
