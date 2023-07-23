#pragma once

#ifndef _SDVM_EMPTY_INSTRUCTION_H
#define _SDVM_EMPTY_INSTRUCTION_H

#include "InstructionDefinition.h"

namespace sdi
{

	class EmptyInstructionDefinition
		: public InstructionDefinition
	{
	public:
		EmptyInstructionDefinition()
			: InstructionDefinition(_SDVM_NOP_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class EmptyInstructionVariant
		: public InstructionVariant
	{
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "EmptyInstruction"; }
	};
}

#endif // !_SDVM_EMPTY_INSTRUCTION_H




