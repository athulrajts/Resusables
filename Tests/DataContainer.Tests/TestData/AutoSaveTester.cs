using KEI.Infrastructure;

namespace DataContainer.Tests.TestData
{
    class AutoSaveTester
    {
        public DataContainerAutoSaver AutoSaver { get; set; }

        public AutoSaveTester(IDataContainer dc)
        {
            AutoSaver = dc.GetAutoSaver();

            AutoSaver.SavingStarted += AutoSaver_SavingStarted;
            AutoSaver.SavingFinished += AutoSaver_SavingFinished;
        }

        public bool SavingStartedInvoked { get; set; }

        public bool SavingFinishedInvoked { get; set; }

        public void Reset()
        {
            SavingFinishedInvoked = SavingFinishedInvoked = false;
        }

        ~AutoSaveTester()
        {
            AutoSaver.SavingStarted -= AutoSaver_SavingStarted;
            AutoSaver.SavingFinished -= AutoSaver_SavingFinished;
        }

        private void AutoSaver_SavingFinished() => SavingFinishedInvoked = true;

        private void AutoSaver_SavingStarted() => SavingStartedInvoked = true;
    }
}
