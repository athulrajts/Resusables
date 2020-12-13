#include "pch.h"

#include "DataContainer.h"
#include "DataContainerNativeWrapper.h"
#include "Utils.h"
#include <iostream>

using namespace System;
using namespace System::Linq;

#pragma warning(push)
#pragma warning( disable : 4691 )

#pragma region Implementation

DataContainer::DataContainer()
{
	ManagedDC = new DataContainerNativeWrapper();
}

DataContainer::DataContainer(DataContainerNativeWrapper* wrapper)
{
	ManagedDC = wrapper;
}

DataContainer::~DataContainer()
{
	if (ManagedDC)
	{
		delete ManagedDC;
	}
}

bool DataContainer::Get(std::string key, std::string& value)
{
	return ManagedDC->Get(key, value);
}

bool DataContainer::Get(std::string key, int& value)
{
	return ManagedDC->Get(key, value);
}

bool DataContainer::Get(std::string key, bool& value)
{
	return ManagedDC->Get(key, value);
}

bool DataContainer::Get(std::string key, float& value)
{
	return ManagedDC->Get(key, value);
}

bool DataContainer::Get(std::string key, double& value)
{
	return ManagedDC->Get(key, value);
}

bool DataContainer::Get(std::string key, DataContainer& value)
{
	return ManagedDC->Get(key, *value.ManagedDC);
}

void DataContainer::Put(std::string key, std::string value)
{
	ManagedDC->Put(key, value);
}

void DataContainer::Put(std::string key, bool value)
{
	ManagedDC->Put(key, value);
}

void DataContainer::Put(std::string key, int value)
{
	ManagedDC->Put(key, value);
}

void DataContainer::Put(std::string key, float value)
{
	ManagedDC->Put(key, value);
}

void DataContainer::Put(std::string key, double value)
{
	ManagedDC->Put(key, value);
}

void DataContainer::Put(std::string key, DataContainer* value)
{
	ManagedDC->Put(key, value->ManagedDC);
}

std::vector<std::string> DataContainer::GetKeys()
{
	return ManagedDC->GetKeys();
}

DataContainer* DataContainer::Load(std::string path)
{
	return new DataContainer(DataContainerNativeWrapper::Load(path));
}

bool DataContainer::Store(std::string path)
{
	return ManagedDC->Store(path);
}

bool DataContainer::Store()
{
	return ManagedDC->Store();
}


#pragma endregion


#pragma region Wrapper

DataContainerNativeWrapper::DataContainerNativeWrapper()
{
	instance = gcnew KEI::Infrastructure::DataContainer();
}

DataContainerNativeWrapper::DataContainerNativeWrapper(KEI::Infrastructure::IDataContainer^ managed)
{
	instance = managed;
}

bool DataContainerNativeWrapper::Get(std::string key, std::string& value)
{
	String^ val = gcnew String(value.c_str());
	bool retValue = instance->GetValue(gcnew String(key.c_str()), val);
	value = MarshalString(val);
	return retValue;
}

bool DataContainerNativeWrapper::Get(std::string key, int& value)
{
	return instance->GetValue(gcnew String(key.c_str()), value);
}

bool DataContainerNativeWrapper::Get(std::string key, bool& value)
{
	return instance->GetValue(gcnew String(key.c_str()), value);
}

bool DataContainerNativeWrapper::Get(std::string key, float& value)
{
	return instance->GetValue(gcnew String(key.c_str()), value);
}

bool DataContainerNativeWrapper::Get(std::string key, double& value)
{
	return instance->GetValue(gcnew String(key.c_str()), value);
}

bool DataContainerNativeWrapper::Get(std::string key, DataContainerNativeWrapper& value)
{
	KEI::Infrastructure::IDataContainer^ val = gcnew KEI::Infrastructure::DataContainer();
	bool retValue = instance->GetValue(gcnew String(key.c_str()), val);
	value = DataContainerNativeWrapper(val);
	return retValue;
}

void DataContainerNativeWrapper::Put(std::string key, std::string value)
{
	KEI::Infrastructure::DataContainerExtensions::PutValue(instance, gcnew String(key.c_str()), gcnew String(value.c_str()));
}

void DataContainerNativeWrapper::Put(std::string key, bool value)
{
	KEI::Infrastructure::DataContainerExtensions::PutValue(instance, gcnew String(key.c_str()), value);
}

void DataContainerNativeWrapper::Put(std::string key, int value)
{
	KEI::Infrastructure::DataContainerExtensions::PutValue(instance, gcnew String(key.c_str()), value);
}

void DataContainerNativeWrapper::Put(std::string key, float value)
{
	KEI::Infrastructure::DataContainerExtensions::PutValue(instance, gcnew String(key.c_str()), value);
}

void DataContainerNativeWrapper::Put(std::string key, double value)
{
	KEI::Infrastructure::DataContainerExtensions::PutValue(instance, gcnew String(key.c_str()), value);
}

void DataContainerNativeWrapper::Put(std::string key, DataContainerNativeWrapper* value)
{
	KEI::Infrastructure::DataContainerExtensions::PutValue(instance, gcnew String(key.c_str()), value->instance);
}

std::vector<std::string> DataContainerNativeWrapper::GetKeys()
{
	std::vector<std::string> keys;

	for each (String^ key in instance->GetKeys())
	{
		keys.push_back(MarshalString(key));
	}

	return keys;
}

DataContainerNativeWrapper* DataContainerNativeWrapper::Load(std::string path)
{
	KEI::Infrastructure::IDataContainer^ dc = KEI::Infrastructure::DataContainer::FromFile(gcnew String(path.c_str()));
	return new DataContainerNativeWrapper(dc);
}

bool DataContainerNativeWrapper::Store(std::string path)
{
	return instance->Store(gcnew String(path.c_str()));
}

bool DataContainerNativeWrapper::Store()
{
	return instance->Store();
}

#pragma endregion


#pragma warning(pop)