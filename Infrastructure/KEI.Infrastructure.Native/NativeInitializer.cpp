#include "pch.h"
#include "LogManagerNativeWrapper.h"
#include "ViewServiceNativeWrapper.h"

public ref class NativeInitializer
{
public:
	static void InitializeLogger()
	{
		LogManagerNativeWrapper::AutoInitialize();
	}

	static void InitalizeViewService()
	{
		ViewServiceNativeWrapper::AutoInitialize();
	}
};