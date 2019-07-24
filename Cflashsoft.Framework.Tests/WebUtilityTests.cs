using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cflashsoft.Framework.Web;
using System.Web;
using Moq;

namespace Cflashsoft.Framework.Tests
{
    [TestClass]
    public class WebUtilityTests
    {
        [TestMethod]
        public void Epoch_IsAccurate()
        {
            Assert.IsTrue(WebUtility.EPOCH == new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [TestMethod]
        public void ConvertFromUnixTime_IsAccurate()
        {
            Assert.IsTrue(WebUtility.ConvertFromUnixTime(344525425).ToUniversalTime() == new DateTime(1980, 12, 1, 13, 30, 25, DateTimeKind.Utc));
        }

        [TestMethod]
        public void ConvertToUnixTime_IsAccurate()
        {
            Assert.IsTrue(WebUtility.ConvertToUnixTime(new DateTime(1980, 12, 1, 13, 30, 25, DateTimeKind.Utc)) == 344525425);
        }

        [TestMethod]
        public void ConvertToClientTime_IsAccurate()
        {
            HttpRequest request = new HttpRequest("foo", "http://foo", null);

            request.Cookies.Add(new HttpCookie("timezoneOffset", "-300")); //-5 hours from UTC

            HttpContext context = new HttpContext(request, new HttpResponse(null));
            
            Assert.IsTrue(WebUtility.ConvertToClientTime(new HttpContextWrapper(context), new DateTime(1980, 12, 1, 13, 30, 25, DateTimeKind.Utc)) == new DateTime(1980, 12, 1, 8, 30, 25, DateTimeKind.Utc));
        }

        [TestMethod]
        public void ConvertToClientTime_IsAccurate2()
        {
            Assert.IsTrue(WebUtility.ConvertToClientTime(new DateTime(1980, 12, 1, 13, 30, 25, DateTimeKind.Utc), -300) == new DateTime(1980, 12, 1, 8, 30, 25, DateTimeKind.Utc));
        }

        public void SendEmail()
        {
            using (ShimsContext.Create())
            {
                // Arrange:  
                // Shim DateTime.Now to return a fixed date:  
                System.Fakes.ShimDateTime.NowGet =
                () =>
                { return new DateTime(fixedYear, 1, 1); };

                // Instantiate the component under test:  
                var componentUnderTest = new MyComponent();

                // Act:  
                int year = componentUnderTest.GetTheCurrentYear();

                // Assert:   
                // This will always be true if the component is working:  
                Assert.AreEqual(fixedYear, year);
            }

        }

    }
}
