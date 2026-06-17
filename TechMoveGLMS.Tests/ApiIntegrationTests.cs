using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using TechMoveGLMS.API.Data;
using TechMoveGLMS.API.Models;

namespace TechMoveGLMS.Tests
{
    public class ApiIntegrationTests : IClassFixture<ApiIntegrationTests.ApiFactory>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetClients_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/clients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_WithFilters_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/contracts?status=Draft");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostContract_ReturnsCreated()
        {
            var contract = new Contract
            {
                ClientId = 1,
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddMonths(12),
                Status = "Draft",
                ServiceLevel = "Standard"
            };

            var response = await _client.PostAsJsonAsync("/api/contracts", contract);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PatchContractStatus_ReturnsNoContent()
        {
            var response = await _client.PatchAsJsonAsync("/api/contracts/1/status", new { status = "Active" });

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        public class ApiFactory : WebApplicationFactory<TechMoveGLMS.API.ApiAssemblyMarker>
        {
            private readonly InMemoryDatabaseRoot _databaseRoot = new();
            private readonly string _databaseName = $"TechMoveGLMS.ApiTests.{Guid.NewGuid()}";

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureLogging(logging => logging.ClearProviders());
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(_databaseName, _databaseRoot));

                    var provider = services.BuildServiceProvider();
                    using var scope = provider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    context.Database.EnsureCreated();
                    context.Clients.Add(new Client
                    {
                        Id = 1,
                        Name = "Integration Client",
                        ContactEmail = "integration@example.com",
                        ContactPhone = "0123456789",
                        Region = "Gauteng"
                    });
                    context.Contracts.Add(new Contract
                    {
                        Id = 1,
                        ClientId = 1,
                        StartDate = DateTime.UtcNow.Date,
                        EndDate = DateTime.UtcNow.Date.AddMonths(12),
                        Status = "Draft",
                        ServiceLevel = "Standard"
                    });
                    context.SaveChanges();
                });
            }
        }
    }
}
