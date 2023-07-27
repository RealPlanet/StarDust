#pragma once

#include "Emitter.h"
#include "MethodEmitter.h"

namespace sde {

	namespace internal{
		struct EmitterData {
			std::vector<EmitterEntity*>				ModuleEntities;
			std::map<std::string, MethodEmitter*>	ModuleMethods;

			std::string ModuleName;
			std::ofstream ModuleStream;
		};
	}

	class ModuleEmitter
		: public Emitter
	{
	protected:
		internal::EmitterData m_Data;

	public:
		_SDVM_API ModuleEmitter(const char* module_name, size_t name_len);
		_SDVM_API size_t method_count() { return m_Data.ModuleMethods.size(); }

		_SDVM_API MethodEmitter* add_method(const char* name);

		_SDVM_API virtual void write(std::stringstream* stream) override;

		virtual void dispose() override;
	};
}

