#pragma once

namespace StarDust
{
	namespace Emitter {
		public enum class OpcodeW
		{
			NoOperation = -1,

			Add,
			Subtract,
			Multiply,
			Division,

			Call,
			Return,

			LoadConstantI32,
		};
	}
}

