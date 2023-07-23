#pragma once

#include "VirtualMachine.h"
#include "Logger.h"

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;

namespace StarDust {
	namespace Interpreter {
		public ref class VirtualMachineW
		{
		private:
			sdi::VirtualMachine* theVirtualMachine;
		public:
			VirtualMachineW();
			!VirtualMachineW() { delete theVirtualMachine;  theVirtualMachine = NULL; }
			virtual ~VirtualMachineW() { this->!VirtualMachineW(); }

			void Run() { theVirtualMachine->run(); }
			void Pause() { theVirtualMachine->pause(); }
			void Reset() { theVirtualMachine->reset(); }

			void DumpDiagnostics(TextWriter^ writer);
			void Validate();
			bool LoadScripts(List<String^>^ scriptNames);
		};
	}
}

