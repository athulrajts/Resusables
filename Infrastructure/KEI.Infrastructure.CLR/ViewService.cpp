#pragma once

#include "pch.h"
#include "ViewService.h"
#include "ViewServiceNativeWrapper.h"

void ViewService::AutoInitialize()
{
	ViewServiceNativeWrapper::AutoInitialize();
}

void ViewService::InitializeBaseViewService()
{
	ViewServiceNativeWrapper::InitializeBaseViewService();
}

void ViewService::ErrorDialog(std::string message, bool is_modal)
{
	ViewServiceNativeWrapper::ErrorDialog(message, is_modal);
}

void ViewService::WarningDialog(std::string message, bool is_modal)
{
	ViewServiceNativeWrapper::WarningDialog(message, is_modal);
}

void ViewService::InformationDialog(std::string message, bool is_modal)
{
	ViewServiceNativeWrapper::InformationDialog(message, is_modal);
}

PromptResult ViewService::PromptDialog(std::string message, PromptOptions option)
{
	return ViewServiceNativeWrapper::PromptDialog(message, option);
}

std::string ViewService::BrowseFile(std::string filterName, std::string fileterExts)
{
	return ViewServiceNativeWrapper::BrowseFile(filterName, fileterExts);
}
