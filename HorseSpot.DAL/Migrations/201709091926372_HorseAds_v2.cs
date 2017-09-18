namespace HorseSpot.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HorseAds_v2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Addresses", "AddressId", "dbo.HorseAds");
            DropForeignKey("dbo.Pedigrees", "PedigreeId", "dbo.HorseAds");
            DropForeignKey("dbo.HorseAds", "PriceRange_Id", "dbo.PriceRanges");
            DropIndex("dbo.Addresses", new[] { "AddressId" });
            DropIndex("dbo.HorseAds", new[] { "PriceRange_Id" });
            DropIndex("dbo.Pedigrees", new[] { "PedigreeId" });
            RenameColumn(table: "dbo.HorseAds", name: "PriceRange_Id", newName: "PriceRangeId");
            AddColumn("dbo.HorseAds", "AddressId", c => c.Int(nullable: false));
            AddColumn("dbo.HorseAds", "PedigreeId", c => c.Int(nullable: false));
            AlterColumn("dbo.HorseAds", "PriceRangeId", c => c.Int(nullable: false));
            CreateIndex("dbo.HorseAds", "AddressId");
            CreateIndex("dbo.HorseAds", "PedigreeId");
            CreateIndex("dbo.HorseAds", "PriceRangeId");
            AddForeignKey("dbo.HorseAds", "AddressId", "dbo.Addresses", "AddressId", cascadeDelete: true);
            AddForeignKey("dbo.HorseAds", "PedigreeId", "dbo.Pedigrees", "PedigreeId", cascadeDelete: true);
            AddForeignKey("dbo.HorseAds", "PriceRangeId", "dbo.PriceRanges", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HorseAds", "PriceRangeId", "dbo.PriceRanges");
            DropForeignKey("dbo.HorseAds", "PedigreeId", "dbo.Pedigrees");
            DropForeignKey("dbo.HorseAds", "AddressId", "dbo.Addresses");
            DropIndex("dbo.HorseAds", new[] { "PriceRangeId" });
            DropIndex("dbo.HorseAds", new[] { "PedigreeId" });
            DropIndex("dbo.HorseAds", new[] { "AddressId" });
            AlterColumn("dbo.HorseAds", "PriceRangeId", c => c.Int());
            DropColumn("dbo.HorseAds", "PedigreeId");
            DropColumn("dbo.HorseAds", "AddressId");
            RenameColumn(table: "dbo.HorseAds", name: "PriceRangeId", newName: "PriceRange_Id");
            CreateIndex("dbo.Pedigrees", "PedigreeId");
            CreateIndex("dbo.HorseAds", "PriceRange_Id");
            CreateIndex("dbo.Addresses", "AddressId");
            AddForeignKey("dbo.HorseAds", "PriceRange_Id", "dbo.PriceRanges", "Id");
            AddForeignKey("dbo.Pedigrees", "PedigreeId", "dbo.HorseAds", "Id");
            AddForeignKey("dbo.Addresses", "AddressId", "dbo.HorseAds", "Id");
        }
    }
}
