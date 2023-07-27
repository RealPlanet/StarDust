#pragma once

#ifndef _SDVM_EMITTER_ENTITY_H_
#define _SDVM_EMITTER_ENTITY_H_

#include "SDShared.h"
#include <sstream>
#include <string>

namespace sde
{
	class _SDVM_API __declspec(novtable) EmitterEntity
	{
	public:
		virtual void dispose() = 0;
		virtual void write(std::stringstream* stream) = 0;
	};
}

#endif // !_SDVM_EMITTER_ENTITY_H_


