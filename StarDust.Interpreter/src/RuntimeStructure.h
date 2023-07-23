#pragma once

#ifndef _SDVM_RUNTIME_STRUCTURE_H_
#define _SDVM_RUNTIME_STRUCTURE_H_

#include "FunctionRegister.h"

namespace sdi
{
	/// <summary>
	/// The brain of the star dust virtual machine.
	/// 
	/// Holds all runtime data
	/// 
	/// </summary>
	class RuntimeStructure
	{
		friend class BytecodeParser; // Allows the parser special access to build the structure at load time
	private:
		FunctionRegister m_Functions;
	public:
		RuntimeStructure() {};

		int append(RuntimeStructure* other);
		void reset();

		FunctionRegister& functions()			  { return m_Functions; }
		const FunctionRegister& functions() const { return m_Functions; }
	};

	typedef std::shared_ptr<RuntimeStructure>	RuntimeStructurePtr;
}

#endif // !_SDVM_RUNTIME_STRUCTURE_H_
