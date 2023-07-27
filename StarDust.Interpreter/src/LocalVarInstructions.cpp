#include "LocalVarInstructions.h"

#include "Utility.h"
#include "ExecutionScope.h"
#include "DataStack.h"

namespace sdi
{
    std::shared_ptr<InstructionVariant> LoadLocalDefinition::create_variant(InstructionArguments* arguments)
    {
        // TODO -- Implement better runtime validation
        assert(arguments != NULL);
        assert(arguments->size() == 1);

        std::string parameter = arguments->at(0);
        LoadLocalVariant* instVariant = new LoadLocalVariant(util::ston<int32_t>(parameter));
        return std::shared_ptr<InstructionVariant>(instVariant);
    }

    int LoadLocalVariant::execute(ExecutionScope* scope)
    {
        scope->get_data_stack()->push(scope->get_locals().at(theLocalIndex));
        return 0;
    }

    std::shared_ptr<InstructionVariant> StoreLocalDefinition::create_variant(InstructionArguments* arguments)
    {
        // TODO -- Implement better runtime validation
        assert(arguments != NULL);
        assert(arguments->size() == 1);

        std::string parameter = arguments->at(0);
        StoreLocalVariant* instVariant = new StoreLocalVariant(util::ston<int32_t>(parameter));
        return std::shared_ptr<InstructionVariant>(instVariant);
    }

    int StoreLocalVariant::execute(ExecutionScope* scope)
    {
        StackValue val;
        scope->get_data_stack()->pop_into(val);

        std::vector<StackValue>& v = scope->get_arguments();
        v.emplace(v.begin() + theLocalIndex, val);
        return 0;
    }
}


