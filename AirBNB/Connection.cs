namespace AirBNB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using MySql.Data.MySqlClient;
    public class MyDatabaseContext : DbContext
    {


        public MySqlConnection Connection { get; }

        
        public MyDatabaseContext(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public MyDatabaseContext(DbContextOptions<MyDatabaseContext> options) : base(options) { }
        public DbSet<Models.Users> users { get; set; }
        public DbSet<Models.Camping_spots> camping_spots { get; set; }
        public DbSet<Models.Campingplace> campingplace { get; set; }
        public DbSet<Models.Location> location { get; set; }
        public DbSet<Models.Picture> picture { get; set; }
        public DbSet<Models.Reservations> reservations { get; set; }
        public DbSet<Models.Reviews> reviews { get; set; }
        public DbSet<Models.Types> type  { get; set; }
        public DbSet<Models.Amenities_only> amenities  { get; set; }
        public DbSet<Models.Amenities_camp> amenities_camp { get; set; }
        public DbSet<Models.availability> availability { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasKey(a => a.user_id);
            modelBuilder.Entity<Users>().Property(a => a.user_id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Campingplace>().HasKey(a => a.place_id);
            modelBuilder.Entity<Campingplace>().Property(a => a.place_id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Camping_spots>().HasKey(a => a.spot_id);
            modelBuilder.Entity<Camping_spots>().Property(a => a.spot_id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Location>().HasKey(a => a.location_id);
            modelBuilder.Entity<Location>().Property(a => a.location_id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Picture>().HasKey(a => a.pic_id);
            modelBuilder.Entity<Picture>().Property(a => a.pic_id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Reservations>().HasKey(a => a.id);
            modelBuilder.Entity<Reservations>().Property(a => a.id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Reviews>().HasKey(a => a.review_id);
            modelBuilder.Entity<Reviews>().Property(a => a.review_id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Types>().HasKey(a => a.type_id);
            modelBuilder.Entity<Types>().Property(a => a.type_id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Amenities_only>().HasKey(a => a.am_id);
            modelBuilder.Entity<Amenities_only>().Property(a => a.am_id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Models.Amenities_camp>().HasKey(a => a.am_camp_id);
            modelBuilder.Entity<Models.Amenities_camp>().Property(a => a.am_camp_id).ValueGeneratedOnAdd();
            modelBuilder.Entity<availability>().HasKey(a => a.avb_id);
            modelBuilder.Entity<availability>().Property(a => a.avb_id).ValueGeneratedOnAdd();
        }
        public void OpenConnection()
        {
            Connection.Open();
        }

        public void CloseConnection()
        {
            Connection.Close();
        }
    }

}
