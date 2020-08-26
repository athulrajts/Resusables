#pragma once

#include <msclr/gcroot.h>
#include <string>

#include "ViewService.h"

class ViewServiceNativeWrapper
{
public:

	static void AutoInitialize();

	static void WarningDialog(std::string message, bool is_modal = true);
	static void InformationDialog(std::string message, bool is_modal = true);
	static void ErrorDialog(std::string message, bool is_modal = true);
	static PromptResult PromptDialog(std::string message, PromptOptions option);

private:
	static msclr::gcroot<KEI::Infrastructure::IViewService^> instance;
};