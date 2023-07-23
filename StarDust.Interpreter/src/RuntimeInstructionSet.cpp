#include "stdafx.h"

#include "RuntimeInstructionSet.h"
#include "Logger.h"

#define REGISTER_INSTRUCTION(_INSTRUCTION_CLASS_)\
{\
std::shared_ptr<##_INSTRUCTION_CLASS_> o = std::make_shared<##_INSTRUCTION_CLASS_>();\
_SDVM_LOGVF("Registering instruction '{}' into VM", o->keyword());\
m_Instructions.insert(std::make_pair(o->keyword(), std::move(o)));\
}\

#include "EmptyInstruction.h"
#include "ReturnInstruction.h"
#include "CallInstruction.h"

#include "LoadConstant.h"
#include "LocalVarInstructions.h"
#include "ArgumentVarInstructions.h"
#include "MathInstructions.h"
#include "BranchInstructions.h"

namespace sdi
{
    RuntimeInstructionSet* RuntimeInstructionSet::m_ptrInstance = NULL;

    RuntimeInstructionSet* RuntimeInstructionSet::get()
    {
        if (m_ptrInstance == NULL)
            m_ptrInstance = new RuntimeInstructionSet();

        return m_ptrInstance;
    }

    RuntimeInstructionSet::RuntimeInstructionSet()
    {
        allocate_instruction_set();
    }

    std::shared_ptr<InstructionDefinition> RuntimeInstructionSet::stod(std::string keyword)
    {
        if (get()->m_Instructions.find(keyword) != get()->m_Instructions.end())
            return get()->m_Instructions[keyword];

        return nullptr;
    }

    void RuntimeInstructionSet::allocate_instruction_set()
    {
        /* Allocate internal instruction set */
        REGISTER_INSTRUCTION(EmptyInstructionDefinition);

        // Math
        REGISTER_INSTRUCTION(AddInstructionDefinition);
        REGISTER_INSTRUCTION(SubInstructionDefinition);

        // Constants
        REGISTER_INSTRUCTION(LoadConstant32Definition);
        REGISTER_INSTRUCTION(LoadConstantStrDefinition);

        // Variable handling    
        REGISTER_INSTRUCTION(LoadLocalDefinition);
        REGISTER_INSTRUCTION(StoreLocalDefinition);
        REGISTER_INSTRUCTION(LoadArgDefinition);
        REGISTER_INSTRUCTION(StoreArgDefinition);

        // Logic
        REGISTER_INSTRUCTION(CallDefinition);
        REGISTER_INSTRUCTION(ReturnInstructionDefinition);

        // Branches
        REGISTER_INSTRUCTION(UnconditionalJumpDefinition);
        REGISTER_INSTRUCTION(BranchEqualsDefinition);
        REGISTER_INSTRUCTION(BranchNotEqualsDefinition);
        REGISTER_INSTRUCTION(BranchGreaterThanDefinition);
        REGISTER_INSTRUCTION(BranchGreaterThanOrEqualDefinition);
        REGISTER_INSTRUCTION(BranchLessThanDefinition);
        REGISTER_INSTRUCTION(BranchLessThanOrEqualDefinition);
    }
}


