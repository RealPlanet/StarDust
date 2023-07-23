#pragma once

#ifndef _SDVM_FUNCTION_REGISTER_H_
#define _SDVM_FUNCTION_REGISTER_H_

#include "Function.h"

namespace sdi
{
	/// <summary>
	/// Holds all registered functions within a virtual machine.
	/// Lookup performs no checks, as at runtime, it is expected to be running valid bytecode
	/// </summary>
	class FunctionRegister
	{
	private:
		std::unordered_map<std::string, std::shared_ptr<Function>> m_Functions;
	public:
		FunctionRegister() {}

		const std::shared_ptr<Function> get_function(std::string& fullName) const { return m_Functions.at(fullName); }
		void insert_function(std::shared_ptr<Function> funcDev) { m_Functions.insert(std::make_pair(funcDev->function_name(), funcDev)); }

		inline void		clear() { m_Functions.clear(); }
		inline size_t	size() { return m_Functions.size(); }

		inline std::unordered_map<std::string, std::shared_ptr<Function>>::iterator begin() { return m_Functions.begin(); }
		inline std::unordered_map<std::string, std::shared_ptr<Function>>::iterator end() { return m_Functions.end(); }
	};
}

#endif // !_SDVM_FUNCTION_REGISTER_H_
