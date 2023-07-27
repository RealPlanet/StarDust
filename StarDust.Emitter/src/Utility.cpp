

#include "Utility.h"
#include "SDInstructions.h"

namespace sde {
	namespace utility {
		const char* otos(Opcode opcode)
		{
			switch (opcode)
			{
			case Opcode::NoOperation:
				return _SDVM_NOP_INSTRUCTION_NAME;
			case Opcode::Add:
				return _SDVM_ADD_INSTRUCTION_NAME;
			case Opcode::Subtract:
				return _SDVM_SUB_INSTRUCTION_NAME;
			case Opcode::Multiply:
				throw std::exception();
			case Opcode::Division:
				throw std::exception();
			case Opcode::Call:
				return _SDVM_CALL_INSTRUCTION_NAME;
			case Opcode::Return:
				return _SDVM_RET_INSTRUCTION_NAME;
			case Opcode::UnconditionalJump:
				return _SDVM_JMP_INSTRUCTION_NAME;
			case Opcode::BranchEquals:
				return _SDVM_BE_INSTRUCTION_NAME;
			case Opcode::LoadConstantI32:
				return _SDVM_LDCI4_INSTRUCTION_NAME;
			case Opcode::LoadString:
				return _SDVM_LDCST_INSTRUCTION_NAME;
			}

			return nullptr;
		}

		const char* atos(AttributeCode attribute)
		{
			switch (attribute)
			{
			case AttributeCode::EntryPoint:
				return _SDVM_ATTR_ENTRYPOINT;
			}

			return nullptr;
		}
	}
}
