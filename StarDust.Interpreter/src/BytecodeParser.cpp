#include "stdafx.h"

#include "Utility.h"

#include "BytecodeParser.h"
#include "RuntimeStructure.h"

#include "Logger.h"

#include "RuntimeInstructionSet.h"

namespace sdi
{
	BytecodeParser::BytecodeParser()
		: theInstructionCache{this}
	{
	}

#pragma region

	char BytecodeParser::peek(int offset)
	{
		size_t textSize = theCurrentFile->size();
		
		if (theCurrentIndex + offset >= textSize)
			return EOF;

		return (*theCurrentFile)[theCurrentIndex + offset];
	}

	void BytecodeParser::skipwspace(bool incNewline)
	{
		while (iswspace(current()))
		{
			if (!incNewline && util::isnewline(current()))
				return;

			theCurrentIndex++;
		}
	}

	void BytecodeParser::consume_trivia()
	{
		while (true)
		{
			skipwspace();
			if (current() != '/' || peek(1) != '/')
				break;

			tonextline();
		}

		skipwspace();
	}

	void BytecodeParser::tonextline()
	{
		while (!util::isnewline(current()))
			theCurrentIndex++;

		while (util::isnewline(current()))
			theCurrentIndex++;
	}

	bool BytecodeParser::consume_c(char c)
	{
		skipwspace();
		consume_trivia();
		char currC = current();
		if (currC == c)
		{
			theCurrentIndex++;
			return true;
		}

		return false;
	}

	BytecodeParser::BytecodeParser(BytecodeParser&& other)
		: theInstructionCache{other.theInstructionCache}
	{

	}

	void BytecodeParser::add_bytecode(BytecodeFilePtr bytecode)
	{
		if (theBytecodeFiles.find(bytecode->fullname()) != theBytecodeFiles.end())
			return;

		theBytecodeFiles.insert(std::make_pair(bytecode->fullname(), bytecode));
	}

	RuntimeConstructs BytecodeParser::string_to_construct(std::string& construct)
	{
		if (construct.compare(_SDVM_CONSTRUCT_METHOD) == 0)
			return RuntimeConstructs::Method;

		return RuntimeConstructs::Unknown;
	}
#pragma endregion Utility

	std::shared_ptr<RuntimeStructure> BytecodeParser::parse()
	{
		theErrorValue = ParserErrorValue::NoError;

		reset(); // Full reset before starting

		theOutputStructure = std::make_shared<RuntimeStructure>();

		for (auto& filePair : theBytecodeFiles)
		{
			theCurrentIndex = 0;
			theCurrentFile = filePair.second;
			parse_file();
		}

		RuntimeStructurePtr output = theOutputStructure;

		reset(); // Full reset before exiting
		theBytecodeFiles.clear(); // Clear paths too

		return output;
	}

	void BytecodeParser::reset()
	{
		theCurrentFile = NULL;
		theOutputStructure = NULL;

		theCurrentIndex = 0;
		theInstructionCache.clear();
	}

	void BytecodeParser::parse_file()
	{
		while (current() != EOF)
		{
			char c = current();

			switch (c)
			{
				case '\n':
				case '\r':
				case '\t':
				case ' ':
				case EOF:
				{
					theCurrentIndex++;
					break;
				}
				case '/':
				{
					if (peek(1) == '/')
					{
						tonextline();
						continue;
					}
					theCurrentIndex++;
					break;
				}
				case '.':
				{
					theCurrentIndex++;// Skip dot
					parse_bytecode_construct();
					break;
				}
				default:
				{
					theErrorValue = ParserErrorValue::UnknownCharacterError;
					_SDVM_LOGEF("Unknown character at index {}, character is: {}", current(), theCurrentIndex);
					theCurrentIndex++;
					break;
				}
			}
		}
	}

	void BytecodeParser::parse_bytecode_construct()
	{
		// Assumes '.' marker was consumed
		std::string constructName = read_identifier();

		_SDVM_LOGVF("Found construct '{}'", constructName);
		RuntimeConstructs construct = string_to_construct(constructName);

		switch (construct)
		{
		case RuntimeConstructs::Method:
			parse_method_construct();
			return;
		default:
			return;
		}
	}

	void BytecodeParser::parse_method_construct()
	{
		std::string returnTypeName = read_identifier();
		std::string methodName = read_identifier();

		FunctionPtr newFunc = std::make_shared<Function>();
		newFunc->theFunctionName = methodName;

		//newFunc->SetReturnType(Type::GetTypeFromString(returnTypeName));

		parse_method_arguments(newFunc.get());

		_SDVM_LOGVF("STEP :: Found method '{}' with return type '{}'", methodName, returnTypeName);

		parse_method_body(newFunc.get());

		if (newFunc->attributes().has_attribute(AttributeType::EntryPoint))
		{
			_SDVM_LOGV("Method is marked as entrypoint!");
		}

		this->theOutputStructure->m_Functions.insert_function(newFunc);
	}

