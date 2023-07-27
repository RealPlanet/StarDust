#pragma once
#include "EmitterEntity.h"
#include "Opcodes.h"

#include <string>
#include <vector>
#include <map>
#include <fstream>

namespace sde {
	class Emitter 
		: public EmitterEntity
	{
	protected:
	public:
		_SDVM_API Emitter() {}
		_SDVM_API virtual ~Emitter() {}
	};
}

