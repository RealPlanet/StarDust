#pragma once


#ifndef _SDVM_RUNTIMEOBJECT_H_
#define _SDVM_RUNTIMEOBJECT_H_

#include "Attributes.h"
#include "stdafx.h"

namespace sdi
{
	class __declspec(novtable) VirtualMachineConstruct
	{
		friend class BytecodeParser; // Allows the parser special access to build the runtime data
	protected:
		Attributes m_Attributes;
	public:
		virtual const std::string type() = 0;

		const Attributes& attributes() const { return m_Attributes; }
		size_t	attribute_count() const { return m_Attributes.size(); }
	};
}

#endif // !_SDVM_RUNTIMEOBJECT_H_




