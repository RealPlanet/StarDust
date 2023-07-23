#pragma once

#ifndef _SDVM_FUNCTION_H_
#define _SDVM_FUNCTION_H_

#include "stdafx.h"
#include "VirtualMachineConstruct.h"
#include "InstructionSequence.h"
#include "DataStack.h"

namespace sdi
{
	class Function
		: public VirtualMachineConstruct
	{
		friend class BytecodeParser;

	private:
		std::string					theFunctionName;
		size_t						theArgumentNumber;
		std::vector<StackDataType>	theArgumentTypes;
		InstructionSequence			theFunctionBody;


	public:
		const std::string&					function_name()	{ return theFunctionName; }

		InstructionSequence&				body()			{ return theFunctionBody; }
		const std::vector<StackDataType>&	arguments_def()	{ return theArgumentTypes; }
		// Inherited via VirtualMachineConstruct
		virtual const std::string type() override { return typeid(Function).name(); }
	};

	typedef std::shared_ptr<Function> FunctionPtr;
}

#endif // !_SDVM_FUNCTION_H_



