﻿// <auto-generated />
using System;
using CinemaBooking.Web.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CinemaBooking.Web.Migrations
{
    [DbContext(typeof(CinemaDbContext))]
    [Migration("20240429160706_AddManyReservations")]
    partial class AddManyReservations
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Hall", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Halls");
                });

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ScreeningId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SeatId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ScreeningId");

                    b.HasIndex("SeatId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Screening", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("HallId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("HallId");

                    b.ToTable("Screenings");
                });

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Seat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("HallId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsForDisabled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PositionX")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PositionY")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SeatNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("HallId");

                    b.ToTable("Seats");
                });

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Reservation", b =>
                {
                    b.HasOne("CinemaBooking.Web.Db.Entitites.Screening", "Screening")
                        .WithMany("Reservations")
                        .HasForeignKey("ScreeningId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CinemaBooking.Web.Db.Entitites.Seat", "Seat")
                        .WithMany("Reservations")
                        .HasForeignKey("SeatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Screening");

                    b.Navigation("Seat");
                });

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Screening", b =>
                {
                    b.HasOne("CinemaBooking.Web.Db.Entitites.Hall", "Hall")
                        .WithMany("Screenings")
                        .HasForeignKey("HallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hall");
                });

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Seat", b =>
                {
                    b.HasOne("CinemaBooking.Web.Db.Entitites.Hall", "Hall")
                        .WithMany("Seats")
                        .HasForeignKey("HallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hall");
                });

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Hall", b =>
                {
                    b.Navigation("Screenings");

                    b.Navigation("Seats");
                });

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Screening", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("CinemaBooking.Web.Db.Entitites.Seat", b =>
                {
                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}
