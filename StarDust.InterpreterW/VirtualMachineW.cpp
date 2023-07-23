#include "VirtualMachineW.h"
#include "BytecodeParser.h"
#include "RuntimeInstructionBinder.h"
using namespace System;
using namespace StarDust::Shared;
using namespace System::Runtime::InteropServices;

namespace StarDust {
	namespace Interpreter {
		void ManagedPrintToConsole(sdi::ExecutionScope* scope)
		{
			sdi::DataStack& stack = scope->get_data_stack();
			sdi::StackValue value = stack.pop();
			System::Console::WriteLine(value.as_int32());
		}

		VirtualMachineW::VirtualMachineW()
		{
			sdi::Logger::init("WrapperVMLog.log");
			theVirtualMachine = new sdi::VirtualMachine();
			sdi::RuntimeInstructionBinder::bind_function("pTopStackVal", ManagedPrintToConsole);
		}

		void VirtualMachineW::DumpDiagnostics(TextWriter^ writer)
		{
			if (writer == nullptr)
				return;
			std::stringstream ss;
			theVirtualMachine->dump_diagnostics(ss);
			writer->Write(gcnew String(ss.str().c_str()));
		}

		void VirtualMachineW::Validate()
		{
			//throw gcnew System::NotImplementedException();
		}

		bool VirtualMachineW::LoadScripts(List<String^>^ scriptNames)
		{
			sdi::BytecodeParser p;

			std::vector<std::string> nativeScriptNames;
			for (int i = 0; i < scriptNames->Count; i++)
			{
				IntPtr cPtr = Marshal::StringToHGlobalAnsi(scriptNames[i]);
				const char* nativeStr = static_cast<const char*>(cPtr.ToPointer());
				sdi::BytecodeFilePtr bcFile = sdi::BytecodeFile::from(nativeStr);
				p.add_bytecode(bcFile);
				nativeScriptNames.push_back(nativeStr);

				Marshal::FreeHGlobal(cPtr);
			}

			sdi::RuntimeStructurePtr ptr = p.parse();
			if (ptr != nullptr)
			{
				theVirtualMachine->load(ptr);
				return true; 
			}

			return false;
		}
	}
}
