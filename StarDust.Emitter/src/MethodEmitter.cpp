#include "stdafx.h"
#include "MethodEmitter.h"
#include "SDShared.h"
#include "SDEmitter.h"

namespace sde
{
	void MethodEmitter::write(std::stringstream* stream)
	{
		*stream << _SDVM_ATTR_MARKER << _SDVM_CONSTRUCT_METHOD << " void " << theMethodName;
		*stream << "()" << _SDVM_GENERIC_NEWLINE << '{' << _SDVM_GENERIC_NEWLINE;
		// TODO -- support arguments

		for (auto& line : theMethodAttributes)
			*stream << line << _SDVM_GENERIC_NEWLINE;

		for (auto& inst : theMethodInstructions)
			inst.write(stream);

		*stream << '}' << _SDVM_GENERIC_NEWLINE<< _SDVM_GENERIC_NEWLINE;
	}

	MethodEmitter* MethodEmitter::append_opcode(Opcode id, std::vector<std::string>* vecArguments)
	{
		theMethodInstructions.emplace_back(theLabelIndex++, id, *vecArguments);
		return this;
	}

	MethodEmitter* MethodEmitter::add_attribute(Attribute attribute, std::vector<std::string>* vecArguments)
	{
		std::string strAttribute(atos(attribute));
		std::stringstream ss;
		ss << _SDVM_TAB << _SDVM_ATTR_MARKER << strAttribute;

		if (vecArguments != nullptr && vecArguments->size() > 0)
		{
			ss << _SDVM_TOKEN_OPEN_PARENTHESIS << _SDVM_GENERIC_NEWLINE;
			for (auto& arg : *vecArguments)
			{
				ss << arg << _SDVM_GENERIC_NEWLINE;
			}

			ss << _SDVM_GENERIC_NEWLINE << _SDVM_TOKEN_CLOSE_PARENTHESIS;
		}
		theMethodAttributes.push_back(ss.str());
		return this;
	}
}


