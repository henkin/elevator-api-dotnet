using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Elevator.WebApi.Tests
{
    public class FloorRequestsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public FloorRequestsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestGetNextFloorToStop_WithOutstandingRequests()
        {
            // Arrange
            int floorNumber = 5;
            await using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            
            await PostFloorRequest(client, floorNumber);

            // Act
            var floorRequests = await GetFloorRequests(client);

            // Assert
            floorRequests.Should().BeEquivalentTo(new int[] { floorNumber });
        }

        // A test for the case where there are no outstanding requests
        [Fact]
        public async Task TestGetNextFloorToStop_WithNoOutstandingRequests()
        {
            await using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();

            // Act
            var floorRequests = await GetFloorRequests(client);

            // Assert
            floorRequests.Should().BeEmpty();
        }

        private static async Task<int[]?> GetFloorRequests(HttpClient client)
        {
            var response = await client.GetAsync("/floor-requests");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var outstandingRequests = JsonSerializer.Deserialize<int[]>(responseBody);
            return outstandingRequests;
        }

        private async Task PostFloorRequest(HttpClient client, int floorNumber)
        {
            await client.PostAsync("/floor-requests",
                new StringContent(floorNumber.ToString(), System.Text.Encoding.Default, "application/json"));
        }
    }
}
