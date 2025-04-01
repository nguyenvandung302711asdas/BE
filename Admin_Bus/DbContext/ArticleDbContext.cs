    using Microsoft.EntityFrameworkCore;
    using Admin_Bus.Models;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Azure.Core;

    namespace Admin_Bus.Data
    {
        public class ArticleDbContext : DbContext
        {
            public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options) { }

            public DbSet<BaiVietQuangCao> Articles { get; set; }
            public DbSet<AnhBaiViet> ArticleImages { get; set; }
            public DbSet<Account> Accounts { get; set; }
            public DbSet<TripHistory> TripHistory { get; set; }
            public DbSet<NoiDungBaiViet> ArticleContents { get; set; }
            public DbSet<UserAdminChat> UserAdminChat { get; set; }
            public DbSet<UserAdminMessage> UserAdminMessage { get; set; }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<BaiVietQuangCao>().ToTable("BaiVietQuangCao");
                modelBuilder.Entity<AnhBaiViet>().ToTable("AnhBaiViet");
                modelBuilder.Entity<Account>().ToTable("Account");

            modelBuilder.Entity<UserAdminChat>().ToTable("UserAdminChat");
            modelBuilder.Entity<UserAdminMessage>().ToTable("UserAdminMessage");
            modelBuilder.Entity<NoiDungBaiViet>().ToTable("NoiDungBaiViet");
                modelBuilder.Entity<TripHistory>()
                    .HasOne(th => th.account)
                    .WithMany(ca => ca.TripHistories)
                    .HasForeignKey(th => th.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
                base.OnModelCreating(modelBuilder);
          
           

                // Thiết lập quan hệ 1 - N giữa BaiVietQuangCao và AnhBaiViet
                modelBuilder.Entity<AnhBaiViet>()
                    .HasOne(a => a.BaiVietQuangCao)
                    .WithMany(b => b.AnhBaiViets)
                    .HasForeignKey(a => a.ID_BaiViet)
                    .OnDelete(DeleteBehavior.Cascade);
                modelBuilder.Entity<NoiDungBaiViet>()
        .HasOne(nd => nd.BaiVietQuangCao)
        .WithMany(b => b.NoiDungBaiViets)
        .HasForeignKey(nd => nd.ID_BaiViet)
        .OnDelete(DeleteBehavior.Cascade);
                modelBuilder.Entity<NoiDungBaiViet>()
              .HasOne(nd => nd.BaiVietQuangCao)
              .WithMany(b => b.NoiDungBaiViets)
              .HasForeignKey(nd => nd.ID_BaiViet)
              .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ UserAdminChat
            modelBuilder.Entity<UserAdminChat>()
                  .HasOne(uac => uac.User)
                  .WithMany(u => u.UserChats)
                  .HasForeignKey(uac => uac.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserAdminChat>()
                .HasOne(uac => uac.Admin)
                .WithMany(u => u.AdminChats)
                .HasForeignKey(uac => uac.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            // Quan hệ UserAdminMessage với UserAdminChat
            modelBuilder.Entity<UserAdminMessage>()
                .HasOne(uam => uam.Chat)
                .WithMany(uac => uac.Messages)
                .HasForeignKey(uam => uam.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
            }

        }
    }
