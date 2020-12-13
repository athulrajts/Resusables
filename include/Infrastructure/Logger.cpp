#include "pch.h"
#include "Logger.h"
#include "LoggerNativeWrapper.h"

using namespace System;
using namespace KEI::Infrastructure;

#pragma warning(push)
#pragma warning(disable : 4691)

#pragma region Logger 

Logger::Logger(LoggerNativeWrapper* wrapper)
{
	ManagedLogger = wrapper;
}

Logger::~Logger()
{
	if (ManagedLogger)
	{
		delete ManagedLogger;
	}
}


void Logger::Debug(std::string message, const char* filename, const char* function_name, int line_number)
{
	ManagedLogger->Debug(message, filename, function_name, line_number);
}

void Logger::Information(std::string message, const char* filename, const char* function_name, int line_number)
{
	ManagedLogger->Information(message, filename, function_name, line_number);
}

void Logger::Warning(std::string message, const char* filename, const char* function_name, int line_number)
{
	ManagedLogger->Warning(message, filename, function_name, line_number);
}

void Logger::Error(std::string message, const char* filename, const char* function_name, int line_number)
{
	ManagedLogger->Error(message, filename, function_name, line_number);
}

#pragma endregion

#pragma warning(pop)
