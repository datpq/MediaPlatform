using ITF.MediaPlatform.API.Controllers;
using Ninject;
using Xunit;

namespace ITF.MediaPlatform.API.Tests
{
    [Collection("IisExpressTest")]
    public abstract class BaseTest
    {
        protected readonly HomeController HomeController;
        protected readonly NationController NationController;
        protected readonly EventController EventController;
        protected readonly CircuitPlayerController CircuitPlayerController;
        protected readonly BtdController BtdController;
        protected readonly CmsController CmsController;
        protected readonly CircuitController CircuitController;

        protected BaseTest(IisExpressFixture iisExpressFixture)
        {
            HomeController = iisExpressFixture.Kernel.Get<HomeController>();
            NationController = iisExpressFixture.Kernel.Get<NationController>();
            EventController = iisExpressFixture.Kernel.Get<EventController>();
            CircuitPlayerController = iisExpressFixture.Kernel.Get<CircuitPlayerController>();
            BtdController = iisExpressFixture.Kernel.Get<BtdController>();
            CmsController = iisExpressFixture.Kernel.Get<CmsController>();
            CircuitController = iisExpressFixture.Kernel.Get<CircuitController>();
        }
    }
}
