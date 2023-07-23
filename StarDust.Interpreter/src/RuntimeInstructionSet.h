#pragma once

#ifndef _SDVM_RUNTIMEINSTRUCTIONSET_H_
#define _SDVM_RUNTIMEINSTRUCTIONSET_H_

namespace sdi
{
	class InstructionDefinition;
	class RuntimeInstructionSet
	{
	private:
		std::map<std::string, std::shared_ptr<InstructionDefinition>> m_Instructions;
		static RuntimeInstructionSet* m_ptrInstance;
		static RuntimeInstructionSet* get();

	private:
		RuntimeInstructionSet();
		void allocate_instruction_set();

	public:
		/// <summary>
		/// String to Definition
		/// </summary>
		static std::shared_ptr<InstructionDefinition> stod(std::string keyword);
	};
}

#endif // !_SDVM_RUNTIMEINSTRUCTIONSET_H_




