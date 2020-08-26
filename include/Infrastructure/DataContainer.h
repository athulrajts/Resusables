#pragma once
#include "exports.h"
#include <string>
#include <vector>



class DataContainerNativeWrapper;

class INFRASTRUCTURE_API DataContainer
{
public:
	DataContainer();
	DataContainer(DataContainerNativeWrapper* wrapper);
	~DataContainer();

	std::vector<std::string> GetKeys();

	static DataContainer* Load(std::string path);
	bool Store(std::string path);
	bool Store();

	bool Get(std::string key, std::string& value);
	bool Get(std::string key, bool& value);
	bool Get(std::string key, int& value);
	bool Get(std::string key, float& value);
	bool Get(std::string key, double& value);
	bool Get(std::string key, DataContainer& value);

	void Put(std::string key, std::string value);
	void Put(std::string key, bool value);
	void Put(std::string key, int value);
	void Put(std::string key, float value);
	void Put(std::string key, double value);
	void Put(std::string key, DataContainer* value);


private:
	DataContainerNativeWrapper* ManagedDC;
};
