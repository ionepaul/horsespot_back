namespace HorseSpot.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
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
                "dbo.Appointments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(nullable: false),
                        AppointmentDateTime = c.DateTime(nullable: false),
                        AdvertismentId = c.Int(nullable: false),
                        AdvertismentOwnerId = c.String(nullable: false, maxLength: 128),
                        InitiatorId = c.String(nullable: false, maxLength: 128),
                        IsAccepted = c.Boolean(nullable: false),
                        Title = c.String(nullable: false),
                        IsDatePassed = c.Boolean(nullable: false),
                        SeenByOwner = c.Boolean(nullable: false),
                        SeenByInitiator = c.Boolean(nullable: false),
                        IsCanceled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AdvertismentOwnerId, cascadeDelete: false)
                .ForeignKey("dbo.HorseAds", t => t.AdvertismentId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.InitiatorId, cascadeDelete: false)
                .Index(t => t.AdvertismentId)
                .Index(t => t.AdvertismentOwnerId)
                .Index(t => t.InitiatorId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        NewsletterSubscription = c.Boolean(),
                        ImagePath = c.String(),
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
                "dbo.HorseAbilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ability = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.PriceRanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PriceRangeValue = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RecommendedRiders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rider = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.Clients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Secret = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountryName = c.String(nullable: false, maxLength: 50),
                        Capital = c.String(nullable: false, maxLength: 50),
                        TimezoneId = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.Subscribers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Appointments", "InitiatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Appointments", "AdvertismentId", "dbo.HorseAds");
            DropForeignKey("dbo.Appointments", "AdvertismentOwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
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
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.RecommendedRiderHorseAds", new[] { "HorseAd_Id" });
            DropIndex("dbo.RecommendedRiderHorseAds", new[] { "RecommendedRider_Id" });
            DropIndex("dbo.HorseAbilityHorseAds", new[] { "HorseAd_Id" });
            DropIndex("dbo.HorseAbilityHorseAds", new[] { "HorseAbility_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.ImageModels", new[] { "HorseAdId" });
            DropIndex("dbo.HorseAds", new[] { "PriceRangeId" });
            DropIndex("dbo.HorseAds", new[] { "PedigreeId" });
            DropIndex("dbo.HorseAds", new[] { "AddressId" });
            DropIndex("dbo.HorseAds", new[] { "UserId" });
            DropIndex("dbo.UserFavoriteHorseAds", new[] { "HorseAdId" });
            DropIndex("dbo.UserFavoriteHorseAds", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Appointments", new[] { "InitiatorId" });
            DropIndex("dbo.Appointments", new[] { "AdvertismentOwnerId" });
            DropIndex("dbo.Appointments", new[] { "AdvertismentId" });
            DropTable("dbo.RecommendedRiderHorseAds");
            DropTable("dbo.HorseAbilityHorseAds");
            DropTable("dbo.Subscribers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.Countries");
            DropTable("dbo.Clients");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.RecommendedRiders");
            DropTable("dbo.PriceRanges");
            DropTable("dbo.Pedigrees");
            DropTable("dbo.ImageModels");
            DropTable("dbo.HorseAbilities");
            DropTable("dbo.HorseAds");
            DropTable("dbo.UserFavoriteHorseAds");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Appointments");
            DropTable("dbo.Addresses");
        }
    }
}
