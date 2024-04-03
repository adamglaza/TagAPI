using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using TagAPI.Controllers;
using TagAPI.Data;
using static TagAPI.Data.SOAPI;

namespace UnitTestTagAPI
{
    public class TagsControllerTest
    {
        private readonly Mock<ApplicationDbContext> mockDbContext;
        private readonly TagsController tagsController;

        public TagsControllerTest()
        {
            //Arrange
            var logger = new Mock<ILogger<TagsController>>();
            mockDbContext = new Mock<ApplicationDbContext>();

            var mockDbSet = data.BuildMock().BuildMockDbSet();
            mockDbContext.Setup(x => x.Tags).Returns(mockDbSet.Object);
            tagsController = new TagsController(logger.Object, mockDbContext.Object);
            tagsController.dbReady = true;
        }

        private List<TagSQL> data = new List<TagSQL>
        {
            new TagSQL {Id = 1, Name = "tag1" , Count = 15f},
            new TagSQL {Id = 2, Name = "tag2" , Count = 13.5f},
            new TagSQL {Id = 3, Name = "tag3" , Count = 2f},
            new TagSQL {Id = 4, Name = "tag4" , Count = 1.2f},
        };

        [Fact]
        public void Test_GetTagByCountAscending()
        {
            //Act
            var result = tagsController.Get("count", "", 2, 1);

            // Assert
            Assert.Equal(new List<TagSQL> { data[2] }, result.Value);
        }
        [Fact]
        public void Test_GetTagByCountDescending()
        {
            //Act
            var result = tagsController.Get("count", "desc", 2, 1);

            // Assert
            Assert.Equal(new List<TagSQL> { data[1] }, result.Value);
        }

        [Fact]
        public void Test_GetTagByNameAscending()
        {
            //Act
            var result = tagsController.Get("", "", 2, 1);

            // Assert
            Assert.Equal(new List<TagSQL> { data[1] }, result.Value);
        }
        [Fact]
        public void Test_GetTagByNameDescending()
        {
            //Act
            var result = tagsController.Get("", "desc", 2, 1);

            // Assert
            Assert.Equal(new List<TagSQL> { data[2] }, result.Value);
        }
    }
}