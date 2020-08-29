#include "pch.h"

#include "OS.h"
#include "Utils.h"

std::string OS::GetCurrentDirectory()
{
    return MarshalString(System::Environment::CurrentDirectory);
}

std::string OS::GetUserDataDirectory()
{
    return MarshalString(System::Environment::GetFolderPath(System::Environment::SpecialFolder::CommonApplicationData));
}
