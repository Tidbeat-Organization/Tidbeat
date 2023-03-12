using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Data;

namespace TidbeatTests {
    public class ApplicationDbContextFixture {

        public ApplicationDbContext ApplicationDbContext { get; set; }

        public ApplicationDbContextFixture() {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;
            ApplicationDbContext = new ApplicationDbContext(options);

            ApplicationDbContext.Database.EnsureCreated();
        }
    }
}
