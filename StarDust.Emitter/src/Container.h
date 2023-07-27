#pragma once

#ifndef _SDVM_CONTAINER_
#define _SDVM_CONTAINER_

#include "SDShared.h"
#include <vector>
#include <initializer_list>

namespace sde {
	template <class T>
	class Container
	{
	protected:
		std::vector<T> m_Data;
	public:
		_SDVM_API Container() {}
		_SDVM_API Container(std::initializer_list<T> list)
			: m_Data{ list } {}
		
		_SDVM_API size_t size() { return m_Data.size(); }
		_SDVM_API void clear() { m_Data.clear(); }
	};
}

#endif // !_SDVM_CONTAINER_


