
#include "Attribute.h"
#include "Utility.h"

namespace sde {
	void Attributes::dispose()
	{
		this->clear();
	}

	void Attributes::write(std::stringstream* stream)
	{
		for (auto& attribute : m_Data)
			attribute.write(stream);
	}

	void Attribute::dispose()
	{

	}

	void Attribute::write(std::stringstream* stream)
	{
		*stream << _SDVM_TAB << _SDVM_ATTR_MARKER << utility::atos(m_Code);

		if (m_Arguments.size() > 0) {
			*stream << _SDVM_TOKEN_OPEN_PARENTHESIS << _SDVM_GENERIC_NEWLINE;
			for (size_t x{0}; x < m_Arguments.size(); x++)
				*stream << m_Arguments.at(x)<< _SDVM_GENERIC_NEWLINE;

			*stream << _SDVM_GENERIC_NEWLINE << _SDVM_TOKEN_CLOSE_PARENTHESIS;
		}

		*stream << _SDVM_GENERIC_NEWLINE;
	}
}
