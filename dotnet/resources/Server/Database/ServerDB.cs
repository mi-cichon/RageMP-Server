using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Server.Models;
using Newtonsoft.Json;
using System.IO;

namespace Server.Database
{
    public class ServerDB : DbContext
    {
        public DBSettings Settings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<PenaltyLog> PenaltyLogs { get; set; }
        public DbSet<Penalty> Penalties { get; set; }
        public DbSet<Org> Orgs { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<LSPD_Vehicle> LSPD_Vehicles { get; set; }
        public DbSet<LSPD_Member> LSPD_Members { get; set; }
        public DbSet<Licences> Licences { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Discord> Discords { get; set; }
        public DbSet<Data> Data { get; set; }
        public DbSet<CarMarket> CarMarket {get; set;}
        public DbSet<Business_Tune> Business_Tunes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Settings = JsonConvert.DeserializeObject<DBSettings>(File.ReadAllText("logs/config.json"));
            optionsBuilder.UseMySql($"server={Settings.Server};database={Settings.Database};user={Settings.User};password={Settings.Password}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Transfer>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<PenaltyLog>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Penalty>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Org>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<LSPD_Vehicle>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<LSPD_Member>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Licences>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<House>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Discord>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Data>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<CarMarket>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Business_Tune>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });
        }
    }

    public class DBSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
