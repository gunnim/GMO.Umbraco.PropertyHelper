using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GMO.Umbraco;
using Moq;
using Umbraco.Core.Models;
using Our.Umbraco.Vorto.Models;

namespace PropertyHelperTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void IsVortoPropertyWorks()
        {
            var vortoVal = new VortoValue();
            var propHelper = new PrivateType(typeof(PropertyHelper));

            var prop = new Mock<IPublishedProperty>();
            prop.Setup(x => x.Value).Returns(vortoVal);
            prop.Setup(x => x.HasValue).Returns(true);

            var content = new Mock<IPublishedContent>();
            content.Setup(
                x => x.GetProperty(
                    It.Is<string>(y => y == "propAlias"), 
                    It.IsAny<bool>()))
                .Returns(prop.Object);
            content.Setup(
                x => x.GetProperty(
                    It.Is<string>(y => y == "propAlias")))
                .Returns(prop.Object);

            var result = (bool) propHelper.InvokeStatic(
                "IsVortoProperty", 
                new object[] { content.Object, "propAlias" }
            );

            Assert.IsTrue(result);
        }
    }
}
