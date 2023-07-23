#pragma once

#ifndef _SDVM_RETURNINSTRUCTION_H_
#define _SDVM_RETURNINSTRUCTION_H_

#include "InstructionDefinition.h"

namespace sdi
{
	class ReturnInstructionDefinition
		: public InstructionDefinition
	{
	public:
		ReturnInstructionDefinition()
			: InstructionDefinition(_SDVM_RET_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;

	};

	class ReturnInstruction
		: public InstructionVariant
	{
	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "RetInstruction"; }
	};
}

#endif // !_SDVM_RETURNINSTRUCTION_H_
