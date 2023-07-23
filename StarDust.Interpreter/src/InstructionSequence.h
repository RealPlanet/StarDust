#pragma once

#ifndef _SDVM_INSTRUCTION_SEQUENCE_H_
#define _SDVM_INSTRUCTION_SEQUENCE_H_

#include "InstructionVariant.h"

namespace sdi
{
	typedef std::vector<InstructionVariantPtr> InstructionVector;

	class InstructionSequence
	{
	private:
		InstructionVector m_InstructionVec;
	public:
		InstructionVariantPtr instruction_at_label(size_t label) { return m_InstructionVec[label]; }
		bool insert_instruction(InstructionVariantPtr ptr, size_t label);
	};
}

#endif // !_SDVM_INSTRUCTION_SEQUENCE_H_
