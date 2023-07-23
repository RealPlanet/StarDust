#pragma once

#ifndef _SDVM_CALLINSTRUCTION_H_
#define _SDVM_CALLINSTRUCTION_H_

#include "InstructionDefinition.h"
#include "Function.h"

namespace sdi
{
	class CallDefinition
		: public InstructionDefinition
	{
	public:
		CallDefinition()
			: InstructionDefinition(_SDVM_CALL_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class CallInstruction
		: public InstructionVariant
	{
		friend class CallDefinition;
	private:
		std::string m_strFunctionName = "";
		bool m_bIsBuiltinCall = false;
		bool m_bIsFakeThread = false;
		FunctionPtr theFunctionToCall = nullptr;

		CallInstruction() {}
	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "CallInstruction"; }
	};
}

#endif // !_SDVM_RETURNINSTRUCTION_H_

