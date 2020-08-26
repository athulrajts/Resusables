#include "pch.h"
#include <msclr\marshal_cppstd.h>

#include "File.h"
#include "Utils.h"

using namespace System;

bool File::Exists(std::string path)
{
    return IO::File::Exists(gcnew String(path.c_str()));
}

std::string File::ReadAllText(std::string path)
{
    String^ text = IO::File::ReadAllText(gcnew String(path.c_str()));
    return MarashalString(text);
}

std::vector<std::string> File::ReadAllLines(std::string path)
{
    std::vector<std::string> lines;
    array<String^>^ allLines = IO::File::ReadAllLines(gcnew String(path.c_str()));

    for each (String^ line in allLines)
    {
        lines.push_back(MarashalString(line));
    }
    return lines;
}

void File::Delete(std::string path)
{
    IO::File::Delete(gcnew String(path.c_str()));
}

void File::WriteAllText(std::string path, std::string contents)
{
    IO::File::WriteAllText(gcnew String(path.c_str()), gcnew String(contents.c_str()));
}

void File::WriteAllLines(std::string path, std::vector<std::string> contents)
{
    Collections::Generic::List<String^>^ lines;

    for each (std::string line in contents)
    {
        lines->Add(gcnew String(line.c_str()));
    }

    IO::File::WriteAllLines(gcnew String(path.c_str()), lines);
}

void File::AppendAllText(std::string path, std::string contents)
{
    IO::File::AppendAllText(gcnew String(path.c_str()), gcnew String(contents.c_str()));
}

void File::AppendAllLines(std::string path, std::vector<std::string> contents)
{
    Collections::Generic::List<String^>^ lines;

    for each (std::string line in contents)
    {
        lines->Add(gcnew String(line.c_str()));
    }

    IO::File::AppendAllLines(gcnew String(path.c_str()), lines);
}