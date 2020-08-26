#pragma once

#include "exports.h"
#include <string>

enum class MessageBoxResult
{
    None = 0,
    OK = 1,
    Cancel = 2,
    Yes = 6,
    No = 7,
};

enum class MessageBoxButton
{
    OK = 0,
    OKCancel = 1,
    YesNo = 4,
    YesNoCancel = 3,
};

class INFRASTRUCTURE_API MessageBoxService
{
public:
    static void ErrorDialog(std::string message);
    static void InfoDialog(std::string message);
    static void WarningDialog(std::string message);
    static MessageBoxResult Question(std::string message, MessageBoxButton buttons);
};

