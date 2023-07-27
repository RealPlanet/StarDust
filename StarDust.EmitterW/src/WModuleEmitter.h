#pragma once

#include "ModuleEmitter.h"
#include "WMethodEmitter.h"

using namespace System::IO;
using namespace System::Runtime::InteropServices;

namespace StarDust {
	namespace Emitter {
		public ref class ModuleEmitter
		{
			sde::ModuleEmitter* _Ptr;
		public:
			ModuleEmitter(System::String^ moduleName);
			~ModuleEmitter() { this->!ModuleEmitter(); }
			!ModuleEmitter() { _Ptr->dispose(); delete _Ptr; }

			MethodEmitter^ AddMethod(System::String^ methodName);
			void Emit(StreamWriter^ stream);
		};
	}
}



