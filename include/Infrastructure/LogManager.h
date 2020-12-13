#pragma once

#include "Logger.h"

class LogManagerNativeWrapper;

class INFRASTRUCTURE_API LogManager
{
public:
	static void AutoInitialize();
	static void InitializeConsole();
	static void InitializeFile(std::string baseName);

	static Logger* GetLogger(std::string name);
	static Logger* GetDefaultLogger();
};


#define LOG_DEBUG(msg) LogManager::GetDefaultLogger()->Debug(msg, __FILE__ , __func__, __LINE__)
#define LOG_INFORMATION(msg) LogManager::GetDefaultLogger()->Information(msg, __FILE__ , __func__, __LINE__)
#define LOG_WARNING(msg) LogManager::GetDefaultLogger()->Warning(msg, __FILE__ , __func__, __LINE__)
#define LOG_ERROR(msg) LogManager::GetDefaultLogger()->Error(msg, __FILE__ , __func__, __LINE__)