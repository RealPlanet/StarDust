#pragma once

#ifndef _SDVM_SDEMITTER_H_
#define _SDVM_SDEMITTER_H_

#include "EmitterEntity.h"
#include "MethodEmitter.h"
#include "Opcodes.h"

#include <string>
#include <vector>
#include <map>
#include <fstream>

namespace sde
{
	// Opcode to string
	const char* otos(Opcode opcode);
	// Attribute to string
	const char* atos(Attribute opcode);

	class SDEmitter
	{
	private:
#pragma warning( push )
#pragma warning( disable : 4251 )
		std::vector<EmitterEntity*> theAllocatedEntities;
		std::map<std::string, MethodEmitter*> theAllocatedMethods;
		std::string theModuleName;
		std::ofstream theOutputModule;
#pragma warning( pop )

	public:
		_SDVM_API SDEmitter(const char* moduleName, size_t len);
		_SDVM_API SDEmitter(const wchar_t* moduleName, size_t len);
		_SDVM_API ~SDEmitter() { for (EmitterEntity* ent : theAllocatedEntities) { ent->dispose(); delete ent; ent = NULL; }}


		_SDVM_API MethodEmitter* emit_method(const char* methodName, size_t len);
		_SDVM_API MethodEmitter* emit_method(const wchar_t* methodName, size_t len);
		void generate(std::stringstream* stream) { for (EmitterEntity* ent : theAllocatedEntities) ent->write(stream); }

		_SDVM_API size_t number_of_methods() { return theAllocatedMethods.size(); }
	};
}

#endif // !_SDVM_SDEMITTER_H_




