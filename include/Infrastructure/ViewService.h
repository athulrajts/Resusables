#pragma once
#include "exports.h"
#include <string>

class ViewServiceNativeWrapper;

enum class PromptOptions
{
    Ok = 0,
    OkCancel = 1,
    YesNo = 2,
    OkAbort = 3,
    IgnoreRetry = 4
};

enum class PromptResult
{
    None = 0,
    OK = 1,
    Cancel = 2,
    Abort = 3,
    Retry = 4,
    Ignore = 5,
    Yes = 6,
    No = 7
};

class INFRASTRUCTURE_API ViewService
{
public:

	static void AutoInitialize();

	static void WarningDialog(std::string message, bool is_modal = true);
	static void InformationDialog(std::string message, bool is_modal = true);
	static void ErrorDialog(std::string message, bool is_modal = true);
    static PromptResult PromptDialog(std::string message, PromptOptions option);
};