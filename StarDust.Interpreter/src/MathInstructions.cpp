#include "MathInstructions.h"
#include "ExecutionScope.h"

namespace sdi
{
    std::shared_ptr<InstructionVariant> AddInstructionDefinition::create_variant(InstructionArguments* arguments)
    {
        return std::make_shared<AddInstruction>();
    }

    std::shared_ptr<InstructionVariant> SubInstructionDefinition::create_variant(InstructionArguments* arguments)
    {
        return std::make_shared<SubInstruction>();
    }

    int AddInstruction::execute(ExecutionScope* scope)
    {
        StackValue val1, val2;
        scope->get_data_stack()->pop_into(val1);
        scope->get_data_stack()->pop_into(val2);

        assert(val1.is_int32() && val2.is_int32());

        scope->get_data_stack()->push(val1.as_int32() + val2.as_int32());
        return 0;
    }

    int SubInstruction::execute(ExecutionScope* scope)
    {
        StackValue val1, val2;
        scope->get_data_stack()->pop_into(val1);
        scope->get_data_stack()->pop_into(val2);

        assert(val1.is_int32() && val2.is_int32());

        scope->get_data_stack()->push(val2.as_int32() - val1.as_int32());
        return 0;
    }
}
