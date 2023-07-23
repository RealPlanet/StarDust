#pragma once

#ifndef _SDVM_ROUTINE_H_
#define _SDVM_ROUTINE_H_

#include "ExecutionScope.h"
#include "Function.h"

namespace sdi
{
	class VirtualMachine;
	class Routine
	{
		friend class RoutineMap;
	private:
		VirtualMachine*		theParentVirtualMachine;
		ExecutionStack		theExecutionStack;
		StackValue			theExitValue;

		bool m_Dispose = false;

		Routine(VirtualMachine* parentMachine);
		Routine(VirtualMachine* parentMachine, FunctionPtr startFunc);
		Routine(VirtualMachine* parentMachine, DataStack* arguments, FunctionPtr startFunc);
	public:
		VirtualMachine* parent_machine() const { return theParentVirtualMachine; }

		bool awaiting_disposal() { return m_Dispose; }
		void update();
		void exit(StackValue exitValue);

		ExecutionStack& get_execution_stack() { return theExecutionStack; }
		const StackValue& get_exit_value() const { _SDVM_THROW_IF(!theExitValue.is_valid()); return theExitValue; }

	public:
		static std::shared_ptr<Routine> create(VirtualMachine* ptrParentMachine, const FunctionPtr ptr, DataStack* arguments = nullptr);
	};

	typedef std::shared_ptr<Routine> RoutinePtr;

	class RoutineMap
	{
		VirtualMachine* m_ptrVirtualMachine;
		std::map<uint64_t, RoutinePtr> m_Routines;

	public:
		RoutineMap(VirtualMachine* parentVm);

		std::map<uint64_t, RoutinePtr>::iterator begin()	{ return m_Routines.begin(); }
		std::map<uint64_t, RoutinePtr>::iterator end()		{ return m_Routines.end(); }
		std::map<uint64_t, RoutinePtr>::iterator erase(std::map<uint64_t, RoutinePtr>::iterator it) { return m_Routines.erase(it); }
		bool empty() { return m_Routines.empty(); }
		size_t size() { return m_Routines.size(); }

		RoutinePtr new_routine(FunctionPtr startingFunc, DataStack* arguments = nullptr);
	};
}

#endif // !_SDVM_ROUTINE_H_



