using FOODPEDI.API.REST.DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.DataAccess
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
       

        public string _connectionString;

        public AppDbContext()
        {
            _connectionString = DbHelper.GetConnectionString("AppConnStr");
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            _connectionString = DbHelper.GetConnectionString("AppConnStr");

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }


        public DbSet<Brand> Brands { get; set; }
        public DbSet<MadeCountry> MadeCountries { get; set; }
        public DbSet<MadeState> MadeStates { get; set; }
        public DbSet<MadeCity> MadeCities { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<ItemImage> ItemImages { get; set; }
        public DbSet<ItemComment> ItemComments { get; set; }
        public DbSet<ItemIngredient> ItemIngredients { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }


    }

    public static class DbHelper
    {
        public static string GetConnectionString(string connectionName)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var Configuration = builder.Build();

            var connStr = Configuration.GetConnectionString(connectionName);

            return connStr;
        }



    }
}
