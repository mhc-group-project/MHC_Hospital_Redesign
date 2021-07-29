namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAppointmentmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        AId = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Message = c.String(),
                        DateTime = c.String(),
                        Status = c.String(),
                        PatientId = c.String(maxLength: 128),
                        DoctorId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AId)
                .ForeignKey("dbo.AspNetUsers", t => t.DoctorId)
                .ForeignKey("dbo.AspNetUsers", t => t.PatientId)
                .Index(t => t.PatientId)
                .Index(t => t.DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Appointments", "PatientId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Appointments", "DoctorId", "dbo.AspNetUsers");
            DropIndex("dbo.Appointments", new[] { "DoctorId" });
            DropIndex("dbo.Appointments", new[] { "PatientId" });
            DropTable("dbo.Appointments");
        }
    }
}
