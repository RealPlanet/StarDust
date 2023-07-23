#pragma once

#ifndef _SDVM_BYTECODEFILE_H_
#define _SDVM_BYTECODEFILE_H_

#include "stdafx.h"

namespace sdi
{
	class BytecodeFile;

	typedef std::shared_ptr<BytecodeFile> BytecodeFilePtr;

	class BytecodeFile
	{

	private:
		BytecodeFile() {}

		std::string m_strText;
		std::string m_strFullName;

		static void add_line(BytecodeFile* obj, std::string& line);
	public:
		static _SDVM_API BytecodeFilePtr from(std::string filePath);
		static _SDVM_API BytecodeFilePtr froms(std::string fileContents);
		std::string toString(int start, int length) { return m_strText.substr(start, length); }
		std::string& fullname() { return m_strFullName; }
		size_t size() { return m_strText.size(); }

		char _SDVM_API operator[](int i) { return m_strText[i]; }
	};
}

#endif // !_SDVM_BYTECODEFILE_H_
