#include "CallInstruction.h"
#include "RuntimeInstructionBinder.h"
#include "ExecutionScope.h"
#include "Routine.h"
#include "VirtualMachine.h"
#include "FunctionRegister.h"

namespace sdi
{
	std::shared_ptr<InstructionVariant> CallDefinition::create_variant(InstructionArguments* arguments)
	{
		// Func call can be either a standard jump OR a threaded call!
		_SDVM_THROW_IF(arguments->size() != 1 && arguments->size() != 2);

		std::string& funcName = arguments->at(0);
		bool builtIn = RuntimeInstructionBinder::exists(funcName);
		FunctionPtr funcToCall = nullptr;
		if (!builtIn)
		{
			// TODO :: Check if func is already registered to avoid runtime overhead!
		}

		CallInstruction* instruction = new CallInstruction();
		instruction->m_bIsBuiltinCall = builtIn;
		instruction->m_bIsFakeThread = !builtIn && arguments->size() >= 2 && arguments->at(1).compare("thread") == 0;

		instruction->theFunctionToCall = funcToCall;
		instruction->m_strFunctionName = funcName;
		return std::shared_ptr<InstructionVariant>(instruction);
	}

	int CallInstruction::execute(ExecutionScope* scope)
	{
		if (m_bIsBuiltinCall)
		{
			RuntimeInstructionBinder::invoke_builtin(m_strFunctionName, scope);
			return 0;
		}

		if (theFunctionToCall == nullptr)
		{
			const FunctionRegister& allFunctions = scope->get_routine()->parent_machine()->loaded_functions();
			theFunctionToCall = allFunctions.get_function(m_strFunctionName);
		}


		Routine* pRoutine = scope->get_routine();
		if (m_bIsFakeThread)
		{
			pRoutine->parent_machine()->routine_map().new_routine(theFunctionToCall, scope->get_data_stack());
			return 0;
		}

		ExecutionStack* stack = pRoutine->get_execution_stack();
		stack->push(ExecutionScope::create(pRoutine, scope->get_data_stack(), theFunctionToCall));
		return 0;
	}
}