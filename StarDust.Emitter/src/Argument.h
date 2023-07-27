#pragma once

#ifndef _SDVM_ARGUMENTS_
#define _SDVM_ARGUMENTS_

#include "EmitterEntity.h"
#include "Container.h"

#include "SDShared.h"

namespace sde {

	class Arguments
		: public Container<std::string>
	{
	public:
		_SDVM_API Arguments() {}
		_SDVM_API Arguments(std::initializer_list<std::string> list)
			: Container(list) {}

		_SDVM_API const char*	at(size_t i) { return m_Data.at(i).c_str(); }
		_SDVM_API void			set_at(size_t i, const char* str) { m_Data.at(i) = (str); }
		_SDVM_API void			push_back(const char* str) { return m_Data.push_back(str); }
		void copy_from(const Arguments& other);
	};
}

#endif // !_SDVM_ARGUMENTS_




