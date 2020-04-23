﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using online_avalon_web.Core.Enums;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core
{
    public class AvalonContext : DbContext
    {
        public AvalonContext(DbContextOptions<AvalonContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Quest> Quests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.GameId);
                entity.Property(e => e.GameId)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.GameResult)
                    .HasConversion(new EnumToStringConverter<GameResultEnum>());
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.PlayerId);
                entity.Property(e => e.PlayerId)
                    .ValueGeneratedOnAdd();
                entity.HasOne(e => e.Game)
                    .WithMany(e => e.Players)
                    .HasForeignKey(e => e.GameId);
                entity.Property(e => e.Role)
                    .HasConversion(new EnumToStringConverter<RoleEnum>());

                entity.HasQueryFilter(e => !e.Disconnected);
            });

            modelBuilder.Entity<Quest>(entity =>
            {
                entity.HasKey(e => e.QuestId);
                entity.Property(e => e.QuestId)
                    .ValueGeneratedOnAdd();
                entity.HasOne(e => e.Game)
                    .WithMany(e => e.Quests)
                    .HasForeignKey(e => e.GameId);
                entity.Property(e => e.QuestResult)
                    .HasConversion(new EnumToStringConverter<QuestResultEnum>());
            });
        }
    }
}
