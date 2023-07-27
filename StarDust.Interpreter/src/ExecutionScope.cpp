#include "ExecutionScope.h"
#include "Routine.h"
#include "DataStack.h"
namespace sdi
{
	void ExecutionScope::validate_arguments(DataStack* argumentsSource)
	{
		auto& arguments = theExecutingFunction->arguments_def();
		size_t numOfArguments = arguments.size();

		if (numOfArguments > 0 &&
			(argumentsSource == NULL || argumentsSource->size() < numOfArguments))
			throw std::runtime_error("Not enough arguments!");

		for (auto& def : arguments)
		{
			if(def != argumentsSource->top().tag)
				throw std::runtime_error("Unexpected argument type!");

			StackValue data = {};
			argumentsSource->pop_into(data);
			theArguments.push_back(data);
		}
	}

	
	ExecutionScopePtr ExecutionScope::create(Routine* parent, DataStack* arguments, FunctionPtr startingFunction)
	{
		if (parent == NULL || startingFunction == NULL)
			throw std::runtime_error("Invalid input!");

		return ExecutionScopePtr(new ExecutionScope(parent, arguments, startingFunction));
	}

	void ExecutionScope::update()
	{
		InstructionSequence& allInstructions = theExecutingFunction->body();
		InstructionVariantPtr inst = allInstructions.instruction_at_label(theCurrentLabel++);

		inst->execute(this);
	}
}


