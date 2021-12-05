﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PlanterApi;

#nullable disable

namespace PlanterApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20211205170425_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PlanterApi.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastReceivedTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("PlanterApi.DeviceMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AirHumidityPercentage")
                        .HasColumnType("integer");

                    b.Property<int>("AirTemperatureCelsius")
                        .HasColumnType("integer");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uuid");

                    b.Property<bool>("LightSourceOn")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ProcessedTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("SoilMoisturePercentage")
                        .HasColumnType("integer");

                    b.Property<int>("SoilTemperatureCelsius")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("WaterPumpOn")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("DeviceMessages");
                });

            modelBuilder.Entity("PlanterApi.DeviceMessage", b =>
                {
                    b.HasOne("PlanterApi.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });
#pragma warning restore 612, 618
        }
    }
}
