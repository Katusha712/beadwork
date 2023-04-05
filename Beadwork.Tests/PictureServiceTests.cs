using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Beadwork.Tests
{
    public class PictureServiceTests
    {
        [Fact]
        public void GetAllByQuery_WithItemNumber_CallsGetAllByItemNumber()
        {
            var pictureRepositoryStub = new Mock<IPictureRepository>();
            pictureRepositoryStub.Setup(x => x.GetAllByItemNumber(It.IsAny<string>()))
                                 .Returns(new[] { new Picture(1, "", "", "") });

            pictureRepositoryStub.Setup(x => x.GetAllByTitleOrAuthor(It.IsAny<string>()))
                         .Returns(new[] { new Picture(2, "", "", "") });

            var pictureService = new PictureService(pictureRepositoryStub.Object);
            var validItemNumber = "IN-45645672";

            var actual = pictureService.GetAllByQuery(validItemNumber);

            Assert.Collection(actual, picture => Assert.Equal(1, picture.Id));
        }

        [Fact]
        public void GetAllByQuery_WithAuthor_CallsGetAllByTitleOrAuthor()
        {
            var pictureRepositoryStub = new Mock<IPictureRepository>();
            pictureRepositoryStub.Setup(x => x.GetAllByItemNumber(It.IsAny<string>()))
                                 .Returns(new[] { new Picture(1, "", "", "") });

            pictureRepositoryStub.Setup(x => x.GetAllByTitleOrAuthor(It.IsAny<string>()))
                         .Returns(new[] { new Picture(2, "", "", "") });

            var pictureService = new PictureService(pictureRepositoryStub.Object);
            var invalidItemNumber = "IN-4564562";

            var actual = pictureService.GetAllByQuery(invalidItemNumber);

            Assert.Collection(actual, picture => Assert.Equal(2, picture.Id));
        }

    }
}
