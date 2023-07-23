#pragma once

#ifndef _SDVM_LOADCONSTANT32INSTRUCTION_H_
#define _SDVM_LOADCONSTANT32INSTRUCTION_H_

#include "InstructionDefinition.h"

namespace sdi
{
	class LoadConstant32Definition
		: public InstructionDefinition
	{
	public:
		LoadConstant32Definition()
			: InstructionDefinition(_SDVM_LDCI4_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class LoadConstant32Variant
		: public InstructionVariant
	{
		friend class LoadConstant32Definition;

	private:
		int32_t m_iConstant;
		LoadConstant32Variant(int32_t value)
			: m_iConstant{value}{}

	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "LoadConstant32Instruction"; }
	};


	class LoadConstantStrDefinition
		: public InstructionDefinition
	{
	public:
		LoadConstantStrDefinition()
			: InstructionDefinition(_SDVM_LDCST_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;

	};

	class LoadConstantStrVariant
		: public InstructionVariant
	{
		friend class LoadConstantStrDefinition;

	private:
		std::string m_sConstant;
		LoadConstantStrVariant(std::string& value)
			: m_sConstant{ value } {}

	public:
		// Inherited via InstructionVariant
		virtual int execute(ExecutionScope* scope) override;
		virtual const char* getVariantName() override { return "LoadConstantStrInstruction"; }
	};
}

#endif // !_SDVM_LOADCONSTANT32INSTRUCTION_H_






