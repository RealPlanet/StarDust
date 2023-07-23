#include "stdafx.h"

#include "Attributes.h"
namespace sdi
{
	bool Attributes::add_attr(AttributeType attrName)
	{
		if (m_AttributeMap.find(attrName) != m_AttributeMap.end())
			return true;

		m_AttributeMap.insert(std::make_pair(attrName, nullptr));
		return true;
	}

	bool Attributes::rem_attr(AttributeType attrName)
	{
		if (m_AttributeMap.find(attrName) != m_AttributeMap.end())
		{
			m_AttributeMap.erase(attrName);
			return true;
		}

		return false;
	}

	AttributeType Attributes::stoa(const std::string& s)
	{
		if (s == _SDVM_ATTR_ENTRYPOINT)
			return AttributeType::EntryPoint;

		return AttributeType::Error;
	}

}

