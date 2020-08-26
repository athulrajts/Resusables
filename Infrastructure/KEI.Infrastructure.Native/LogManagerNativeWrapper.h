#pragma once
#include <msclr/gcroot.h>
#include <string>

#include "Logger.h"

class LogManagerNativeWrapper
{
public:
	
	static void AutoInitialize();
	static void InitializeConsole();
	static void InitializeFile(std::string baseName);

	static Logger* GetLogger(std::string name);
	static Logger* GetDefaultLogger();

	static msclr::gcroot<KEI::Infrastructure::ILogManager^> GetCurrent()
	{
		return ManagedLogManager;
	}

private:
	static msclr::gcroot<KEI::Infrastructure::ILogManager^> ManagedLogManager;
};
