#pragma once

#include "SDEmitter.h"
#include "MethodEmitterW.h"
#include <vcclr.h>

using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

namespace StarDust {
	namespace Emitter {
		public ref class SDEmitterW
		{
		private:
			sde::SDEmitter* theNativeEmitter = NULL;
		public:
			SDEmitterW(String^ moduleName) {
				pin_ptr<const wchar_t> wch = PtrToStringChars(moduleName);
				theNativeEmitter = new sde::SDEmitter(wch, moduleName->Length);
			}

			~SDEmitterW() { this->!SDEmitterW(); }
			!SDEmitterW() { delete theNativeEmitter; }

		public:
			void Generate(StreamWriter^ outputStream);
			MethodEmitterW^ EmitMethod(String^ methodName);
		};
	}
}



