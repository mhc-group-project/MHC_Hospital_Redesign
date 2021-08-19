namespace MHC_Hospital_Redesign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        AId = c.Int(nullable: false, identity: true),
                        Subject = c.String(nullable: false),
                        Message = c.String(nullable: false),
                        DateTime = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        PatientId = c.String(maxLength: 128),
                        DoctorId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AId)
                .ForeignKey("dbo.AspNetUsers", t => t.DoctorId)
                .ForeignKey("dbo.AspNetUsers", t => t.PatientId)
                .Index(t => t.PatientId)
                .Index(t => t.DoctorId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        ContactMethod = c.String(),
                        Address = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.DId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Listings",
                c => new
                    {
                        ListID = c.Int(nullable: false, identity: true),
                        ListTitle = c.String(),
                        ListDate = c.DateTime(nullable: false),
                        ListDescription = c.String(),
                        ListRequirements = c.String(),
                        ListLocation = c.String(),
                        DepartmentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ListID)
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .Index(t => t.DepartmentID);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Ecards",
                c => new
                    {
                        EcardId = c.Int(nullable: false, identity: true),
                        SenderName = c.String(nullable: false),
                        PatientName = c.String(nullable: false),
                        Message = c.String(nullable: false),
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
                        TemplateHasStyle = c.Boolean(nullable: false),
                        TemplateStyleExtension = c.String(),
                    })
                .PrimaryKey(t => t.TemplateId);
            
            CreateTable(
                "dbo.FaqCategories",
                c => new
                    {
                        FaqCategoryID = c.Int(nullable: false, identity: true),
                        CategoryDateAdded = c.DateTime(nullable: false),
                        CategoryName = c.String(nullable: false),
                        CategoryDescription = c.String(nullable: false),
                        CategoryColor = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.FaqCategoryID);
            
            CreateTable(
                "dbo.Faqs",
                c => new
                    {
                        FaqID = c.Int(nullable: false, identity: true),
                        DateAdded = c.DateTime(nullable: false),
                        FaqQuestions = c.String(nullable: false),
                        FaqAnswers = c.String(nullable: false),
                        FaqSort = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FaqID);
            
            CreateTable(
                "dbo.FeedbackCategories",
                c => new
                    {
                        FeedbackCategoryID = c.Int(nullable: false, identity: true),
                        FeedbackCategoryName = c.String(),
                        FeedbackCategoryColor = c.String(),
                    })
                .PrimaryKey(t => t.FeedbackCategoryID);
            
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        FeedbackId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        FeedbackContent = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                        FeedbackCategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FeedbackId)
                .ForeignKey("dbo.FeedbackCategories", t => t.FeedbackCategoryID, cascadeDelete: true)
                .Index(t => t.FeedbackCategoryID);
            
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
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentID = c.Int(nullable: false, identity: true),
                        NameOnCard = c.String(),
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
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ListingApplicationUsers",
                c => new
                    {
                        Listing_ListID = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Listing_ListID, t.ApplicationUser_Id })
                .ForeignKey("dbo.Listings", t => t.Listing_ListID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Listing_ListID)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.FaqFaqCategories",
                c => new
                    {
                        Faq_FaqID = c.Int(nullable: false),
                        FaqCategory_FaqCategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Faq_FaqID, t.FaqCategory_FaqCategoryID })
                .ForeignKey("dbo.Faqs", t => t.Faq_FaqID, cascadeDelete: true)
                .ForeignKey("dbo.FaqCategories", t => t.FaqCategory_FaqCategoryID, cascadeDelete: true)
                .Index(t => t.Faq_FaqID)
                .Index(t => t.FaqCategory_FaqCategoryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Payments", "InvoiceID", "dbo.Invoices");
            DropForeignKey("dbo.Feedbacks", "FeedbackCategoryID", "dbo.FeedbackCategories");
            DropForeignKey("dbo.FaqFaqCategories", "FaqCategory_FaqCategoryID", "dbo.FaqCategories");
            DropForeignKey("dbo.FaqFaqCategories", "Faq_FaqID", "dbo.Faqs");
            DropForeignKey("dbo.Ecards", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.Appointments", "PatientId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Appointments", "DoctorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Departments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Listings", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.ListingApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ListingApplicationUsers", "Listing_ListID", "dbo.Listings");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.FaqFaqCategories", new[] { "FaqCategory_FaqCategoryID" });
            DropIndex("dbo.FaqFaqCategories", new[] { "Faq_FaqID" });
            DropIndex("dbo.ListingApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ListingApplicationUsers", new[] { "Listing_ListID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Payments", new[] { "InvoiceID" });
            DropIndex("dbo.Feedbacks", new[] { "FeedbackCategoryID" });
            DropIndex("dbo.Ecards", new[] { "TemplateId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Listings", new[] { "DepartmentID" });
            DropIndex("dbo.Departments", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Appointments", new[] { "DoctorId" });
            DropIndex("dbo.Appointments", new[] { "PatientId" });
            DropTable("dbo.FaqFaqCategories");
            DropTable("dbo.ListingApplicationUsers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Payments");
            DropTable("dbo.Invoices");
            DropTable("dbo.Feedbacks");
            DropTable("dbo.FeedbackCategories");
            DropTable("dbo.Faqs");
            DropTable("dbo.FaqCategories");
            DropTable("dbo.Templates");
            DropTable("dbo.Ecards");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Listings");
            DropTable("dbo.Departments");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Appointments");
        }
    }
}
