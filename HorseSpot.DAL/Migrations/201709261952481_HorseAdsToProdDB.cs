namespace HorseSpot.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HorseAdsToProdDB : DbMigration
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
                .PrimaryKey(t => t.AddressId);
            
            CreateTable(
                "dbo.UserFavoriteHorseAds",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        HorseAdId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.HorseAdId })
                .ForeignKey("dbo.HorseAds", t => t.HorseAdId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.HorseAdId);
            
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
                        AddressId = c.Int(nullable: false),
                        PedigreeId = c.Int(nullable: false),
                        PriceRangeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: true)
                .ForeignKey("dbo.Pedigrees", t => t.PedigreeId, cascadeDelete: true)
                .ForeignKey("dbo.PriceRanges", t => t.PriceRangeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AddressId)
                .Index(t => t.PedigreeId)
                .Index(t => t.PriceRangeId);
            
            CreateTable(
                "dbo.ImageModels",
                c => new
                    {
                        ImageId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsProfilePic = c.Boolean(nullable: false),
                        HorseAdId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ImageId)
                .ForeignKey("dbo.HorseAds", t => t.HorseAdId, cascadeDelete: true)
                .Index(t => t.HorseAdId);
            
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
                .PrimaryKey(t => t.PedigreeId);
            
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
            AlterColumn("dbo.Appointments", "AdvertismentId", c => c.Int(nullable: false));
            CreateIndex("dbo.Appointments", "AdvertismentId");
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
            DropForeignKey("dbo.UserFavoriteHorseAds", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.HorseAds", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RecommendedRiderHorseAds", "HorseAd_Id", "dbo.HorseAds");
            DropForeignKey("dbo.RecommendedRiderHorseAds", "RecommendedRider_Id", "dbo.RecommendedRiders");
            DropForeignKey("dbo.HorseAds", "PriceRangeId", "dbo.PriceRanges");
            DropForeignKey("dbo.HorseAds", "PedigreeId", "dbo.Pedigrees");
            DropForeignKey("dbo.ImageModels", "HorseAdId", "dbo.HorseAds");
            DropForeignKey("dbo.UserFavoriteHorseAds", "HorseAdId", "dbo.HorseAds");
            DropForeignKey("dbo.HorseAds", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.HorseAbilityHorseAds", "HorseAd_Id", "dbo.HorseAds");
            DropForeignKey("dbo.HorseAbilityHorseAds", "HorseAbility_Id", "dbo.HorseAbilities");
            DropIndex("dbo.RecommendedRiderHorseAds", new[] { "HorseAd_Id" });
            DropIndex("dbo.RecommendedRiderHorseAds", new[] { "RecommendedRider_Id" });
            DropIndex("dbo.HorseAbilityHorseAds", new[] { "HorseAd_Id" });
            DropIndex("dbo.HorseAbilityHorseAds", new[] { "HorseAbility_Id" });
            DropIndex("dbo.ImageModels", new[] { "HorseAdId" });
            DropIndex("dbo.HorseAds", new[] { "PriceRangeId" });
            DropIndex("dbo.HorseAds", new[] { "PedigreeId" });
            DropIndex("dbo.HorseAds", new[] { "AddressId" });
            DropIndex("dbo.HorseAds", new[] { "UserId" });
            DropIndex("dbo.UserFavoriteHorseAds", new[] { "HorseAdId" });
            DropIndex("dbo.UserFavoriteHorseAds", new[] { "UserId" });
            DropIndex("dbo.Appointments", new[] { "AdvertismentId" });
            AlterColumn("dbo.Appointments", "AdvertismentId", c => c.String(nullable: false));
            DropColumn("dbo.Appointments", "IsCanceled");
            DropTable("dbo.RecommendedRiderHorseAds");
            DropTable("dbo.HorseAbilityHorseAds");
            DropTable("dbo.Pedigrees");
            DropTable("dbo.ImageModels");
            DropTable("dbo.HorseAds");
            DropTable("dbo.UserFavoriteHorseAds");
            DropTable("dbo.Addresses");
        }
    }
}
