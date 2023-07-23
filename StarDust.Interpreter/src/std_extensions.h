#pragma once

#ifndef _SDVM_STD_EXTENSIONS_H_
#define _SDVM_STD_EXTENSIONS_H_

#include "Utility.h"
#include "InstructionDefinition.h"
#include <string>

namespace std
{
	template<>
	struct hash<sdi::InstructionArguments>
	{
		constexpr size_t operator()(const sdi::InstructionArguments& p) const {
			size_t seed(0);

			return seed;
		}
	};

	template<typename T1, typename T2>
	struct hash<std::pair<T1, T2>>
	{
		constexpr size_t operator()(const pair<T1, T2>& p) const {
			size_t seed(0);
			::sdi::util::hash_combine(seed, p.first);
			::sdi::util::hash_combine(seed, p.second);
			return seed;
		}
	};
}

#endif // !_SDVM_STD_EXTENSIONS_H_
