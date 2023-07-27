#pragma once

#include "Opcodes.h"
#include "Attribute.h"

namespace sde {
	namespace utility {
		// Opcode to string
		_SDVM_API const char* otos(Opcode opcode);
		// Attribute to string
		_SDVM_API const char* atos(AttributeCode opcode);
	}
}

