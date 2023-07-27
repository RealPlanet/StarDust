#pragma once

#ifndef _SDVM_VIRTUAL_MACHINE_H_
#define _SDVM_VIRTUAL_MACHINE_H_

#include "DllWrappers.h"
#include "MachineDiagnostics.h"
#include "Routine.h"
#include "VirtualMemory.h"

namespace sdi
{
	// What is needed?
	//
	//	1 - Entry point
	//	2 - A collection of active routines
	//	3 - Each routine has it's own:
	//		1 - ScopeStack, each scope has:
	//			2 - A reference to the function currently executing
	//			3 - A data stack
	//			When returning the top value of the data stack is taken and pushed on the ScopeStack below the current one.
	//			If the returning stack is tha last one an exit value equal to the top value of the data stack is assigned to the routine.
	//	The runtime machine requests an update from the runtime structure, the runtime structure updates each routine once before returning
	//  control to the vm.
	//  Each 'Update' moves the program counter forward.
	//
	// 
	// FW
	class RuntimeStructure;
	class FunctionRegister;

	/// <summary>
	/// Executes a runtime structure
	/// </summary>
	class  VirtualMachine
	{
	private:
		struct VMData {
			VMData(VirtualMachine* parent)
				: m_Routines{ parent } {}

			std::shared_ptr<RuntimeStructure>	m_pRuntimeStructure;
			MachineDiagnostics					m_RuntimeDiagnostics;
			RoutineMap							m_Routines;
			VirtualMemory						m_Memory;
		} theVmData;

	private:
		bool running() { return !theVmData.m_Routines.empty(); }// TODO -- Add machine flags to keep machine running

	public:	
		_SDVM_API VirtualMachine();
		_SDVM_API void load(std::shared_ptr<RuntimeStructure> structure);

		std::shared_ptr<RuntimeStructure>	structure()		{ return theVmData.m_pRuntimeStructure; }
		RoutineMap&							routines()		{ return theVmData.m_Routines; }
		MachineDiagnostics&					diagnostics()	{ return theVmData.m_RuntimeDiagnostics; }
		RoutineMap&							routine_map()	{ return theVmData.m_Routines; }

		FunctionRegister& loaded_functions();
	public:
		// State control

		_SDVM_API void run();
		_SDVM_API void pause();
		_SDVM_API void reset();
		_SDVM_API void dump_diagnostics(std::ostream& s) { diagnostics().dump(s); }
	};
}

#endif // !_SDVM_VIRTUAL_MACHINE_H_
