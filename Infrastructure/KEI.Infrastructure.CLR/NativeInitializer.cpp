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

	static void SetLogger(KEI::Infrastructure::ILogManager^ log_manager)
	{
		LogManagerNativeWrapper::SetLogger(log_manager);
	}

	static void SetViewService(KEI::Infrastructure::IViewService^ view_service)
	{
		ViewServiceNativeWrapper::SetViewService(view_service);
	}
};