using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Data;

namespace TidbeatTests2._0 {
    public class ApplicationDbContextFixture : IDisposable {
        private readonly string _connectionString = "DataSource=:memory:";
        private readonly DbContextOptions<ApplicationDbContext> _options;
        public ApplicationDbContext ApplicationDbContext { get; private set; }

        public ApplicationDbContextFixture() {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            ApplicationDbContext = new ApplicationDbContext(_options);
            ApplicationDbContext.Database.EnsureCreated();
        }

        public void Dispose() {
            ApplicationDbContext.Database.EnsureDeleted();
            ApplicationDbContext.Dispose();
        }
    }

}
