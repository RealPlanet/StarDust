#include "CppUnitTest.h"

#include "ModuleEmitter.h"
#include "MethodEmitter.h"

#include "Argument.h"

#include "TestUtilities.h"
#include "Opcodes.h"
#include "RuntimeInstructionBinder.h"
#include "ExecutionScope.h"
#include "Logger.h"

#include <VirtualMachine.h>
#include <BytecodeParser.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace StarDustNativeTest
{
	TEST_CLASS(StarDust_EmitterTest_Execution)
	{
	private:
		static const char* m_pBranchTestScriptName;
		static bool m_bAck;

		static void setAck(sdi::ExecutionScope* scope)
		{
			m_bAck = true;
			Logger::WriteMessage(" ** Ack SET **\n");
		}

		static void printVal(sdi::ExecutionScope* scope)
		{
			// MOVE CONSTRUCTOR BREAKS MEMORY
			//sdi::StackValue val;
			//scope->get_data_stack()->pop(val);
			sdi::StackValue val = scope->get_data_stack()->pop();

			Assert::AreNotEqual((int)sdi::StackDataType::Err, (int)val.tag);
			std::string str;
			switch (val.tag)
			{
			case sdi::StackDataType::Bool:
				str = std::to_string(val.as_bool()) + "\n";
				break;
			case sdi::StackDataType::Char:
				str = val.as_char() + "\n";
				break;
			case sdi::StackDataType::Int32:
				str = std::to_string(val.as_int32()) + "\n";
				break;
			case sdi::StackDataType::Float:
				str = std::to_string(val.as_float()) + "\n";
				break;
			case sdi::StackDataType::String:
				str = val.as_string() + "\n";
				break;
			default:
				Assert::Fail();
			}

			Logger::WriteMessage(str.c_str());

		}

		static void appen_std_return(sde::MethodEmitter* method) {
			sde::Arguments arguments;
			arguments.push_back("0");

			method->add_opcode(sde::Opcode::LoadConstantI32, arguments);
			method->add_opcode(sde::Opcode::Return);
		}
	public:
		TEST_CLASS_INITIALIZE(emit_test_script)
		{
			sdi::RuntimeInstructionBinder::bind_function("doAck", setAck);
			sdi::RuntimeInstructionBinder::bind_function("print", printVal);
			sdi::Logger::init(".\\");
		}

		TEST_CLASS_CLEANUP(clear_test_scripts)
		{
		}

		TEST_METHOD_INITIALIZE(pre_test)
		{
			m_bAck = false;
		}

		TEST_METHOD(test_entrypoint_execution)
		{
			Logger::WriteMessage(" ** Begin test! **\n");

			const char* scriptName = "Entrypoint testing";
			sde::ModuleEmitter emitter(scriptName, strlen(scriptName));

			sde::MethodEmitter* method = emitter.add_method("Main");
			sde::Arguments arguments{ "doAck" };

			method->add_attribute(sde::AttributeCode::EntryPoint)
				->add_opcode(sde::Opcode::Call, arguments);

			appen_std_return(method);

			sdi::VirtualMachine vm;
			std::stringstream ss;
			emitter.write(&ss);

			sdi::BytecodeParser parser;
			sdi::BytecodeFilePtr bytecodePtr = sdi::BytecodeFile::froms(ss.str());
			parser.add_bytecode(bytecodePtr);

			sdi::RuntimeStructurePtr structure = parser.parse();
			vm.load(structure);
			vm.run();
			Assert::IsTrue(m_bAck, L"Ack not set!");
			Logger::WriteMessage(" ** End test! **\n");
		}

		TEST_METHOD(test_branch_execution)
		{
			Logger::WriteMessage(" ** Begin test! **\n");

			// load 1
			// load 2
			// if 1 > 2
			//		ack
			// else
			//		nop
			//	nop

			const char* scriptName = "Branch testing";
			sde::ModuleEmitter emitter(scriptName, strlen(scriptName));

			sde::MethodEmitter* method = emitter.add_method("Main");
			sde::Arguments const1{ "1" };
			sde::Arguments const2{ "2" };
			sde::Arguments labelIfTrue{ "L_0004" };
			sde::Arguments labelIfSkipExit{ "L_0005" };
			sde::Arguments callArgs{ "doAck" };

			method->add_attribute(sde::AttributeCode::EntryPoint)
				->add_opcode(sde::Opcode::LoadConstantI32, const1) // 0
				->add_opcode(sde::Opcode::LoadConstantI32, const2) // 1
				->add_opcode(sde::Opcode::BranchEquals, labelIfTrue) // 2
				->add_opcode(sde::Opcode::UnconditionalJump, labelIfSkipExit) //3
				->add_opcode(sde::Opcode::Call, callArgs) // 4
				->add_opcode(sde::Opcode::NoOperation)		// 5
				->add_opcode(sde::Opcode::NoOperation);		// 6;

			appen_std_return(method);

			sdi::VirtualMachine vm;
			std::stringstream ss;
			emitter.write(&ss);

			sdi::BytecodeParser parser;
			sdi::BytecodeFilePtr bytecodePtr = sdi::BytecodeFile::froms(ss.str());
			parser.add_bytecode(bytecodePtr);

			sdi::RuntimeStructurePtr structure = parser.parse();
			vm.load(structure);
			vm.run();
			Assert::IsFalse(m_bAck, L"Ack was set set!");
			Logger::WriteMessage(" ** End test! **\n");
		}

		TEST_METHOD(test_print_values)
		{
			Logger::WriteMessage(" ** Begin test! **\n");

			const char* scriptName = "Branch testing";
			sde::ModuleEmitter emitter(scriptName, strlen(scriptName));
			sde::MethodEmitter* method = emitter.add_method("Main");

			sde::Arguments callArgs{ "print" };
			sde::Arguments str1{ "Deez" };
			sde::Arguments str2{ "Nuts" };

			method->add_attribute(sde::AttributeCode::EntryPoint)
				->add_opcode(sde::Opcode::LoadString, str1)
				->add_opcode(sde::Opcode::Call, callArgs)
				->add_opcode(sde::Opcode::LoadString, str2)
				->add_opcode(sde::Opcode::Call, callArgs)
				->add_opcode(sde::Opcode::NoOperation);

			appen_std_return(method);

			std::stringstream ss;
			emitter.write(&ss);

			Logger::WriteMessage(ss.str().c_str());

			sdi::BytecodeParser parser;
			sdi::BytecodeFilePtr bytecodePtr = sdi::BytecodeFile::froms(ss.str());
			parser.add_bytecode(bytecodePtr);
			sdi::RuntimeStructurePtr structure = parser.parse();
			Assert::AreEqual(
				(int)sdi::BytecodeParser::ParserErrorValue::NoError,
				(int)parser.get_err_value());

			Assert::AreEqual(1, (int)structure->functions().size());

			sdi::VirtualMachine vm;
			vm.load(structure);
			vm.run();

			Assert::IsFalse(m_bAck, L"Ack was set set!");
			Logger::WriteMessage(" ** End test! **\n");
		}
	};

	const char* StarDust_EmitterTest_Execution::m_pBranchTestScriptName = "";
	bool StarDust_EmitterTest_Execution::m_bAck = false;
}
