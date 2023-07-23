#include "stdafx.h"

#include "RuntimeInstructionBinder.h"

namespace sdi
{
	std::map<std::string, void(*)(sdi::ExecutionScope*)> RuntimeInstructionBinder::m_BoundInstructions;

	int RuntimeInstructionBinder::bind_function(std::string funcName, void(*func)(sdi::ExecutionScope*))
	{
		_SDVM_THROW_IF(m_BoundInstructions.find(funcName) != m_BoundInstructions.end());

		m_BoundInstructions.insert(std::make_pair(funcName, func));
		return 0;
	}
}