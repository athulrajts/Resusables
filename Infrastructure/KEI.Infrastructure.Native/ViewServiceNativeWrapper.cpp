#pragma once

#include "pch.h"
#include "ViewServiceNativeWrapper.h"

using namespace CommonServiceLocator;
using namespace System;

#pragma warning(push)
#pragma warning(disable : 4691)

#ifdef Error
#undef Error
#endif // Error

msclr::gcroot<KEI::Infrastructure::IViewService^> ViewServiceNativeWrapper::instance = nullptr;

void ViewServiceNativeWrapper::AutoInitialize()
{
	instance = ServiceLocator::Current->GetInstance<KEI::Infrastructure::IViewService^>();
}

void ViewServiceNativeWrapper::ErrorDialog(std::string message, bool is_modal)
{
	instance->Error(gcnew String(message.c_str()), is_modal);
}

void ViewServiceNativeWrapper::WarningDialog(std::string message, bool is_modal)
{
	instance->Warn(gcnew String(message.c_str()), is_modal);
}

void ViewServiceNativeWrapper::InformationDialog(std::string message, bool is_modal)
{
	instance->Inform(gcnew String(message.c_str()), is_modal);
}

PromptResult ViewServiceNativeWrapper::PromptDialog(std::string message, PromptOptions option)
{
	return (PromptResult)instance->Prompt(gcnew String(message.c_str()), (KEI::Infrastructure::PromptOptions)option);
}

#pragma warning(pop)