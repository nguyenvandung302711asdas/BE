using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusMapApi.Model;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public DbSet<TravelInfo> TravelInfos { get; set; }
       public DbSet<Account> Accounts { get; set; }
       public DbSet<TripHistory> TripHistories { get; set; }

      


    }
}