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

#define Debug(msg) Debug(msg, __FILE__ , __func__, __LINE__)
#define Information(msg) Information(msg, __FILE__ , __func__, __LINE__)
#define Warning(msg) Warning(msg, __FILE__ , __func__, __LINE__)
#define Error(msg) Error(msg, __FILE__ , __func__, __LINE__)