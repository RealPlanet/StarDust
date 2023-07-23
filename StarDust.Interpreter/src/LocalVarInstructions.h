#pragma once

#ifndef _SDVM_LOCALVARINSTRUCTIONS_H_
#define _SDVM_LOCALVARINSTRUCTIONS_H_

#include "InstructionDefinition.h"

namespace sdi
{
	class LoadLocalDefinition
		: public InstructionDefinition
	{
	public:
		LoadLocalDefinition()
			: InstructionDefinition(_SDVM_LDLOC_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class LoadLocalVariant
		: public InstructionVariant
	{
		friend class LoadLocalDefinition;

	private:
		int32_t theLocalIndex;
		LoadLocalVariant(int32_t index)
			: theLocalIndex{ index } {}

	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "LoadLocalInstruction"; }
	};

	class StoreLocalDefinition
		: public InstructionDefinition
	{
	public:
		StoreLocalDefinition()
			: InstructionDefinition(_SDVM_STLOC_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class StoreLocalVariant
		: public InstructionVariant
	{
		friend class StoreLocalDefinition;

	private:
		int32_t theLocalIndex;
		StoreLocalVariant(int32_t index)
			: theLocalIndex{ index } {}

	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "StoreLocalInstruction"; }
	};
}

#endif // !_SDVM_LOCALVARINSTRUCTIONS_H_






