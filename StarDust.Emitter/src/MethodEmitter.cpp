
#include "SDShared.h"

#include "Utility.h"
#include "MethodEmitter.h"

#include "Instruction.h"
#include "Attribute.h"
#include "Argument.h"

#define BEGIN_METHOD()\
	* stream << _SDVM_ATTR_MARKER << _SDVM_CONSTRUCT_METHOD << " void " << m_Data.MethodName;\
	*stream << "()" << _SDVM_GENERIC_NEWLINE << '{' << _SDVM_GENERIC_NEWLINE;\

#define END_METHOD()\
		*stream << '}' << _SDVM_GENERIC_NEWLINE << _SDVM_GENERIC_NEWLINE;


namespace sde
{
	MethodEmitter::MethodEmitter(const char* method_name, size_t name_len)
	{
		m_Data.MethodName.assign(method_name, name_len);
	}

	void MethodEmitter::dispose()
	{
		m_Data.MethodAttributes.clear();
		m_Data.MethodInstructions.clear();
	}

	void MethodEmitter::write(std::stringstream* stream)
	{
		BEGIN_METHOD();

		m_Data.MethodAttributes.write(stream);
		m_Data.MethodInstructions.write(stream);

		END_METHOD();
	}

	MethodEmitter* MethodEmitter::add_opcode(const Opcode id, Arguments& args)
	{
		Instruction inst(m_LabelIndex++ ,id, args);
		m_Data.MethodInstructions.add(inst);
		return this;
	}

	MethodEmitter* MethodEmitter::add_opcode(const Opcode id)
	{
		Arguments args;
		return add_opcode(id, args);
	}

	MethodEmitter* MethodEmitter::add_attribute(const AttributeCode attribute, Arguments& args)
	{
		Attribute atr(attribute, args);
		m_Data.MethodAttributes.add(atr);
		return this;
	}

	MethodEmitter* MethodEmitter::add_attribute(const AttributeCode attribute)
	{
		Arguments args;
		return this->add_attribute(attribute, args);
	}
}


