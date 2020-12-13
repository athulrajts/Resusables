#pragma once
#include "pch.h"
#include <string>

#include "LoggerNativeWrapper.h"

using namespace System;
using namespace KEI::Infrastructure;

#pragma warning(push)
#pragma warning(disable : 4691)

#pragma region Wrapper

LoggerNativeWrapper::LoggerNativeWrapper(ILogger^ logger)
{
	instance = logger;
}

void LoggerNativeWrapper::Debug(std::string message, const char* filename, const char* function_name, int line_number)
{
	instance->Log(KEI::Infrastructure::Logging::LogLevel::Debug, gcnew String(message.c_str()),
		gcnew String(filename),
		gcnew String(function_name),
		line_number);
}

void LoggerNativeWrapper::Information(std::string message, const char* filename, const char* function_name, int line_number)
{
	instance->Log(KEI::Infrastructure::Logging::LogLevel::Information, gcnew String(message.c_str()),
		gcnew String(filename),
		gcnew String(function_name),
		line_number);
}

void LoggerNativeWrapper::Warning(std::string message, const char* filename, const char* function_name, int line_number)
{
	instance->Log(KEI::Infrastructure::Logging::LogLevel::Warning, gcnew String(message.c_str()),
		gcnew String(filename),
		gcnew String(function_name),
		line_number);
}

void LoggerNativeWrapper::Error(std::string message, const char* filename, const char* function_name, int line_number)
{
	instance->Log(KEI::Infrastructure::Logging::LogLevel::Error, gcnew String(message.c_str()),
		gcnew String(filename),
		gcnew String(function_name),
		line_number);
}

#pragma endregion

#pragma warning(pop)