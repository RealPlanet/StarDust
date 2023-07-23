#include "BytecodeFile.h"
#include "Utility.h"

namespace sdi
{
	void BytecodeFile::add_line(BytecodeFile* obj, std::string& line)
	{
		sdi::util::trim(line);
		if (line.empty() || line.rfind(_SDVM_COMMENT_PREFIX, 0) == 0)
			return;

		obj->m_strText += line + '\n';
	}

	BytecodeFilePtr BytecodeFile::from(std::string filePath)
	{
		if (filePath.empty())
			return nullptr;

		std::ifstream file;
		file.open(filePath, std::fstream::in);

		if (!file.is_open())
		{
			char msg[128] = { 0 };
			sprintf_s(msg, 128, "Could not open file '%s'", filePath.c_str());
			throw std::runtime_error(msg);
		}

		BytecodeFile* srcFile = new BytecodeFile();
		srcFile->m_strFullName = filePath;

		for (std::string line; getline(file, line);)
		{
			add_line(srcFile, line);
		}

		return BytecodeFilePtr(srcFile);
	}

	BytecodeFilePtr BytecodeFile::froms(std::string fileContents)
	{
		std::stringstream ss(fileContents);
		BytecodeFile* srcFile = new BytecodeFile();

		for (std::string line; getline(ss, line);)
		{
			add_line(srcFile, line);
		}

		return BytecodeFilePtr(srcFile);
	}
}
