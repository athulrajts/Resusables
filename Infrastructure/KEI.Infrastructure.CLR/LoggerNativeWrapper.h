#pragma once
#include <msclr/gcroot.h>
#include <string>

#pragma push_macro("Debug")
#pragma push_macro("Information")
#pragma push_macro("Warning")
#pragma push_macro("Error")
#undef Debug
#undef Information
#undef Warning
#undef Error


class LoggerNativeWrapper
{
public:
	LoggerNativeWrapper(KEI::Infrastructure::ILogger^ logger);

	void Debug(std::string message, const char* filename, const char* function_name, int line_number);
	void Information(std::string message, const char* filename, const char* function_name, int line_number);
	void Warning(std::string message, const char* filename, const char* function_name, int line_number);
	void Error(std::string message, const char* filename, const char* function_name, int line_number);

private:
	msclr::gcroot< KEI::Infrastructure::ILogger^> instance;
};

#pragma pop_macro("Debug")
#pragma pop_macro("Information")
#pragma pop_macro("Warning")
#pragma pop_macro("Error")
