#pragma once

#ifndef _SDVM_METHOD_EMITTER_H_
#define _SDVM_METHOD_EMITTER_H_

#include "EmitterEntity.h"
#include "Opcodes.h"

#include "Instruction.h"
#include "Argument.h"
#include "Attribute.h"

#include <vector>

namespace sde
{
	namespace internal {
		struct MethodEmitterData {
			std::string					MethodName;
			Attributes					MethodAttributes;
			Instructions				MethodInstructions;
		};
	}

	class MethodEmitter
		: public EmitterEntity
	{
	private:
		internal::MethodEmitterData m_Data;
		size_t m_LabelIndex = 0;

	public:
		_SDVM_API MethodEmitter(const char* method_name, size_t name_len);

		// Inherited via EmitterEntity
		virtual void dispose() override;
		virtual void write(std::stringstream* stream) override;

		_SDVM_API MethodEmitter* add_opcode(const Opcode id, Arguments& args);
		_SDVM_API MethodEmitter* add_opcode(const Opcode id);
		_SDVM_API MethodEmitter* add_attribute(const AttributeCode attribute, Arguments& args);
		_SDVM_API MethodEmitter* add_attribute(const AttributeCode attribute);

		_SDVM_API const char*	name() { return m_Data.MethodName.c_str(); }
		_SDVM_API Instructions& body() { return m_Data.MethodInstructions; }
	};
}

#endif // !_SDVM_METHOD_EMITTER_H_
