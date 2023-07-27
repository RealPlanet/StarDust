#pragma once
#include "EmitterEntity.h"
#include "Opcodes.h"
#include "Argument.h"

namespace sde {
	class Instruction
		: EmitterEntity {
	private:
		size_t m_Offset;
		Opcode m_Opcode;
		Arguments m_Data;

	public:

		_SDVM_API Opcode opcode() { return m_Opcode; }
		_SDVM_API Arguments& arguments() { return m_Data; }

		// Emitter entity
		virtual void write(std::stringstream* stream) override;
		_SDVM_API virtual void dispose() {}

		_SDVM_API Instruction(size_t offset, Opcode code, Arguments& args)
			: m_Offset{ offset }, m_Opcode{ code }, m_Data{ args }
		{}

		_SDVM_API Instruction(Instruction&& other) noexcept {
			m_Offset = other.m_Offset;
			m_Opcode = other.m_Opcode;
			m_Data = std::move(other.m_Data);
		}

		_SDVM_API Instruction(const Instruction& other) noexcept {
			m_Offset = other.m_Offset;
			m_Opcode = other.m_Opcode;
			m_Data.copy_from(other.m_Data);
		}

		_SDVM_API virtual ~Instruction() {}

		_SDVM_API void operator=(const Instruction& other) {
			m_Offset = other.m_Offset;
			m_Opcode = other.m_Opcode;
			m_Data = other.m_Data;
		}
	};

	class Instructions
		: public Container<Instruction>, EmitterEntity

	{
	public:
		_SDVM_API Instruction& at(size_t i) { return m_Data.at(i); }
		virtual void add(Instruction inst) { m_Data.push_back(inst); }

		virtual void dispose() override;
		virtual void write(std::stringstream* stream) override;
	};
}
