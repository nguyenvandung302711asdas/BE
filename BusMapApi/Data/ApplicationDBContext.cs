using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusMapApi.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }

        public DbSet<BusStop> BusStop { get; set; }
        public DbSet<CustomerAccount> CustomerAccount { get; set; }
        public DbSet<BusRoute> BusRoute { get; set; }
        public DbSet<DetailBusRoute> DetailBusRoute { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<ForgotPassword> ForgotPassword { get; set; }
        public DbSet<Admin> Admin { get; set; }

        public DbSet<Bus> Bus { get; set; }






    }
}