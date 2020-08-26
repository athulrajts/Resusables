#pragma once
#include "pch.h"
#include "MessageBoxService.h"

using namespace System;



void MessageBoxService::ErrorDialog(std::string message)
{
	System::Windows::MessageBox::Show(gcnew String(message.c_str()),
		gcnew String("Error"),
		System::Windows::MessageBoxButton::OK,
		System::Windows::MessageBoxImage::Error);
}

void MessageBoxService::InfoDialog(std::string message)
{
	System::Windows::MessageBox::Show(gcnew String(message.c_str()),
		gcnew String("Information"),
		System::Windows::MessageBoxButton::OK,
		System::Windows::MessageBoxImage::Information);
}

void MessageBoxService::WarningDialog(std::string message)
{
	System::Windows::MessageBox::Show(gcnew String(message.c_str()),
		gcnew String("Warning"),
		System::Windows::MessageBoxButton::OK,
		System::Windows::MessageBoxImage::Warning);
}

MessageBoxResult MessageBoxService::Question(std::string message, MessageBoxButton option)
{
	System::Windows::MessageBoxResult result = System::Windows::MessageBox::Show(gcnew String(message.c_str()),
		gcnew String("Warning"),
		(System::Windows::MessageBoxButton)option,
		System::Windows::MessageBoxImage::Question);

	return (MessageBoxResult)result;
}