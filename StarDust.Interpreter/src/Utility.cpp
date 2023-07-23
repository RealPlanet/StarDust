#include "stdafx.h"

#include "Utility.h"

namespace sdi
{
	namespace util
	{
		void ltrim(std::string& s)
		{
			s.erase(s.begin(), std::find_if(s.begin(), s.end(), [](unsigned char ch) {
				return !std::isspace(ch);
				}));
		}

		void rtrim(std::string& s)
		{
			s.erase(std::find_if(s.rbegin(), s.rend(), [](unsigned char ch) {
				return !std::isspace(ch);
				}).base(), s.end());
		}

		void trim(std::string& s)
		{
			ltrim(s);
			rtrim(s);
		}

		bool isnewline(char c)
		{
			return (c == '\n' || c == '\r' || c == '\t');
		}

		bool isnumber(const std::string& s)
		{
			return !s.empty() && std::find_if(s.begin(),
				s.end(), [](unsigned char c) { return !std::isdigit(c); }) == s.end();
		}

	}
}
