
#include "ModuleEmitter.h"

namespace sde {
	ModuleEmitter::ModuleEmitter(const char* module_name, size_t name_len)
	{
		m_Data.ModuleName.assign(module_name, name_len);
	}

	void ModuleEmitter::dispose()
	{
		for (auto ptr : m_Data.ModuleEntities)
			ptr->dispose();
	}

	MethodEmitter* ModuleEmitter::add_method(const char* name)
	{
		if (m_Data.ModuleMethods.find(name) != m_Data.ModuleMethods.end())
		{
			throw std::exception("Method already exists!");
		}

		MethodEmitter* e = new MethodEmitter(name, strlen(name));
		m_Data.ModuleEntities.push_back(e);
		m_Data.ModuleMethods.insert(std::make_pair(std::string(name), e));
		return e;
	}

	void ModuleEmitter::write(std::stringstream* stream)
	{
		for (auto ptr : m_Data.ModuleEntities)
			ptr->write(stream);
	}
}


