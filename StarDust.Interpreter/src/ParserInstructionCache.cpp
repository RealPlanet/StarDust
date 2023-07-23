#include "stdafx.h"

#include "ParserInstructionCache.h"
#include "InstructionVariant.h"

namespace sdi
{
    InstructionVariantPtr ParserInstructionCache::try_get_variant(const std::string& instructionType, InstructionArguments& args)
    {
        auto key = std::make_pair(instructionType, args);
        if (theCachedInstructions.find(key) != theCachedInstructions.end()) 
            return theCachedInstructions[key];

        return NULL;
    }

    InstructionVariantPtr ParserInstructionCache::create_variant(std::shared_ptr<InstructionDefinition> def, InstructionArguments& args)
    {
        if (def == NULL)
            return NULL;

        auto ptr = try_get_variant(def->keyword(), args);
        if (ptr != NULL)
            return ptr;

        ptr = def->create_variant(&args);
        auto key = std::make_pair(def->keyword(), args);

        theCachedInstructions.insert(std::make_pair(key, ptr));
        return ptr;
    }
}

