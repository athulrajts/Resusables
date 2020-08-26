#pragma once
#include <string>
#include <msclr\marshal_cppstd.h>

using namespace System;

static std::string MarashalString(String^ managedString)
{
    msclr::interop::marshal_context context;
    return context.marshal_as<std::string>(managedString);
}

