#pragma once

#include "pch.h"
#include "ViewServiceNativeWrapper.h"
#include "Utils.h"

using namespace CommonServiceLocator;
using namespace System;

#pragma warning(push)
#pragma warning(disable : 4691)


msclr::gcroot<KEI::Infrastructure::IViewService^> ViewServiceNativeWrapper::instance = nullptr;

void ViewServiceNativeWrapper::AutoInitialize()
{
	instance = ServiceLocator::Current->GetInstance<KEI::Infrastructure::IViewService^>();
}

void ViewServiceNativeWrapper::InitializeBaseViewService()
{
	instance = gcnew KEI::UI::Wpf::ViewService::BaseViewService();
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

std::string ViewServiceNativeWrapper::BrowseFile(std::string description, std::string filter)
{
	return MarshalString(instance->BrowseFile(gcnew String(description.c_str()), gcnew String(filter.c_str())));
}


#pragma warning(pop)