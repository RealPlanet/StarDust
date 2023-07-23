#include "VirtualMachineStructs.h"

namespace sdi
{
	void RuntimeStruct::validate()
	{
		_SDVM_THROW_IF(m_StructVariables.size() != m_pOriginalDefinition->defined_types().size());

		for (int i = 0; i < m_StructVariables.size(); i++)
		{
			_SDVM_THROW_IF_W_ERR(m_StructVariables[i].tag != m_pOriginalDefinition->defined_types()[i],
				"Type did not match during struct validation");
		}
	}
}

