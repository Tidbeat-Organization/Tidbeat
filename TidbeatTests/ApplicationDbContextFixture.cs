using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Data;
using Tidbeat.Models;

namespace TidbeatTests {
    public class ApplicationDbContextFixture : IDisposable {

        public ApplicationDbContext ApplicationDbContext { get; set; }

        public ApplicationDbContextFixture() {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection).EnableSensitiveDataLogging()
                    .Options;
            ApplicationDbContext = new ApplicationDbContext(options);

            ApplicationDbContext.Database.EnsureCreated();
        }

        public void Dispose() {
            ApplicationDbContext.Database.EnsureDeleted();
            ApplicationDbContext.Dispose();
        }
    }
}
