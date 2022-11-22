using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SSC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class BaseApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment("Testing")
                .ConfigureServices(services => //also (as of ASP NET Core 3.0) runs after TStartup.ConfigureServices
            {
                // remove DbcontextOptions from original API project
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                services.Remove(descriptor);

                // add the test database context instead
                services.AddDbContext<DataContext>(options => options.UseMySql("datasource=localhost;port=3307;username=root;password=;database=CovidTestDB;", new MySqlServerVersion(new Version(8, 0, 22))));

                });
        }
    }
}
