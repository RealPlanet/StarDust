#include "ReturnInstruction.h"
#include "ExecutionScope.h"
#include "Routine.h"

namespace sdi
{
    std::shared_ptr<InstructionVariant> ReturnInstructionDefinition::create_variant(InstructionArguments* arguments)
    {
        return std::make_shared<ReturnInstruction>();
    }

    int ReturnInstruction::execute(ExecutionScope* scope)
    {
        Routine* routine = scope->get_routine();

        ExecutionStack& stack = routine->get_execution_stack();

        if (stack.size() == 1)
        {
            // Grab the last value on the data stack and set the exit value
            StackValue val = scope->get_data_stack().pop();
            routine->exit(val);
            return 0;
        }

        ExecutionScopePtr scopeToDispose = std::move(stack.top()); // Claim ownership of the scope
        stack.pop(); // Remove it from the execution scope


        DataStack& otherFuncStack = stack.top()->get_data_stack();
        otherFuncStack.push(scope->get_data_stack().top());

        scopeToDispose.release();
        return 0;
    }
}