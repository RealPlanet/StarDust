#pragma once

#ifndef _SDVM_ATTRIBUTES_H_
#define _SDVM_ATTRIBUTES_H_

#include <map>
#include <string>

namespace sdi
{
	enum class AttributeType
	{
		Error = 0x0,

		EntryPoint,
		LocalsDefinition,
	};

	class Attributes
	{
	private:
		std::map<AttributeType, void*> m_AttributeMap;
	public:
		Attributes() = default;
		Attributes(const Attributes& other) = default;
		Attributes(Attributes&& other) noexcept { m_AttributeMap = std::move(other.m_AttributeMap); }

		bool add_attr(AttributeType attrName);
		[[deprecated("This function is currently unused, custom attributes is currently unsupported!")]]
		bool add_attr(AttributeType attrName, void* ptr) {}

		bool rem_attr(AttributeType attrName);
		[[deprecated("This function is currently unused, custom attributes is currently unsupported!")]]
		bool rem_attr(AttributeType attrName, void* ptr) {}

		bool has_attribute(AttributeType attrName) const { return m_AttributeMap.find(attrName) != m_AttributeMap.end(); }
		size_t size() const { return m_AttributeMap.size(); }
		bool empty() const { return m_AttributeMap.empty(); }

		static AttributeType stoa(const std::string& s);

		void operator=(const Attributes& other)
		{
			m_AttributeMap.clear();
			m_AttributeMap = other.m_AttributeMap;
		}
	};
}

#endif // !_SDVM_ATTRIBUTES_H_



