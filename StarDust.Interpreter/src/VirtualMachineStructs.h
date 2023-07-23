#pragma once
#include "VirtualMachineConstruct.h"
#include "DataStack.h"

#include <map>
#include <vector>
#include <string>

namespace sdi
{
	class RuntimeStructDefinition
		: public VirtualMachineConstruct
	{
	private:
		std::vector<StackDataType>	m_Types;
		std::string					m_strStructName;
	public:
		const std::vector<StackDataType> defined_types() { return m_Types; }
		_SDVM_API const char* name() { return m_strStructName.c_str(); }

		// Inherited via VirtualMachineConstruct
		virtual const std::string type() override { return m_strStructName; }
	};

	class RuntimeStruct
	{
	private:
		RuntimeStructDefinition* m_pOriginalDefinition;
		std::vector<StackValue> m_StructVariables;
	public:
		void set_val(int i, StackValue& v)
		{
			if (m_StructVariables[i].tag != v.tag)
				throw std::exception();

			m_StructVariables[i] = v;
		}
		void validate();
	};

	typedef std::shared_ptr<RuntimeStruct> StructInstance;

	class RuntimeStructLibrary
	{
	private:
		std::map<std::string, RuntimeStructDefinition> m_Definitions;
	public:
		bool struct_defined(std::string& name) { return m_Definitions.find(name) != m_Definitions.end();}
		bool define_struct(RuntimeStructDefinition&& definition) { return false; }

		StructInstance allocate_instance(std::string& name) { return NULL; }
		StructInstance allocate_instance(RuntimeStructDefinition& definition){ return NULL; }
	};
}

