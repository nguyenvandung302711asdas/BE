using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusMapApi.Model;
using BusMapApi.Model.Entities;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Reflection.Emit;
using Azure.Core;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }

        public DbSet<BusStop> BusStop { get; set; }

        public DbSet<BusRoute> BusRoute { get; set; }
        public DbSet<DetailBusRoute> DetailBusRoute { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ForgotPassword> ForgotPassword { get; set; }
        public DbSet<Admin> Admin { get; set; }

        public DbSet<Bus> Bus { get; set; }
        public DbSet<BaiVietQuangCao> Articles { get; set; }
        public DbSet<AnhBaiViet> ArticleImages { get; set; }

        public DbSet<NoiDungBaiViet> ArticleContents { get; set; }
        public DbSet<UserAdminChat> UserAdminChat { get; set; }
        public DbSet<UserAdminMessage> UserAdminMessage { get; set; }
      
        public DbSet<Chat> Chats { get; set; }
        public DbSet<DetailChat> DetailChats { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupChatMessage> GroupChatMessages { get; set; }

        public DbSet<TravelInfo> TravelInfos { get; set; }
        public DbSet<TripHistory> TripHistories { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<GroupChatMessage> GroupChatMessages { get; set; }

        public DbSet<FavoriteRoute> FavoriteRoutes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaiVietQuangCao>().ToTable("BaiVietQuangCao");
            modelBuilder.Entity<AnhBaiViet>().ToTable("AnhBaiViet");
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<Admin>().ToTable("admin");

            modelBuilder.Entity<UserAdminChat>().ToTable("UserAdminChat");
            modelBuilder.Entity<UserAdminMessage>().ToTable("UserAdminMessage");
            modelBuilder.Entity<NoiDungBaiViet>().ToTable("NoiDungBaiViet");
      



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
            // khanh 
            // Cấu hình quan hệ


            modelBuilder.Entity<UserAdminChat>()
                .HasOne(uac => uac.User)
                .WithMany(u => u.UserChats)
                .HasForeignKey(uac => uac.UserId);

            modelBuilder.Entity<UserAdminChat>()
                .HasOne(uac => uac.Admin)
                .WithMany(u => u.AdminChats)
                .HasForeignKey(uac => uac.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserAdminMessage>()
                .HasOne(uam => uam.Chat)
                .WithMany(uac => uac.Messages)
                .HasForeignKey(uam => uam.ChatId);

            modelBuilder.Entity<GroupChatMessage>()
                .HasOne(gcm => gcm.GroupChat)
                .WithMany(gc => gc.Messages)
                .HasForeignKey(gcm => gcm.GroupChatId);

            modelBuilder.Entity<GroupChatMessage>()
                .HasOne(gcm => gcm.Sender)
                .WithMany(u => u.GroupChatMessages)
                .HasForeignKey(gcm => gcm.SenderId);

            // Dữ liệu mẫu
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    FullName = "Nguyen Trung Tin",
                    Email = "tin@gmail.com",
                    NumberPhone = "0987654321",
                    Password = "123456789"
                }
            );
          
            modelBuilder.Entity<GroupChat>().HasData(
                new GroupChat
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Nhóm Chat Chung",
                    CreatedAt = DateTime.UtcNow,
                    IsPublic = true
                }
            );


        }





    }
}