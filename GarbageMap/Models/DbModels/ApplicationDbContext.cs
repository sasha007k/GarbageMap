using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GarbageMap.Models.DbModels
{
    public class ApplicationDbContext : IdentityDbContext<IndividualPerson>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
            //Database.Migrate();
        }

        public DbSet<IndividualPerson> IndividualPeople { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CameraPlace> CameraPlaces { get; set; }
        public DbSet<LegalEntity> LegalEntities { get; set; }
        public DbSet<GarbageTruck> GarbageTrucks { get; set; }
        public DbSet<GarbageCanType> GarbageCanTypes { get; set; }
        public DbSet<GarbageCans> GarbageCans { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            CameraPlacesOnModelCreating(builder);
            AddressOnModelCreating(builder);
            CityOnModelCreating(builder);
            DistrictOnModelCreating(builder);
            GarbageCansOnModelCreating(builder);
            base.OnModelCreating(builder);
        }

        private void CameraPlacesOnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CameraPlace>()
                .HasKey(p => p.Id);

            //builder.Entity<CameraPlace>()
            //    .HasOne<Address>()
            //    .WithOne()
            //    .HasForeignKey<CameraPlace>(x => x.AddressId);
        }

        private void AddressOnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Address>()
                .HasKey(p => p.Id);

            //builder.Entity<Address>()
            //    .HasOne<City>()
            //    .WithOne()
            //    .HasForeignKey<Address>(p => p.CityId);
        }

        private void CityOnModelCreating(ModelBuilder builder)
        {
            builder.Entity<City>()
                .HasKey(p => p.Id);

            //builder.Entity<City>()
            //    .HasOne<Region>()
            //    .WithOne()
            //    .HasForeignKey<City>(p => p.RegionId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<City>()
            //    .HasOne<District>()
            //    .WithOne()
            //    .HasForeignKey<City>(p => p.DistrictId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }        
        
        private void DistrictOnModelCreating(ModelBuilder builder)
        {
            builder.Entity<District>()
                .HasKey(p => p.Id);

            //builder.Entity<District>()
            //    .HasOne<Region>()
            //    .WithOne()
            //    .HasForeignKey<District>(p => p.RegionId);
        }

        private void GarbageCansOnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GarbageCans>()
                .HasKey(p => p.Id);

            //builder.Entity<GarbageCans>()
            //    .HasOne<CameraPlace>()
            //    .WithOne()
            //    .HasForeignKey<GarbageCans>(p => p.CameraPlaceId);

            //builder.Entity<GarbageCans>()
            //    .HasOne<GarbageCanType>()
            //    .WithOne()
            //    .HasForeignKey<GarbageCans>(p => p.GarbageCanTypeId);
        }
    }
}
