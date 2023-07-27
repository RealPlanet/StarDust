#pragma once
//#include <MethodEmitter.h>
#include "Opcodes.h"

namespace StarDust {
	namespace Emitter {
		public ref class MethodEmitter
		{
		private:
			sde::MethodEmitter* _Ptr;
		public:
			MethodEmitter(sde::MethodEmitter* native)
				: _Ptr{ native } {}

			~MethodEmitter() { this->!MethodEmitter(); }
			!MethodEmitter() { _Ptr->dispose(); delete _Ptr; }

			property System::String^ Name
			{
				System::String^ get() { return gcnew System::String(_Ptr->name()); }
			}

			void AppendOpcode(OpcodeW opcode)
			{
				_Ptr->add_opcode((sde::Opcode)opcode);
			}
		};
	}
}


