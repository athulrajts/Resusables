#include "pch.h"

#include "OS.h"
#include "Utils.h"

std::string OS::GetCurrentDirectory()
{
    return MarashalString(System::Environment::CurrentDirectory);
}

std::string OS::GetUserDataDirectory()
{
    return MarashalString(System::Environment::GetFolderPath(System::Environment::SpecialFolder::CommonApplicationData));
}
