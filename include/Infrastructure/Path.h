#pragma once
#include "exports.h"

#include <string>

class INFRASTRUCTURE_API Path
{
public:
	static std::string GetFullPath(std::string path);
	static std::string GetFileName(std::string path);
	static std::string GetFileNameWithoutExtension(std::string path);
	static std::string GetDirectoryName(std::string path);
};

