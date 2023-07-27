#include "Instruction.h"
#include "Utility.h"

#include <iomanip>

namespace sde {
	void Instruction::write(std::stringstream* stream)
	{
		std::string instName(utility::otos(opcode()));
		std::stringstream ss;

		ss << _SDVM_TAB << _SDVM_LABEL_PREFIX << std::setfill('0') << std::setw(4) << m_Offset << " : " << instName;
		for (size_t i{0}; i < m_Data.size(); i++)
			ss << " " << m_Data.at(i);

		*stream << ss.str() << _SDVM_GENERIC_NEWLINE;
	}

	void Instructions::dispose()
	{
		for (size_t x{ 0 }; x < m_Data.size(); x++) {
			Instruction& inst = m_Data.at(x);
			inst.dispose();
		};

		this->clear();
	}

	void Instructions::write(std::stringstream* stream)
	{
		for (auto& inst : m_Data)
			inst.write(stream);
	}
}
