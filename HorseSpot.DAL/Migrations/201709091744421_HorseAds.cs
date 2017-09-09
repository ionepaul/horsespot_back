namespace HorseSpot.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HorseAds : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        Country = c.String(),
                        City = c.String(),
                        Street = c.String(),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.HorseAds", t => t.AddressId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.HorseAds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        DatePosted = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VideoLink = c.String(),
                        Views = c.Int(nullable: false),
                        IsSponsorized = c.Boolean(nullable: false),
                        IsValidated = c.Boolean(nullable: false),
                        HorseName = c.String(nullable: false),
                        Gender = c.String(nullable: false),
                        Age = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Breed = c.String(nullable: false),
                        HaveXRays = c.Boolean(nullable: false),
                        HaveCompetionalExperience = c.Boolean(nullable: false),
                        UserId = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        IsSold = c.Boolean(nullable: false),
                        UserModel_Id = c.String(maxLength: 128),
                        PriceRange_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserModel_Id)
                .ForeignKey("dbo.PriceRanges", t => t.PriceRange_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.UserModel_Id)
                .Index(t => t.PriceRange_Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        ImageId = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.ImageId);
            
            CreateTable(
                "dbo.Pedigrees",
                c => new
                    {
                        PedigreeId = c.Int(nullable: false, identity: true),
                        Father = c.String(),
                        Father_Father = c.String(),
                        Father_Mother = c.String(),
                        Father_Father_Father = c.String(),
                        Father_Father_Mother = c.String(),
                        Father_Mother_Father = c.String(),
                        Father_Mother_Mother = c.String(),
                        Mother = c.String(),
                        Mother_Father = c.String(),
                        Mother_Mother = c.String(),
                        Mother_Father_Father = c.String(),
                        Mother_Father_Mother = c.String(),
                        Mother_Mother_Father = c.String(),
                        Mother_Mother_Mother = c.String(),
                    })
                .PrimaryKey(t => t.PedigreeId)
                .ForeignKey("dbo.HorseAds", t => t.PedigreeId)
                .Index(t => t.PedigreeId);
            
            CreateTable(
                "dbo.HorseAbilityHorseAds",
                c => new
                    {
                        HorseAbility_Id = c.Int(nullable: false),
                        HorseAd_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.HorseAbility_Id, t.HorseAd_Id })
                .ForeignKey("dbo.HorseAbilities", t => t.HorseAbility_Id, cascadeDelete: true)
                .ForeignKey("dbo.HorseAds", t => t.HorseAd_Id, cascadeDelete: true)
                .Index(t => t.HorseAbility_Id)
                .Index(t => t.HorseAd_Id);
            
            CreateTable(
                "dbo.ImageHorseAds",
                c => new
                    {
                        Image_ImageId = c.Int(nullable: false),
                        HorseAd_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Image_ImageId, t.HorseAd_Id })
                .ForeignKey("dbo.Images", t => t.Image_ImageId, cascadeDelete: true)
                .ForeignKey("dbo.HorseAds", t => t.HorseAd_Id, cascadeDelete: true)
                .Index(t => t.Image_ImageId)
                .Index(t => t.HorseAd_Id);
            
            CreateTable(
                "dbo.RecommendedRiderHorseAds",
                c => new
                    {
                        RecommendedRider_Id = c.Int(nullable: false),
                        HorseAd_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RecommendedRider_Id, t.HorseAd_Id })
                .ForeignKey("dbo.RecommendedRiders", t => t.RecommendedRider_Id, cascadeDelete: true)
                .ForeignKey("dbo.HorseAds", t => t.HorseAd_Id, cascadeDelete: true)
                .Index(t => t.RecommendedRider_Id)
                .Index(t => t.HorseAd_Id);
            
            AddColumn("dbo.Appointments", "IsCanceled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "HorseAd_Id", c => c.Int());
            AlterColumn("dbo.Appointments", "AdvertismentId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "HorseAd_Id");
            CreateIndex("dbo.Appointments", "AdvertismentId");
            AddForeignKey("dbo.AspNetUsers", "HorseAd_Id", "dbo.HorseAds", "Id");
            AddForeignKey("dbo.Appointments", "AdvertismentId", "dbo.HorseAds", "Id", cascadeDelete: true);
            DropTable("dbo.Genders");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Genders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GenderValue = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Appointments", "AdvertismentId", "dbo.HorseAds");
            DropForeignKey("dbo.Addresses", "AddressId", "dbo.HorseAds");
            DropForeignKey("dbo.HorseAds", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RecommendedRiderHorseAds", "HorseAd_Id", "dbo.HorseAds");
            DropForeignKey("dbo.RecommendedRiderHorseAds", "RecommendedRider_Id", "dbo.RecommendedRiders");
            DropForeignKey("dbo.HorseAds", "PriceRange_Id", "dbo.PriceRanges");
            DropForeignKey("dbo.Pedigrees", "PedigreeId", "dbo.HorseAds");
            DropForeignKey("dbo.ImageHorseAds", "HorseAd_Id", "dbo.HorseAds");
            DropForeignKey("dbo.ImageHorseAds", "Image_ImageId", "dbo.Images");
            DropForeignKey("dbo.AspNetUsers", "HorseAd_Id", "dbo.HorseAds");
            DropForeignKey("dbo.HorseAds", "UserModel_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.HorseAbilityHorseAds", "HorseAd_Id", "dbo.HorseAds");
            DropForeignKey("dbo.HorseAbilityHorseAds", "HorseAbility_Id", "dbo.HorseAbilities");
            DropIndex("dbo.RecommendedRiderHorseAds", new[] { "HorseAd_Id" });
            DropIndex("dbo.RecommendedRiderHorseAds", new[] { "RecommendedRider_Id" });
            DropIndex("dbo.ImageHorseAds", new[] { "HorseAd_Id" });
            DropIndex("dbo.ImageHorseAds", new[] { "Image_ImageId" });
            DropIndex("dbo.HorseAbilityHorseAds", new[] { "HorseAd_Id" });
            DropIndex("dbo.HorseAbilityHorseAds", new[] { "HorseAbility_Id" });
            DropIndex("dbo.Appointments", new[] { "AdvertismentId" });
            DropIndex("dbo.Pedigrees", new[] { "PedigreeId" });
            DropIndex("dbo.AspNetUsers", new[] { "HorseAd_Id" });
            DropIndex("dbo.HorseAds", new[] { "PriceRange_Id" });
            DropIndex("dbo.HorseAds", new[] { "UserModel_Id" });
            DropIndex("dbo.HorseAds", new[] { "UserId" });
            DropIndex("dbo.Addresses", new[] { "AddressId" });
            AlterColumn("dbo.Appointments", "AdvertismentId", c => c.String(nullable: false));
            DropColumn("dbo.AspNetUsers", "HorseAd_Id");
            DropColumn("dbo.Appointments", "IsCanceled");
            DropTable("dbo.RecommendedRiderHorseAds");
            DropTable("dbo.ImageHorseAds");
            DropTable("dbo.HorseAbilityHorseAds");
            DropTable("dbo.Pedigrees");
            DropTable("dbo.Images");
            DropTable("dbo.HorseAds");
            DropTable("dbo.Addresses");
        }
    }
}
