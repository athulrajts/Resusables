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
        /// Updates text shown in busy overlay
        /// </summary>
        /// <param name="msg">array messages, each representing a line</param>
        void UpdateBusyText(params string[] msg);

        /// <summary>
        /// Returns whether or not the UI is currently in a busy/unavailable state
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Alerts the user of an error
        /// </summary>
        /// <param name="message">Error description</param>
        void Error(string message, bool isModal = true);

        /// <summary>
        /// Prompts the user for input
        /// </summary>
        /// <param name="confirmMsg">message shown in the dialog</param>
        /// <param name="buttons">User inputs</param>
        /// <returns>User input</returns>
        PromptResult Prompt(string confirmMsg, PromptOptions buttons);

        /// <summary>
        /// Prompts the user for input, and closes itself with default
        /// result if no input is recieved before <paramref name="timeout"/>
        /// </summary>
        /// <param name="message">message shown in the dialog</param>
        /// <param name="buttons">User inputs</param>
        /// <param name="defaultResult">Default result if user doesn't respond in time</param>
        /// <param name="timeout">After timeout dialog closes with <paramref name="defaultResult"/></param>
        /// <returns>Userinput or <paramref name="defaultResult"/></returns>
        PromptResult PromptWithDefault(string message, PromptOptions buttons, PromptResult defaultResult, TimeSpan timeout);

        /// <summary>
        /// Display a warning to the user
        /// </summary>
        /// <param name="message">The warning to display</param>
        void Warn(string message, bool isModal = true);

        /// <summary>
        /// Display an informational message to the user
        /// </summary>
        /// <param name="message">The info to display</param>
        void Inform(string message, bool isModal = true);

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
        /// Opens a filedialog to choose a file with given filters
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        string BrowseFile(FilterCollection filters = null, string initialDirectory = null);

        /// <summary>
        /// Opens a file dialog to chose a folder.
        /// </summary>
        /// <returns></returns>
        string BrowseFolder(string initialDirectory = null);

        /// <summary>
        /// Opens a file dialog to browse and save a file
        /// </summary>
        /// <param name="saveAction">Action to save file given filename</param>
        /// <param name="filters">A System.String that contains the filter. The default is System.String.Empty,
        /// which means that no filter is applied and all file types are displayed.</param>
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
