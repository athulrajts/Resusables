using System;

namespace KEI.Infrastructure
{
    public interface IViewService
    {
        /// <summary>
        /// Instructs the UI to be in a busy/unavailable state
        /// </summary>
        void SetBusy(params string[] msg);

        /// <summary>
        /// Instructs the UI to be in an available state
        /// </summary>
        void SetAvailable();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void UpdateBusyText(params string[] msg);

        /// <summary>
        /// Returns whether or not the UI is currently in a busy/unavailable state
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Alerts the user of an error
        /// </summary>
        /// <param name="strAlert">Error description</param>
        void Error(string alert, bool isModal = false);

        /// <summary>
        /// Asks the user to confirm an action
        /// </summary>
        /// <param name="confirmMsg">The action to confirm</param>
        /// <param name="dialogButtons">Buttons for confirmation</param>
        /// <returns>True if the user confirms the action, false otherwise</returns>
        PromptResult Prompt(string confirmMsg, PromptOptions dialogButtons);

        PromptResult PromptWithDefault(string message, PromptOptions buttons, PromptResult defaultResult, TimeSpan timeout);

        /// <summary>
        /// Display a warning to the user
        /// </summary>
        /// <param name="warning">The warning to display</param>
        void Warn(string warning, bool isModal = false);

        /// <summary>
        /// Display an informational message to the user
        /// </summary>
        /// <param name="info">The info to display</param>
        void Inform(string info, bool isModal = false);

        /// <summary>
        /// Show dialog to Switch user
        /// </summary>
        void SwitchUser();

        /// <summary>
        /// Opens up Object editor
        /// </summary>
        /// <param name="o">object to edit</param>
        void EditObject(object o);

        /// <summary>
        /// Opens a file dialog to choose a file
        /// </summary>
        /// <param name="description">description of extension</param>
        /// <param name="filters">extension</param>
        /// <returns></returns>
        string BrowseFile(string description = "", string filters = "");

        string BrowseFolder();

        void SaveFile(Action<string> saveAction, string filters = "");

    }

    public enum PromptOptions
    {
        Ok = 0,
        OkCancel = 1,
        YesNo = 2,
        OkAbort = 3,
        IgnoreRetry = 4
    }

    public enum PromptResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Abort = 3,
        Retry = 4,
        Ignore = 5,
        Yes = 6,
        No = 7
    }

}
