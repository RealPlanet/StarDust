#pragma once

#ifndef _SDVM_METHOD_EMITTER_H_
#define _SDVM_METHOD_EMITTER_H_

#include "EmitterEntity.h"
#include "Opcodes.h"
#include "Instruction.h"

#include <vector>

namespace sde
{
	class SDEmitter;

	class VariableDefinition {
	private:
		int m_Index = -1;

	};

	class MethodEmitter
		: public EmitterEntity
	{
	private:
#pragma warning( push )
#pragma warning( disable : 4251 )
		std::string theMethodName;
		std::vector<Instruction> theMethodInstructions;
		std::vector<std::string> theMethodAttributes;
#pragma warning( pop )

		SDEmitter* theContext;
		size_t theLabelIndex = 0;
	public:
		_SDVM_API MethodEmitter(SDEmitter* ctx, const char* methodStr, size_t len)
			: theContext{ ctx }
		{
			theMethodName.assign(methodStr, len);
		}

		// Inherited via EmitterEntity
		virtual void dispose() override {}
		virtual void write(std::stringstream* stream) override;

		_SDVM_API MethodEmitter* append_opcode(Opcode id, std::vector<std::string>* vecArguments);
		_SDVM_API MethodEmitter* add_attribute(Attribute attribute, std::vector<std::string>* vecArguments);
		_SDVM_API const std::vector<Instruction>* body() { return &theMethodInstructions; }
		_SDVM_API const char* name() { return theMethodName.c_str(); }

		_SDVM_API size_t		instruction_size() { return theMethodInstructions.size(); }
		_SDVM_API Instruction*	instruction_at(size_t i) { return &theMethodInstructions[i]; }
		_SDVM_API void			set_instruction_at(size_t i, Instruction* inst) { theMethodInstructions.at(i) = *inst; }
	};
}

#endif // !_SDVM_METHOD_EMITTER_H_
