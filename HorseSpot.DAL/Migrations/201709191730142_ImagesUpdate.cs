namespace HorseSpot.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImagesUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ImageHorseAds", "Image_ImageId", "dbo.Images");
            DropForeignKey("dbo.ImageHorseAds", "HorseAd_Id", "dbo.HorseAds");
            DropIndex("dbo.ImageHorseAds", new[] { "Image_ImageId" });
            DropIndex("dbo.ImageHorseAds", new[] { "HorseAd_Id" });
            AddColumn("dbo.Images", "Name", c => c.String());
            AddColumn("dbo.Images", "IsProfilePic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Images", "HorseAdId", c => c.Int(nullable: false));
            CreateIndex("dbo.Images", "HorseAdId");
            AddForeignKey("dbo.Images", "HorseAdId", "dbo.HorseAds", "Id", cascadeDelete: true);
            DropColumn("dbo.Images", "Path");
            DropTable("dbo.ImageHorseAds");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ImageHorseAds",
                c => new
                    {
                        Image_ImageId = c.Int(nullable: false),
                        HorseAd_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Image_ImageId, t.HorseAd_Id });
            
            AddColumn("dbo.Images", "Path", c => c.String());
            DropForeignKey("dbo.Images", "HorseAdId", "dbo.HorseAds");
            DropIndex("dbo.Images", new[] { "HorseAdId" });
            DropColumn("dbo.Images", "HorseAdId");
            DropColumn("dbo.Images", "IsProfilePic");
            DropColumn("dbo.Images", "Name");
            CreateIndex("dbo.ImageHorseAds", "HorseAd_Id");
            CreateIndex("dbo.ImageHorseAds", "Image_ImageId");
            AddForeignKey("dbo.ImageHorseAds", "HorseAd_Id", "dbo.HorseAds", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ImageHorseAds", "Image_ImageId", "dbo.Images", "ImageId", cascadeDelete: true);
        }
    }
}
