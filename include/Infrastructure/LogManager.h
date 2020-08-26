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
