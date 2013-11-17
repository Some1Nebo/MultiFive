using System.Web.Mvc;
using NUnit.Framework;

using MultiFive.Web.Controllers;

namespace MultiFive.Web.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public void IndexReturnsNonNullView()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
