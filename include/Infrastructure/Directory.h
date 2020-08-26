#pragma once
#include "exports.h"

#include <string>
#include <vector>

class INFRASTRUCTURE_API Directory
{
public:
	static bool Exists(std::string path);
	static void CreateDirectory(std::string path);
	static std::vector<std::string> GetFiles(std::string path);
	static std::vector<std::string> GetDirectories(std::string path);
};

