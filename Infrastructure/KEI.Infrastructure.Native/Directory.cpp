#include "pch.h"
#include <msclr\marshal_cppstd.h>

#include "Directory.h"
#include "Utils.h"

using namespace System;

bool Directory::Exists(std::string path)
{
    return IO::Directory::Exists(gcnew String(path.c_str()));
}

void Directory::CreateDirectory(std::string path)
{
    IO::Directory::CreateDirectory(gcnew String(path.c_str()));
}

std::vector<std::string> Directory::GetFiles(std::string path)
{
    std::vector<std::string> files;

    for each (String ^ file in IO::Directory::EnumerateFiles(gcnew String(path.c_str())))
    {
        files.push_back(MarashalString(file));
    }

    return files;
}

std::vector<std::string> Directory::GetDirectories(std::string path)
{
    std::vector<std::string> dirs;

    for each (String ^ file in IO::Directory::EnumerateDirectories(gcnew String(path.c_str())))
    {
        dirs.push_back(MarashalString(file));
    }

    return dirs;
}