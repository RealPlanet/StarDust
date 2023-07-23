#include "pch.h"
#include "CppUnitTest.h"

#include "SDEmitter.h"
#include "TestUtilities.h"
#include "Opcodes.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define VALIDATE_EMITTER() assert_result(&emitter, __func__)

namespace StarDustNativeTest
{
	TEST_CLASS(StarDust_EmitterTest)
	{
	public:
		
		static void assert_result(sde::SDEmitter* emitter, std::string testName)
		{
			std::stringstream ss;
			emitter->generate(&ss);

			std::string output = ss.str();
			trim(output);

			std::string fileLocation;
			fileLocation.append(".\\Data\\");
			fileLocation.append(testName);
			fileLocation.append(".txt");

			std::string expected = read_expected_result_from_file(fileLocation.c_str());
			Assert::AreEqual(expected, output);
		}

		TEST_METHOD(TestEmitMethod)
		{
			std::string moduleNameTest = "TEST";
			std::string methodNameTest = "Main";
			sde::SDEmitter emitter(moduleNameTest.c_str(), moduleNameTest.length());

			sde::MethodEmitter* method = emitter.emit_method(methodNameTest.c_str(), methodNameTest.length());

			Assert::IsTrue(emitter.number_of_methods() == 1);
			VALIDATE_EMITTER();
		}

		TEST_METHOD(TestEmitMethodSum)
		{
			std::string moduleNameTest = "TEST";
			std::string methodNameTest = "Main";
			sde::SDEmitter emitter(moduleNameTest.c_str(), moduleNameTest.length());

			sde::MethodEmitter* method = emitter.emit_method(methodNameTest.c_str(), methodNameTest.length());

			Assert::IsTrue(emitter.number_of_methods() == 1);

			std::vector<std::string> args;
			args.push_back("3");
			method->append_opcode(sde::Opcode::LoadConstantI32, &args);
			args[0] = "2";
			method->append_opcode(sde::Opcode::LoadConstantI32, &args);
			method->append_opcode(sde::Opcode::Add, nullptr);
			method->append_opcode(sde::Opcode::Return, nullptr);

			VALIDATE_EMITTER();
		}
	};
}
