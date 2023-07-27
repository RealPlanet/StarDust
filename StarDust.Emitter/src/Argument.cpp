
#include "Argument.h"
#include <vector>

namespace sde {
	void Arguments::copy_from(const Arguments& other)
	{
		for (auto& l : other.m_Data)
			m_Data.push_back(l);
	}
}



