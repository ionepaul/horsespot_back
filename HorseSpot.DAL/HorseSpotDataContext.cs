using HorseSpot.DAL.Entities;
using HorseSpot.DAL.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace HorseSpot.DAL
{
    /// <summary>
    /// Horse Spot Relational Database Context
    /// </summary>
    public class HorseSpotDataContext : IdentityDbContext<UserModel>
    {
        public HorseSpotDataContext() : base("HorseSpotDataContext") { }

        public DbSet<Client> Clients { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Gender> Genders { get; set; }

        public DbSet<HorseAbility> HorseAbilities { get; set; }

        public DbSet<RecommendedRider> RecommendedRiders { get; set; }

        public DbSet<PriceRange> PriceRanges { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Subscriber> Subscribers { get; set; }
    }
}
