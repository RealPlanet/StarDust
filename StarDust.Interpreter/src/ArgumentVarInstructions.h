#pragma once

#ifndef _SDVM_ARGVARINSTRUCTIONS_H_
#define _SDVM_ARGVARINSTRUCTIONS_H_

#include "InstructionDefinition.h"

namespace sdi
{
	class LoadArgDefinition
		: public InstructionDefinition
	{
	public:
		LoadArgDefinition()
			: InstructionDefinition(_SDVM_LDARG_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class LoadArgVariant
		: public InstructionVariant
	{
		friend class LoadArgDefinition;

	private:
		int32_t theArgIndex;

		LoadArgVariant(int32_t index)
			: theArgIndex{ index } {}

	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "LoadArgInstruction"; }
	};

	class StoreArgDefinition
		: public InstructionDefinition
	{
	public:
		StoreArgDefinition()
			: InstructionDefinition(_SDVM_STARG_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class StoreArgVariant
		: public InstructionVariant
	{
		friend class StoreArgDefinition;

	private:
		int32_t theArgIndex;

		StoreArgVariant(int32_t index)
			: theArgIndex{ index } {}

	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "StoreArgInstruction"; }
	};
}

#endif // !_SDVM_ARGVARINSTRUCTIONS_H_
