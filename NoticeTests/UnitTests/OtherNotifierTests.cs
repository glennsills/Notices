using System;
using Xunit;
using Notices.TestNotifiers;
using Notices.DCNotifier;
using Notices.NoticeServiceImplementation;
using Notices.NoticeService;
using System.Threading.Tasks;
using Notices.NoticeData;
using Moq;

namespace Notices.NoticeTests.UnitTests
{
    public class OtherNotifierTests
    {

        [Fact]
        public async Task SelectTestNotifierByName()
        {
            var expected = new NoticeRecord
            {
                WasSuccessful = true,
                ProcessMessage = "The correct processor was selected"
            };

            var notExpected = new NoticeRecord
            {
                WasSuccessful = true,
                ProcessMessage = "The wrong processor was selected"
            };

            var testNotifier = new Mock<ITestNotifier>();
            var dcNotifier = new Mock<IDCWageNotifier>();

            testNotifier.Setup(x => x.Notify(It.IsAny<string>(), Mandate.TestNotifications, "Test")).ReturnsAsync(expected);
            dcNotifier.Setup(x => x.Notify(It.IsAny<string>(), Mandate.TestNotifications, "Test")).ReturnsAsync(notExpected);           

            var cut = new NotifyService(dcNotifier.Object, testNotifier.Object);
            var actual = await cut.Notify("recipientId", Mandate.TestNotifications,"Test");
            Assert.Equal(expected.ProcessMessage, actual.ProcessMessage, true, true);
        }
    }
}
