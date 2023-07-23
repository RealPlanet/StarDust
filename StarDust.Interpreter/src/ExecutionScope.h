#pragma once

#ifndef _SDVM_EXECUTION_SCOPE_H_
#define _SDVM_EXECUTION_SCOPE_H_

#include "DataStack.h"
#include "Function.h"
#include "SDShared.h"

namespace sdi
{
	class Routine;
	class ExecutionScope
	{
	private:
		Routine*				theParentRoutine;
		FunctionPtr				theExecutingFunction;

#pragma region
		DataStack				theDataStack;
		std::vector<StackValue>	theLocalVariables;
		std::vector<StackValue>	theArguments;

		size_t					theLocalVariableOffset = 0;
		size_t					theCurrentLabel = 0;
#pragma endregion ScopeData


		ExecutionScope(Routine* pRoutine, DataStack* dataStack, FunctionPtr funcToExecute)
			: theParentRoutine{ pRoutine }, theExecutingFunction {funcToExecute}
		{
			validate_arguments(dataStack);
		}

		void validate_arguments(DataStack* argumentsSource);
	public:
		void set_label_number(size_t target) { theCurrentLabel = target; }
		void update();
		DataStack&					get_data_stack()	{ return theDataStack; }
		Routine*					get_routine()		{ return theParentRoutine; }

		std::vector<StackValue>&	get_locals()		{ return theLocalVariables; }
		std::vector<StackValue>&	get_arguments()		{ return theArguments; }

		static std::unique_ptr<ExecutionScope> create(Routine* parent, DataStack* arguments, FunctionPtr startingFunction);
	};

	typedef std::unique_ptr<ExecutionScope> ExecutionScopePtr;
	typedef std::stack<ExecutionScopePtr> ExecutionStack;
}

#endif //!_SDVM_EXECUTION_SCOPE_H_