	void BytecodeParser::parse_method_arguments(Function* pFunc)
	{
		if (!consume_c(_SDVM_TOKEN_OPEN_PARENTHESIS))
		{
			theErrorValue = ParserErrorValue::CharacterMatchError;
			_SDVM_LOGE("Could not match opening parenthesis for method arguments");
			return;
		}

		while (!consume_c(_SDVM_TOKEN_CLOSE_PARENTHESIS))
		{
			std::string type = read_identifier();
			std::string argName = read_identifier(); // TODO :: Metadata

			StackDataType argType = stodt(type);
			if (argType == StackDataType::Err)
			{
				theErrorValue = ParserErrorValue::UnknownTypeError;
				_SDVM_LOGEF("THIS TYPE DOES NOT EXIST! {}", type);
				return;
			}

			pFunc->theArgumentTypes.push_back(argType);

			if (!consume_c(_SDVM_TOKEN_COMMA) && !consume_c(_SDVM_TOKEN_CLOSE_PARENTHESIS))
			{
				theErrorValue = ParserErrorValue::UnknownCharacterError;
				_SDVM_LOGEF("EXPECTED COMMA SEPARATOR! NOT {}", current());
				return;
			}
		}
	}

	void BytecodeParser::parse_method_body(Function* pFunc)
	{
		if (!consume_c(_SDVM_TOKEN_OPEN_BRACKET))
		{
			theErrorValue = ParserErrorValue::CharacterMatchError;
			_SDVM_LOGE("Could not match opening bracket for method body");
			return;
		}
		Attributes funcAttributes = parse_attributes();
		InstructionSequence funcBody;

		while (!consume_c(_SDVM_TOKEN_CLOSE_BRACKET) && current() != EOF)
		{
			std::string instLabel = read_identifier();

			if (instLabel.rfind(_SDVM_LABEL_PREFIX, 0) != 0)
			{
				theErrorValue = ParserErrorValue::GenericError;
				_SDVM_LOGEF("Expected label but found: {}", instLabel);
				return;
			}

			if (!consume_c(_SDVM_TOKEN_COLON))
			{
				theErrorValue = ParserErrorValue::CharacterMatchError;
				_SDVM_LOGE("Expected colon token separating instruction label!");
				return;
			}

			std::string instName = read_instruction();
			InstructionArguments args;
			read_instruction_arguments(&args);


			// Get the definition
			std::shared_ptr<InstructionDefinition> def = RuntimeInstructionSet::stod(instName);

			if (def == NULL)
			{
				theErrorValue = ParserErrorValue::GenericError;
				_SDVM_LOGEF("Unknown function name '{}', aborting...", instName);
				return;
			}

			// Grab a variant, if possible, a cached one
			InstructionVariantPtr actualInstruction = theInstructionCache.create_variant(def, args);

			instLabel = instLabel.substr(2);
			funcBody.insert_instruction(actualInstruction, std::stoi(instLabel, 0, 16));
		}

		pFunc->m_Attributes = funcAttributes;
		pFunc->theFunctionBody = funcBody;

		if (peek(-1) != _SDVM_TOKEN_CLOSE_BRACKET)
		{
			theErrorValue = ParserErrorValue::CharacterMatchError;
			_SDVM_LOGE("Could not match closing bracket for method body");
			return;
		}
	}

	Attributes BytecodeParser::parse_attributes()
	{
		Attributes objAttributes;

		while (true)
		{
			int start = theCurrentIndex;
			skipwspace();

			if (!consume_c(_SDVM_ATTR_MARKER))
			{
				_SDVM_LOG("No marker found, exiting...");
				break;
			}

			std::string attributeName = read_identifier();
			AttributeType attribute = Attributes::stoa(attributeName);

			if (attribute == AttributeType::Error)
			{
				theErrorValue = ParserErrorValue::GenericError;
				_SDVM_LOGE("Unknown attribute, if this is a custom one they are not supported yet!");
				break;
			}

			objAttributes.add_attr(attribute);
		}

		_SDVM_LOGVF("Parsed {} attributes", objAttributes.size());
		return objAttributes;
	}

	std::string BytecodeParser::read_identifier()
	{
		skipwspace();
		int start = theCurrentIndex;
		while (isalpha(current()) || current() == '_' || isdigit(current()))
			theCurrentIndex++;

		return theCurrentFile->toString(start, theCurrentIndex - start);
	}

	std::string BytecodeParser::read_instruction()
	{
		skipwspace();

		int start = theCurrentIndex;
		while (isalpha(current()) || isdigit(current()) ||
			current() == '_' || current() == '.')
		{
			theCurrentIndex++;
		}

		return theCurrentFile->toString(start, theCurrentIndex - start);
	}

	void BytecodeParser::read_instruction_arguments(InstructionArguments* args)
	{
		if (args == NULL)
			return;

		args->clear();	
		while (!util::isnewline(current()))
		{
			skipwspace(false);
			int start = theCurrentIndex;

			while (!iswspace(current()))
				theCurrentIndex++;

			// Found something
			std::string arg = theCurrentFile->toString(start, theCurrentIndex - start);
			util::trim(arg);
			args->add(arg);
		}
	}
}


