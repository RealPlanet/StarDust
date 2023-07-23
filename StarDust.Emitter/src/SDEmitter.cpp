#include "stdafx.h"
#include "SDEmitter.h"
#include "MethodEmitter.h"
#include "SDInstructions.h"

namespace sde
{
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

	const char* atos(Attribute attribute)
	{
		switch (attribute)
		{
		case Attribute::EntryPoint:
			return _SDVM_ATTR_ENTRYPOINT;
		}

		return nullptr;
	}

	SDEmitter::SDEmitter(const char* moduleName, size_t len)
	{
		theModuleName.assign(moduleName, len);
	}

	SDEmitter::SDEmitter(const wchar_t* moduleName, size_t len)
	{
		std::wstring ws(moduleName, len);
		theModuleName = std::string(ws.begin(), ws.end());
	}

	MethodEmitter* SDEmitter::emit_method(const char* methodName, size_t len)
	{
		std::string mName;
		MethodEmitter* methodEmitter = new MethodEmitter(this, methodName, len);

		theAllocatedMethods.insert(std::make_pair(mName, methodEmitter));
		theAllocatedEntities.push_back(methodEmitter);
		return methodEmitter;
	}

	MethodEmitter* SDEmitter::emit_method(const wchar_t* methodName, size_t len)
	{
		std::wstring ws(methodName, len);
		std::string s = std::string(ws.begin(), ws.end());
		return emit_method(s.c_str(), s.length());
	}
}

