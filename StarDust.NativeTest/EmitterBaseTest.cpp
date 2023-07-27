#include "CppUnitTest.h"

#include "ModuleEmitter.h"
#include "TestUtilities.h"
#include "Opcodes.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

#define VALIDATE_EMITTER() assert_result(&emitter, __func__)

namespace StarDustNativeTest
{
	TEST_CLASS(StarDust_EmitterTest)
	{
	public:
		
		static void assert_result(sde::ModuleEmitter* emitter, std::string testName)
		{
			std::stringstream ss;
			emitter->write(&ss);

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
			sde::ModuleEmitter emitter(moduleNameTest.c_str(), moduleNameTest.length());		
			sde::MethodEmitter* method = emitter.add_method(methodNameTest.c_str());			
			Assert::IsTrue(emitter.method_count() == 1);
			VALIDATE_EMITTER();
		}

		TEST_METHOD(TestEmitMethodSum)
		{
			std::string moduleNameTest = "TEST";
			std::string methodNameTest = "Main";
			sde::ModuleEmitter emitter(moduleNameTest.c_str(), moduleNameTest.length());
			
			sde::MethodEmitter* method = emitter.add_method(methodNameTest.c_str());
			
			Assert::IsTrue(emitter.method_count() == 1);
			
			sde::Arguments args;
			args.push_back("3");
			method->add_opcode(sde::Opcode::LoadConstantI32, args);
			
			args.set_at(0, "2");
			method->add_opcode(sde::Opcode::LoadConstantI32, args);
			method->add_opcode(sde::Opcode::Add);
			method->add_opcode(sde::Opcode::Return);
			
			VALIDATE_EMITTER();
		}
	};
}
