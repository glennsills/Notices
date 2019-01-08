using System;
using Xunit;
using Notices.TestNotifiers;
using Notices.DCNotifier;
using Notices.NoticeServiceImplementation;
using Notices.NoticeService;
using System.Threading.Tasks;
using Notices.NoticeData;

namespace Notices.NoticeTests.UnitTests
{
    public class OtherNotifierTests
    {
        private readonly ITestNotifier _testNotifier;

        public OtherNotifierTests()
        {
            _testNotifier = new TestNotifier(null, null);
        }

        [Fact]
        public async Task SelectTestNotifierByName()
        {
            var cut = new NotifyService(null, _testNotifier);
            var expected = $"Notify(recipientId, {nameof(Mandate.TestNotifications)}, purpose)";
            var actual = await cut.Notify("recipientId", Mandate.TestNotifications,"purpose");
            Assert.False(actual.WasSuccessful);
            Assert.Equal(expected, actual.ProcessMessage, true, true);
        }
    }
}
