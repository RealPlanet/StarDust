#include "stdafx.h"

#include "InstructionSequence.h"

bool sdi::InstructionSequence::insert_instruction(InstructionVariantPtr ptr, size_t label)
{
    // Replace existing instruction
    if (m_InstructionVec.size() > label)
    {
        m_InstructionVec[label] = ptr;
        return true;
    }

    // Append instruction
    if (m_InstructionVec.size() == label)
    {
        m_InstructionVec.push_back(ptr);
        return true;
    }

    throw std::exception("Unable to insert instruction, label is outside of bounds!");
}
