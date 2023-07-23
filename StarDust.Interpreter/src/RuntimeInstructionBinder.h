#pragma once
#ifndef _SDVM_RETURNINSTRUCTION_H_
#define _SDVM_RETURNINSTRUCTION_H_


namespace sdi
{
	class ExecutionScope;
	class RuntimeInstructionBinder
	{
	private:
		static std::map<std::string, void(*)(sdi::ExecutionScope*)> m_BoundInstructions;

	public:
		static _SDVM_API int bind_function(std::string funcName, void(*func)(sdi::ExecutionScope*));
		static _SDVM_API bool exists(std::string funcName) { return m_BoundInstructions.find(funcName) != m_BoundInstructions.end(); }
		static void invoke_builtin(std::string& funcName, sdi::ExecutionScope* scope) { m_BoundInstructions[funcName](scope); }
	};
}


#endif
