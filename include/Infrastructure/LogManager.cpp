#include "pch.h"

#include "LogManager.h"
#include "LogManagerNativeWrapper.h"
#include "LoggerNativeWrapper.h"

using namespace System;
using namespace KEI::Infrastructure;

#pragma warning(push)
#pragma warning(disable : 4691)

#pragma region Implementation

void LogManager::AutoInitialize()
{
	LogManagerNativeWrapper::AutoInitialize();
}

void LogManager::InitializeConsole()
{
	LogManagerNativeWrapper::InitializeConsole();
}

void LogManager::InitializeFile(std::string baseName)
{
	LogManagerNativeWrapper::InitializeFile(baseName);
}

Logger* LogManager::GetLogger(std::string name)
{
	return LogManagerNativeWrapper::GetLogger(name);
}

Logger* LogManager::GetDefaultLogger()
{
	return LogManagerNativeWrapper::GetDefaultLogger();
}



#pragma endregion


#pragma region Wrapper

msclr::gcroot<KEI::Infrastructure::ILogManager^> LogManagerNativeWrapper::ManagedLogManager = nullptr;

void LogManagerNativeWrapper::AutoInitialize()
{
	ManagedLogManager = CommonServiceLocator::ServiceLocator::Current->GetInstance<ILogManager^>();
}

void LogManagerNativeWrapper::InitializeConsole()
{
	ManagedLogManager = Logging::SimpleLogConfigurator().ConfigureConsoleLogger();
}

void LogManagerNativeWrapper::InitializeFile(std::string baseName)
{
	ManagedLogManager = Logging::SimpleLogConfigurator().ConfigureFileLogger(gcnew String(baseName.c_str()));
}

Logger* LogManagerNativeWrapper::GetLogger(std::string name)
{
	ILogger^ logger = ManagedLogManager->GetLogger(gcnew String(name.c_str()));
	return new Logger(new LoggerNativeWrapper(logger));
}

Logger* LogManagerNativeWrapper::GetDefaultLogger()
{
	ILogger^ logger = ManagedLogManager->DefaultLogger;
	return new Logger(new LoggerNativeWrapper(logger));
}

#pragma endregion



#pragma warning(pop)