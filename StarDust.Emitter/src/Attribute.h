#pragma once

#include "Container.h"
#include "Argument.h"
#include "EmitterEntity.h"
#include "SDShared.h"

namespace sde {
	enum class _SDVM_API AttributeCode
	{
		NoAttribute = -1,

		EntryPoint,
	};

	class Attribute
		: public EmitterEntity {
	private:
		AttributeCode m_Code;
		Arguments m_Arguments;
	public:
		_SDVM_API Attribute(AttributeCode code, Arguments& args)
			: m_Code{ code }, m_Arguments{ args } {}

		_SDVM_API virtual void dispose();
		_SDVM_API virtual void write(std::stringstream* stream) override;
	};

	class Attributes
		: public Container<Attribute>, public EmitterEntity
	{
	public:
		_SDVM_API Attribute& at(size_t i) { return m_Data.at(i); }
		_SDVM_API virtual void add(Attribute& atr) { m_Data.push_back(atr); }

		_SDVM_API virtual void dispose() override;
		_SDVM_API virtual void write(std::stringstream* stream) override;
	};
}