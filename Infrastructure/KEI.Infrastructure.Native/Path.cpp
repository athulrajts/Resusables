#include "pch.h"
#include <msclr\marshal_cppstd.h>

#include "Path.h"
#include "Utils.h"

using namespace System;


std::string Path::GetFullPath(std::string path)
{
	String^ fullpath = IO::Path::GetFullPath(gcnew String(path.c_str()));
    return MarashalString(fullpath);
}

std::string Path::GetFileName(std::string path)
{
    return MarashalString(IO::Path::GetFileName(gcnew String(path.c_str())));
}

std::string Path::GetFileNameWithoutExtension(std::string path)
{
    return MarashalString(IO::Path::GetFileNameWithoutExtension(gcnew String(path.c_str())));
}

std::string Path::GetDirectoryName(std::string path)
{
    return MarashalString(IO::Path::GetDirectoryName(gcnew String(path.c_str())));
}
