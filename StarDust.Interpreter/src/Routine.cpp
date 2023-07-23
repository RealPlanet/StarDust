#include "stdafx.h"
#include "Logger.h"

#include "Routine.h"
#include "ExecutionScope.h"
#include "VirtualMachine.h"

namespace sdi
{
	Routine::Routine(VirtualMachine* parentMachine)
		: theParentVirtualMachine{parentMachine}
	{
	}

	Routine::Routine(VirtualMachine* ptrParentVm, FunctionPtr startFunc)
		: Routine(ptrParentVm, nullptr, startFunc)
	{
	}

	Routine::Routine(VirtualMachine* ptrParentVm, DataStack* arguments, FunctionPtr startFunc)
		: Routine(ptrParentVm)
	{
		theExecutionStack.push(ExecutionScope::create(this, arguments, startFunc));
	}

	RoutinePtr Routine::create(VirtualMachine* ptrParentMachine, const FunctionPtr ptr, DataStack* arguments)
	{
		_SDVM_THROW_IF_NULL(ptrParentMachine);

		return RoutinePtr(new Routine(ptrParentMachine, arguments, ptr));
	}

	void Routine::update()
	{
		_SDVM_LOG("Updating routine!");

		ExecutionScopePtr& scopePtr = theExecutionStack.top();
		scopePtr->update();
	}

	void Routine::exit(StackValue exitValue)
	{
		_SDVM_THROW_IF(!exitValue.is_valid());

		m_Dispose = true;
		theExecutionStack = {}; // Dump all stack data
		theExitValue = exitValue;
	}

	RoutinePtr RoutineMap::new_routine(FunctionPtr startingFunc, DataStack* arguments)
	{
		uint64_t id = 0;

		RoutinePtr newRoutine = Routine::create(m_ptrVirtualMachine, startingFunc, arguments);

		// TODO USE GUIDS
		m_Routines.insert(std::make_pair(startingFunc->function_name().length(), newRoutine));
		return newRoutine;
	}

	RoutineMap::RoutineMap(VirtualMachine* parentVm)
		: m_ptrVirtualMachine{parentVm}
	{
		_SDVM_THROW_IF_NULL(m_ptrVirtualMachine);
	}
}
