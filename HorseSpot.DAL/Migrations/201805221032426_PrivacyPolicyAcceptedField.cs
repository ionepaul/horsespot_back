namespace HorseSpot.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrivacyPolicyAcceptedField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "PrivacyPolicyAccepted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "PrivacyPolicyAccepted");
        }
    }
}
