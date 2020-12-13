#pragma once
#include "exports.h"
#include <string>

class LoggerNativeWrapper;

class INFRASTRUCTURE_API Logger
{
public:
	Logger(LoggerNativeWrapper* wrapper);
	~Logger();

	void Debug(std::string message, const char* filename, const char* function_name, int line_number);
	void Information(std::string message, const char* filename, const char* function_name, int line_number);
	void Warning(std::string message, const char* filename, const char* function_name, int line_number);
	void Error(std::string message, const char* filename, const char* function_name, int line_number);

private:
	LoggerNativeWrapper* ManagedLogger;
};