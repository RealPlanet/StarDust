#pragma once

#ifndef _SDVM_PARSER_INSTRUCTION_CACHE_H_
#define _SDVM_PARSER_INSTRUCTION_CACHE_H_

#include "InstructionDefinition.h"

#include "Utility.h"
#include "std_extensions.h"

namespace sdi
{
	typedef std::pair<std::string, InstructionArguments> InstructionCacheKey;
	class BytecodeParser;
	class ParserInstructionCache
	{
	private:
		std::unordered_map<InstructionCacheKey, InstructionVariantPtr> theCachedInstructions;
		BytecodeParser* theParser;

	public:
		ParserInstructionCache(BytecodeParser* parserPtr)
			: theParser{ parserPtr } {}

		std::shared_ptr<InstructionVariant> try_get_variant(const std::string& instructionType, InstructionArguments& args);
		std::shared_ptr<InstructionVariant> create_variant(std::shared_ptr<InstructionDefinition> def, InstructionArguments& args);

		// Drops the cache from memory
		void clear() { theCachedInstructions.clear(); }
	};
}
#endif // !_SDVM_PARSER_INSTRUCTION_CACHE_H_




