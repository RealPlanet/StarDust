#pragma once
#include "EmitterEntity.h"
#include "Opcodes.h"
#include <vector>

namespace sde {
	class Instruction
		: EmitterEntity {
	private:
		size_t m_Offset;
		Opcode m_Opcode;
		std::vector<std::string> m_Data;

	public:

		_SDVM_API Opcode opcode() { return m_Opcode; }
		_SDVM_API std::vector<std::string>& arguments() { return m_Data; }

		// Emitter entity
		virtual void write(std::stringstream* stream) override;
		_SDVM_API virtual void dispose() {}

		_SDVM_API Instruction(size_t offset, Opcode code, std::vector<std::string>& args)
			: m_Offset{ offset }, m_Opcode{ code }, m_Data{ args }
		{}

		_SDVM_API Instruction(Instruction&& other) noexcept {
			m_Offset = other.m_Offset;
			m_Opcode = other.m_Opcode;
			m_Data = std::move(other.m_Data);
		}

		_SDVM_API virtual ~Instruction() {}

		_SDVM_API void operator=(const Instruction& other) {
			m_Offset = other.m_Offset;
			m_Opcode = other.m_Opcode;
			m_Data = other.m_Data;
		}
	};
}
