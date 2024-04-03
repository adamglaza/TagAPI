using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using static TagAPI.Data.SOAPI;

namespace IntegrationTestTagAPI
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient httpClient;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Arrange
            httpClient = factory.CreateClient();
        }
        [Fact]
        public void FetchTagsFromStackOverflowAPIAndUpdateDatabase()
        {
            // Arrange
            var uri = "http://localhost:8080/Tags?page=1&pagesize=100";

            // Act
            var response = httpClient.GetAsync(uri).Result;

            // Assert
            response.EnsureSuccessStatusCode();
            var tagList = response.Content.ReadFromJsonAsync<List<TagSQL>>().Result;

            Assert.Equal(100, tagList.Count);
        }

        [Fact]
        public void UpdateDatabaseWithTagsFromStackOverflow()
        {
            // Arrange
            var uri = "http://localhost:8080/Tags/RefreshDatabase";

            // Act
            var response = httpClient.PostAsync(uri, new StringContent("")).Result;

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}