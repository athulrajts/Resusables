#pragma once
#include "exports.h"

#include <string>
#include <vector>

constexpr auto EMPTY_STRING = "";

class INFRASTRUCTURE_API File
{
public:
	static bool Exists(std::string path);
	static std::string ReadAllText(std::string path);
	static std::vector<std::string> ReadAllLines(std::string path);
	static void Delete(std::string path);
	static void WriteAllText(std::string path, std::string contents);
	static void WriteAllLines(std::string path, std::vector<std::string> contents);
	static void AppendAllText(std::string path, std::string contents);
	static void AppendAllLines(std::string path, std::vector<std::string> contents);
};

