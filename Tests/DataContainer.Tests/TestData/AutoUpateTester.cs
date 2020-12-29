using KEI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContainer.Tests.TestData
{
    class AutoUpateTester
    {
        public DataContainerAutoUpdater AutoUpdater { get; set; }

        public bool UpdateStartedInvoked { get; set; }
        public bool UpdateFinishedInvoked { get; set; }

        public AutoUpateTester(IDataContainer dc)
        {
            AutoUpdater = dc.GetAutoUpdater();

            AutoUpdater.UpdateStarted += AutoUpdater_UpdateStarted;
            AutoUpdater.UpdateFinished += AutoUpdater_UpdateFinished;
        }

        public void Reset()
        {
            UpdateStartedInvoked = UpdateFinishedInvoked = false;
        }

        ~AutoUpateTester()
        {
            AutoUpdater.UpdateStarted -= AutoUpdater_UpdateStarted;
            AutoUpdater.UpdateFinished -= AutoUpdater_UpdateFinished;
        }

        private void AutoUpdater_UpdateFinished() => UpdateFinishedInvoked = true;

        private void AutoUpdater_UpdateStarted() => UpdateStartedInvoked = true;
    }
}
