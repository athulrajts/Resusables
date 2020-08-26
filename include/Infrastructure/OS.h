#pragma once
#include "exports.h"

#include <string>
class INFRASTRUCTURE_API OS
{
public:
	static std::string GetCurrentDirectory();
	static std::string GetUserDataDirectory();
};

