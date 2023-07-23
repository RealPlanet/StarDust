#pragma once

#ifndef _SDVM_INTERPRETER_EXPCETION_H_
#define _SDVM_INTERPRETER_EXPCETION_H_

#include <stdexcept>

namespace sdi
{
	namespace exceptions 
	{
		class SDInterpreterException
			: public std::runtime_error
		{
		public:
			SDInterpreterException(const char* what)
				: std::runtime_error(what) {}
		};
	}
}

#endif // !_SDVM_INTERPRETER_EXPCETION_H_

