#pragma once

#include<msclr/gcroot.h>

class DataContainerNativeWrapper
{
public:
	DataContainerNativeWrapper();
	DataContainerNativeWrapper(KEI::Infrastructure::IDataContainer^ instance);

	std::vector<std::string> GetKeys();

	static DataContainerNativeWrapper* Load(std::string path);
	bool Store(std::string path);
	bool Store();

	bool Get(std::string key, std::string& value);
	bool Get(std::string key, bool& value);
	bool Get(std::string key, int& value);
	bool Get(std::string key, float& value);
	bool Get(std::string key, double& value);
	bool Get(std::string key, DataContainerNativeWrapper& value);

	void Put(std::string key, std::string value);
	void Put(std::string key, bool value);
	void Put(std::string key, int value);
	void Put(std::string key, float value);
	void Put(std::string key, double value);
	void Put(std::string key, DataContainerNativeWrapper* value);


private:
	msclr::gcroot<KEI::Infrastructure::IDataContainer^> instance;
};