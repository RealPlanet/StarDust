#include "stdafx.h"

#include "Instruction.h"
// TODO MOVE OTOS AND OTHER UTILITY INTO A SEPARATE HEADER
#include "SDEmitter.h"

namespace sde {
	void Instruction::write(std::stringstream* stream)
	{
		std::string instName(otos(opcode()));
		std::stringstream ss;

		ss << _SDVM_TAB << _SDVM_LABEL_PREFIX << std::setfill('0') << std::setw(4) << m_Offset << " : " << instName;
		for (auto& strArg : m_Data)
			ss << " " << strArg;

		*stream << ss.str() << _SDVM_GENERIC_NEWLINE;
	}
}
