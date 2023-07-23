#pragma once
#include <MethodEmitter.h>
#include "Opcodes.h"

using namespace System;

namespace StarDust{
	namespace Emitter {
		public ref class MethodEmitterW
		{
		private:
			sde::MethodEmitter* theNativeEmitter = NULL;
		public:
			MethodEmitterW(sde::MethodEmitter* native)
				: theNativeEmitter{ native } {}

			~MethodEmitterW() { this->!MethodEmitterW(); }
			!MethodEmitterW() {}

		public:
			property String^ Name
			{
				String^ get() { return gcnew String(theNativeEmitter->name()); }
			}

			void AppendOpcode(OpcodeW opcode)
			{
				theNativeEmitter->append_opcode((sde::Opcode)opcode, nullptr);
			}
		};
	}
}


