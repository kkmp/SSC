
using Microsoft.EntityFrameworkCore;
using SSC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    //public class TestDatabaseFixture
    //{
    //    private SqliteConnection _connection;
    //    private DbContextOptions<DataContext> _contextOptions;

    //    public TestDatabaseFixture()
    //    {
    //        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
    //        // at the end of the test (see Dispose below).
    //        _connection = new SqliteConnection("Filename=:memory:");
    //        _connection.Open();

    //        // These options will be used by the context instances in this test suite, including the connection opened above.
    //        _contextOptions = new DbContextOptionsBuilder<DataContext>()
    //            .UseSqlite(_connection)
    //            .Options;

    //        // Create the schema and seed some data
    //        using var context = new DataContext(_contextOptions);

    //        /*context.AddRange(
    //            new Blog { Name = "Blog1", Url = "http://blog1.com" },
    //            new Blog { Name = "Blog2", Url = "http://blog2.com" });*/
    //        context.SaveChanges();
    //    }

    //    public DataContext CreateContext() => new DataContext(_contextOptions);
    //}
}
