using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Data;
using Tidbeat.Models;

namespace TidbeatTests2._0 {
    public class ApplicationDbContextFixture : IDisposable {
        private readonly string _connectionString = "DataSource=:memory:";
        private readonly DbContextOptions<ApplicationDbContext> _options;
        public ApplicationDbContext ApplicationDbContext { get; private set; }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        public ApplicationDbContextFixture() {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            ApplicationDbContext = new ApplicationDbContext(_options);
            ApplicationDbContext.Database.EnsureCreated();

            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext), null, null, null, null, null, null, null, null);
        }

        public void Dispose() {
            ApplicationDbContext.Database.EnsureDeleted();
            ApplicationDbContext.Dispose();
        }
    }

}
