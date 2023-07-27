#include "TestUtilities.h"

std::string StarDustNativeTest::read_expected_result_from_file(const char* filename)
{
	std::string result;
	std::fstream file;
	file.open(filename);

	for (std::string line; getline(file, line);)
	{
		// Do not include empty lines or comments
		if (line.empty())
			continue;

		result += line + '\n';
	}

	file.close();

	trim(result);
	return result;
}
