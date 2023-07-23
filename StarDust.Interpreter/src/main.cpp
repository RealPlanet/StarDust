/*#include "stdafx.h"

#include "VirtualMachine.h"
#include "BytecodeParser.h"
#include "RuntimeStructure.h"
#include "RuntimeInstructionBinder.h"

#include "Logger.h"

void _printToConsole(sdi::ExecutionScope* scope)
{
	sdi::StackValue& valToPrint = scope->get_data_stack().top();

	if (valToPrint.is_int32())
	{
		std::cout << valToPrint.as_int32();
		return;
	}
}

int main(void)
{
	sdi::Logger::init(".\\");
	sdi::Logger::set_log_level(sdi::Logger::LogLevel::Verbose2);

	sdi::RuntimeInstructionBinder::bind_function("pTopStackVal", _printToConsole);
	sdi::VirtualMachine vm;
	
	std::vector<std::string> files = { ".\\sample.sds" };
	sdi::BytecodeParser builder(files);

	auto structure = builder.parse();
	if (structure == NULL)
		return -1;

	vm.load(structure);
	vm.run();

	std::cout << "\n";
	vm.diagnostics().dump(sdi::Logger::get_fstream());
	vm.diagnostics().dump(std::cout);

	vm.reset();

	return 0;
}
*/