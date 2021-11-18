﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dbot.Data;

namespace dbot.Migrations
{
    [DbContext(typeof(MovieBotContext))]
    [Migration("20211103182951_UserTablePrincipalKey")]
    partial class UserTablePrincipalKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.11");

            modelBuilder.Entity("dbot.Models.Nomination", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImdbId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserKey")
                        .HasColumnType("TEXT");

                    b.Property<int>("VotingID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Year")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("UserKey");

                    b.ToTable("WeeklyNominations");
                });

            modelBuilder.Entity("dbot.Models.Session", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("NominatedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("VoteOpen")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WinningIMDBId")
                        .HasColumnType("TEXT");

                    b.Property<string>("WinningName")
                        .HasColumnType("TEXT");

                    b.Property<string>("WinningYear")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("dbot.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Discriminator")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("dbot.Models.Vote", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("NominationID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserKey")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("NominationID");

                    b.HasIndex("UserKey");

                    b.ToTable("WeeklyVotes");
                });

            modelBuilder.Entity("dbot.Models.Nomination", b =>
                {
                    b.HasOne("dbot.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserKey")
                        .HasPrincipalKey("Key");

                    b.Navigation("User");
                });

            modelBuilder.Entity("dbot.Models.Vote", b =>
                {
                    b.HasOne("dbot.Models.Nomination", "Nomination")
                        .WithMany()
                        .HasForeignKey("NominationID");

                    b.HasOne("dbot.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserKey")
                        .HasPrincipalKey("Key");

                    b.Navigation("Nomination");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}