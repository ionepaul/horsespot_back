namespace HorseSpot.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GdprUserFieldsUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "TermsAccepted", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "DisplayEmail", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "DisplayPhoneNumber", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DisplayPhoneNumber");
            DropColumn("dbo.AspNetUsers", "DisplayEmail");
            DropColumn("dbo.AspNetUsers", "TermsAccepted");
        }
    }
}
