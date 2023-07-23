#pragma once

#ifndef _SDVM_INSTRUCTION_VARIANT_H_
#define _SDVM_INSTRUCTION_VARIANT_H_

namespace sdi
{
	class ExecutionScope;

	/// <summary>
	/// Rapresentation of a specific instruction at runtime
	/// </summary>
	class InstructionVariant
	{
	public:
		virtual int execute(ExecutionScope* scope) = 0;
		virtual const char* getVariantName() { return typeid(this).name(); };
	};

	typedef std::shared_ptr<InstructionVariant> InstructionVariantPtr;
}

#endif //!_SDVM_INSTRUCTION_VARIANT_H_
