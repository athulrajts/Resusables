#pragma once

#include "pch.h"
#include "ViewServiceNativeWrapper.h"
#include "Utils.h"

using namespace Prism::Ioc;
using namespace System;
using namespace System::Collections::Generic;

#pragma warning(push)
#pragma warning(disable : 4691)


msclr::gcroot<KEI::Infrastructure::IViewService^> ViewServiceNativeWrapper::instance = nullptr;

void ViewServiceNativeWrapper::AutoInitialize()
{
	instance = (KEI::Infrastructure::IViewService^)ContainerLocator::Current->Resolve(KEI::Infrastructure::IViewService::typeid);
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

	KEI::Infrastructure::FilterCollection^ filters = gcnew KEI::Infrastructure::FilterCollection();
	filters->Add(gcnew KEI::Infrastructure::Filter(gcnew String(description.c_str()), gcnew String(filter.c_str())));

	return MarshalString(instance->BrowseFile(filters, gcnew String("")));
}


#pragma warning(pop)