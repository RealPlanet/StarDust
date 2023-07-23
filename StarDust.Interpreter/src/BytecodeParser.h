#pragma once

#ifndef _SDVM_BYTECODE_PARSER_H_
#define _SDVM_BYTECODE_PARSER_H_

#include "ParserInstructionCache.h"
#include "BytecodeFile.h"
#include "RuntimeStructure.h"

namespace sdi
{
	/// <summary>
	/// Constructs 
	/// </summary>
	enum class RuntimeConstructs
	{
		Unknown,
		Method
	};

	class BytecodeParser
	{
	public:
		enum class _SDVM_API ParserErrorValue
		{
			GenericError = 0x0,
			UnknownInstructionError,
			UnknownCharacterError,
			CharacterMatchError,
			UnknownTypeError,

			NoError,
		};

	private:
		std::map<std::string, BytecodeFilePtr>  theBytecodeFiles;
		ParserInstructionCache					theInstructionCache;
		BytecodeFilePtr							theCurrentFile = NULL;
		RuntimeStructurePtr						theOutputStructure = NULL;
		ParserErrorValue						theErrorValue = ParserErrorValue::NoError;
	private:
		int theCurrentIndex = 0;

		char peek(int offset);
		char current() { return peek(0); }
		void skipwspace(bool incNewline = true);
		void consume_trivia();
		void tonextline();
		bool consume_c(char c);
	public:
		_SDVM_API BytecodeParser();

		_SDVM_API BytecodeParser(BytecodeParser&& other);
		_SDVM_API void add_bytecode(BytecodeFilePtr bytecode);

		/// <summary>
		/// Parse all the currently loaded source data
		/// </summary>
		_SDVM_API std::shared_ptr<RuntimeStructure> parse();
		_SDVM_API ParserErrorValue get_err_value() { return theErrorValue; }
		void reset();
		static RuntimeConstructs string_to_construct(std::string& str);
	private:
		

		void parse_file();
		void parse_bytecode_construct();
		void parse_method_construct();
		void parse_method_arguments(Function* pFunc);
		void parse_method_body(Function* pFunc);
		Attributes parse_attributes();

		std::string read_identifier();
		std::string read_instruction();
		void read_instruction_arguments(InstructionArguments* args);
	};
}

#endif // !_SDVM_BYTECODE_PARSER_H_






