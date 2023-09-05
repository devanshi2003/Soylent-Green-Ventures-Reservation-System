using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SVG_Restaurants.Models;

namespace SVG_Restaurants.Models
{
    public partial class SGVContext : DbContext
    {
        public SGVContext()
        {
        }

        public SGVContext(DbContextOptions<SGVContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Banquet> Banquets { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<DiningArea> DiningAreas { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Restaurant> Restaurants { get; set; } = null!;
        public virtual DbSet<RestaurantHour> RestaurantHours { get; set; } = null!;
        public virtual DbSet<RestaurantWorker> RestaurantWorkers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=sgv.chjtb5ba2uqq.us-east-1.rds.amazonaws.com,1433;Initial Catalog=SGV;User ID=admin;Password=password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Banquet>(entity =>
            {
                entity.Property(e => e.BanquetId)
                    .ValueGeneratedNever()
                    .HasColumnName("BanquetID");

                entity.Property(e => e.BanquetName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Banquets)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Banquets__Restau__412EB0B6");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.LoyaltyPoints).HasDefaultValueSql("((0))");

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Username).HasMaxLength(20);
            });

            modelBuilder.Entity<DiningArea>(entity =>
            {
                entity.HasKey(e => e.AreaId)
                    .HasName("PK__DiningAr__70B820280A60E3E9");

                entity.Property(e => e.AreaId)
                    .ValueGeneratedNever()
                    .HasColumnName("AreaID");

                entity.Property(e => e.Area)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.DiningAreas)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__DiningAre__Resta__3E52440B");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("Reservation");

                entity.Property(e => e.ReservationId).HasColumnName("ReservationID");

                entity.Property(e => e.AreaId).HasColumnName("AreaID");

                entity.Property(e => e.BanquetId).HasColumnName("BanquetID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.ReservationTiming).HasColumnType("datetime");

                entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK__Reservati__AreaI__45F365D3");

                entity.HasOne(d => d.Banquet)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.BanquetId)
                    .HasConstraintName("FK__Reservati__Banqu__46E78A0C");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Reservati__Custo__440B1D61");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Reservati__Resta__44FF419A");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.Property(e => e.RestaurantId)
                    .ValueGeneratedNever()
                    .HasColumnName("RestaurantID");

                entity.Property(e => e.RestaurantAddress).HasMaxLength(255);

                entity.Property(e => e.RestaurantName).HasMaxLength(300);
            });

            modelBuilder.Entity<RestaurantHour>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ClosedDateFrom)
                    .HasColumnType("datetime")
                    .HasColumnName("closedDateFrom");

                entity.Property(e => e.ClosedDateTo)
                    .HasColumnType("datetime")
                    .HasColumnName("closedDateTo");

                entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");

                entity.HasOne(d => d.Restaurant)
                    .WithMany()
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Restauran__Resta__48CFD27E");
            });

            modelBuilder.Entity<RestaurantWorker>(entity =>
            {
                entity.HasKey(e => e.WorkerId)
                    .HasName("PK__Restaura__077C880627394D28");

                entity.Property(e => e.WorkerId).HasColumnName("WorkerID");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");

                entity.Property(e => e.Username).HasMaxLength(20);

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.RestaurantWorkers)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK__Restauran__Resta__3B75D760");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
