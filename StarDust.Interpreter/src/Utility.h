#pragma once
#ifndef _SDVM_UTILITY_H_
#define _SDVM_UTILITY_H_

namespace sdi
{
	namespace util
	{
		template<typename T>
		void hash_combine(size_t& seed, T const& key)
		{
			std::hash<T> hasher;
			seed ^= hasher(key) + 0x9e3779b9 + (seed << 6) + (seed >> 2);
		}

		void ltrim(std::string& s);

		void rtrim(std::string& s);

		void trim(std::string& s);

		bool isnewline(char c);

		bool isnumber(const std::string& s);

		template<typename T>
		T ston(std::string& s)
		{
			assert(util::isnumber(s));
			return (T)std::stoi(s);
		}
	}
}

#endif // !_SDVM_UTILITY_H_
