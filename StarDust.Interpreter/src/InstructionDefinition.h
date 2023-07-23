#pragma once

#ifndef _SDVM_INSTRUCTION_DEF_H_
#define _SDVM_INSTRUCTION_DEF_H_

#include "stdafx.h"
#include "InstructionVariant.h"
#include "SDInstructions.h"

namespace sdi
{
	class InstructionArguments
	{
		std::vector<std::string> m_Arguments;
	private:
		

	public:
		void add(std::string& arg) { m_Arguments.push_back(arg); }
		void clear() { m_Arguments.clear(); }
		size_t size() { return m_Arguments.size(); }
		std::string& at(int i) { return m_Arguments.at(i); }

		bool equals(const InstructionArguments& other) const { return m_Arguments == other.m_Arguments; }
	};

	inline bool operator==(const InstructionArguments& l, const InstructionArguments& r)
	{
		return l.equals(r);
	}

	/// <summary>
	/// Base definition of a generic instruction
	/// </summary>
	class __declspec(novtable) InstructionDefinition
	{
	private:
		std::string m_sKeyword;
	public:
		const std::string& keyword() { return m_sKeyword; }
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) = 0;

		InstructionDefinition(std::string sKeyword)
			: m_sKeyword{ sKeyword } {}
	};
}

#endif // !_SDVM_INSTRUCTION_DEF_H_






