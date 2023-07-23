#pragma once

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

	enum class _SDVM_API Attribute
	{
		NoAttribute = -1,

		EntryPoint,
	};
}
