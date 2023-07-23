#include "stdafx.h"

#include "EmptyInstruction.h"
#include "ExecutionScope.h"

#include "Logger.h"

namespace sdi
{
    std::shared_ptr<InstructionVariant> EmptyInstructionDefinition::create_variant(InstructionArguments* arguments)
    {
        if (arguments->size() > 0)
            _SDVM_LOGE("Empty instruction does not support arguments, this most likely broke the cache system!!");

        return std::make_shared<EmptyInstructionVariant>();
    }

    int EmptyInstructionVariant::execute(ExecutionScope* scope)
    {
        return 0;
    }
}


