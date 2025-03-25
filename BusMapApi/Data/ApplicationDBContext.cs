using BusMapApi.Model;
using BusMapApi.Model.Entities;
using BusMapApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {}

        public DbSet<BusStop> BusStops { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<DetailChat> DetailChats { get; set; }
        public DbSet<UserAdminChat> UserAdminChat { get; set; }
        public DbSet<UserAdminMessage> UserAdminMessage { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupChatMessage> GroupChatMessages { get; set; }
        public DbSet<FavoriteRoute> FavoriteRoutes { get; set; }
        public DbSet<BusStop> bustop { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình quan hệ
            modelBuilder.Entity<FavoriteRoute>()
                .HasOne(fr => fr.User)
                .WithMany(u => u.FavoriteRoutes)
                .HasForeignKey(fr => fr.UserId);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User)
                .WithMany(u => u.Chats)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<DetailChat>()
                .HasOne(dc => dc.Chat)
                .WithMany(c => c.DetailChats)
                .HasForeignKey(dc => dc.ChatId);

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