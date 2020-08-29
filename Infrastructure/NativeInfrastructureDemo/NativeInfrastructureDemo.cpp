// NativeInfrastructureDemo.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "Infrastructure.h"


int main()
{

    //LogManager::InitializeConsole();

    //auto logger = LogManager::GetLogger("Test");

    //logger->Information("TestLog");

    std::cout << "Hello World!\n";

    bool is_initalized = ViewService::InitializeBaseViewService();

    std::cout << is_initalized;

    ViewService::BrowseFile("DLL files", "dll");

    //MessageBoxService::ErrorDialog("404");
    //MessageBoxService::WarningDialog("Not Found");
    //MessageBoxService::InfoDialog("How are you ?");

    //auto result = MessageBoxService::Question("Are you sure you want to quit ?", MessageBoxButton::YesNo);

    //std::cout << (int)result;

    getchar();
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
