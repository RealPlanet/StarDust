#include "stdafx.h"

#include "VirtualMachine.h"
#include "RuntimeStructure.h"

namespace sdi
{
	VirtualMachine::VirtualMachine() 
		: theVmData{this}
	{
	}

	void VirtualMachine::load(std::shared_ptr<RuntimeStructure> newStructure)
	{
		if (structure() == NULL)
		{
			theVmData.m_pRuntimeStructure = newStructure;
			return;
		}

		theVmData.m_pRuntimeStructure->append(newStructure.get());
	}

	FunctionRegister& VirtualMachine::loaded_functions()
	{
		return structure()->functions();
	}

	void VirtualMachine::run()
	{
		diagnostics().log("The sd machine is starting execution...");

		if (structure() == NULL)
		{
			diagnostics().log("The sd machine does not have a runtime structure loaded...");
			return;
		}

		for (auto& func : structure()->functions())
		{
			if (!func.second->attributes().has_attribute(AttributeType::EntryPoint))
				continue;

			routines().new_routine(func.second);
		}

		int32_t exitValue = -1;
		while (this->running()) 
		{
			for (auto it = routines().begin(); it != routines().end(); it++)
			{
				RoutinePtr pRoutine = it->second;
				pRoutine->update();

				if (pRoutine->awaiting_disposal())
				{
					// We are disposing the very last routine
					if (routines().size() == 1)
					{
						const StackValue& val = pRoutine->get_exit_value();
						_SDVM_THROW_IF(!val.is_int32());
						exitValue = val.as_int32();
						diagnostics().log("All routines are finished!");
					}

					diagnostics().log("Disposing routine...");
					it = routines().erase(it);	
				}

				if (it == routines().end())
					break;
			}
		}

		diagnostics().log(sdu::format("Virtual machine exiting with code: {}", exitValue));
	}

	void VirtualMachine::pause()
	{
		diagnostics().log("The sd machine is pausing execution...");
	}

	void VirtualMachine::reset()
	{
		theVmData.m_pRuntimeStructure.reset();
		diagnostics().reset();
	}
}

