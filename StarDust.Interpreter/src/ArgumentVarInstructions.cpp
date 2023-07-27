#include "ArgumentVarInstructions.h"

#include "ExecutionScope.h"
#include "Utility.h"

namespace sdi
{
	std::shared_ptr<InstructionVariant> LoadArgDefinition::create_variant(InstructionArguments* arguments)
	{
		// TODO -- Implement better runtime validation
		assert(arguments != NULL);
		assert(arguments->size() == 1);

		std::string parameter = arguments->at(0);
		LoadArgVariant* instVariant = new LoadArgVariant(util::ston<int32_t>(parameter));
		return std::shared_ptr<InstructionVariant>(instVariant);
	}

	int LoadArgVariant::execute(ExecutionScope* scope)
	{
		DataStack* stack = scope->get_data_stack();
		StackValue& value = scope->get_arguments().at(theArgIndex);
		stack->push(value);
		return 0;
	}

	std::shared_ptr<InstructionVariant> StoreArgDefinition::create_variant(InstructionArguments* arguments)
	{
		// TODO -- Implement better runtime validation
		assert(arguments != NULL);
		assert(arguments->size() == 1);

		std::string parameter = arguments->at(0);
		StoreArgVariant* instVariant = new StoreArgVariant(util::ston<int32_t>(parameter));
		return std::shared_ptr<InstructionVariant>(instVariant);
	}

	int StoreArgVariant::execute(ExecutionScope* scope)
	{
		StackValue val;
		scope->get_data_stack()->pop_into(val);
		std::vector<StackValue>& v = scope->get_arguments();
		v.emplace(v.begin() + theArgIndex, val);
		return 0;
	}
}


