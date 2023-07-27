#include "stdafx.h"
#include "LoadConstant.h"

#include "Utility.h"
#include "ExecutionScope.h"

namespace sdi
{
	std::shared_ptr<InstructionVariant> LoadConstant32Definition::create_variant(InstructionArguments* arguments)
	{
		// TODO -- Implement better runtime validation
		assert(arguments != NULL);
		assert(arguments->size() == 1);

		std::string parameter = arguments->at(0);
		assert(util::isnumber(parameter));

		int32_t constantNumber = std::stoi(parameter);
		LoadConstant32Variant* ptr = new LoadConstant32Variant(constantNumber);
		return std::shared_ptr<LoadConstant32Variant>(ptr);
	}

	int LoadConstant32Variant::execute(ExecutionScope* scope)
	{
		scope->get_data_stack()->push(m_iConstant);
		return 0;
	}

	std::shared_ptr<InstructionVariant> LoadConstantStrDefinition::create_variant(InstructionArguments* arguments)
	{
		// TODO -- Implement better runtime validation
		assert(arguments != NULL);
		assert(arguments->size() == 1);

		LoadConstantStrVariant* ptr = new LoadConstantStrVariant(arguments->at(0));

		return std::shared_ptr<InstructionVariant>(ptr);
	}

	int LoadConstantStrVariant::execute(ExecutionScope* scope)
	{
		scope->get_data_stack()->push(m_sConstant);
#ifdef  _DEBUG
		if (m_sConstant.compare(scope->get_data_stack()->top().as_string()) != 0)
			__debugbreak();
#endif //  DEBUG

		return 0;
	}
}


