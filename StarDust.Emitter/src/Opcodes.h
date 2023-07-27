#pragma once
#include "SDShared.h"

namespace sde {
	enum class _SDVM_API Opcode
	{
		NoOperation = -1,

		Add,
		Subtract,
		Multiply,
		Division,

		Call,
		Return,

		UnconditionalJump,
		BranchEquals,

		LoadConstantI32,
		LoadString,
	};
}
