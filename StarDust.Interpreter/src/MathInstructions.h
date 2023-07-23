#pragma once

#ifndef _SDVM_MATH_INSTRUCTIONS_H_
#define _SDVM_MATH_INSTRUCTIONS_H_

#include "InstructionDefinition.h"

namespace sdi
{
#pragma region
	class AddInstructionDefinition
		: public InstructionDefinition
	{
	public:
		AddInstructionDefinition()
			: InstructionDefinition(_SDVM_ADD_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;

	};

	class AddInstruction
		: public InstructionVariant
	{
	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "AddInstruction"; }
	};
#pragma endregion Add

#pragma region
	class SubInstructionDefinition
		: public InstructionDefinition
	{
	public:
		SubInstructionDefinition()
			: InstructionDefinition(_SDVM_SUB_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;

	};

	class SubInstruction
		: public InstructionVariant
	{
	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "SubInstruction"; }
	};
#pragma endregion Sub
}

#endif // !_SDVM_MATH_INSTRUCTIONS_H_


