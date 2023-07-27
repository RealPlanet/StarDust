#include "DataStack.h"

namespace sdi
{
	void StackValue::copy_memory_if_needed(const StackValue* from, StackValue* to) {
		assert(from != nullptr);
		assert(to != nullptr);

		if (from->is_string())
		{
			to->tag = from->tag;
			const char* src = from->value.s;
			to->value = to->create_value(src, (int)strlen(src));
		}
	}

	StackValue& StackValue::operator=(const StackValue& o)
	{
		value = o.value;
		tag = o.tag;

		copy_memory_if_needed(&o, this);
		return *this;
	}

	StackValue::StackValue(const StackValue& other)
	{
		this->value = other.value;
		this->tag = other.tag;

		copy_memory_if_needed(&other, this);
	}

	StackValue::~StackValue()
	{
		if (is_string()) {
			delete[] value.s;
		}
	}

	StackValue::StackValue(StackValue&& o) noexcept
	{
		this->value = o.value;
		this->tag = o.tag;
		copy_memory_if_needed(&o, this);

		o.value = {0};
		o.tag = StackDataType::Err;
	}

	Value StackValue::create_value(const char* str, int len)
	{
		// Assert len, ensure length is correct!
		assert(strnlen_s(str, len + 1) == len);

		size_t sizeWithTerminator = static_cast<size_t>(len) + 1;
		char* cstr = new char[sizeWithTerminator] {0};
		memset(cstr, 0, sizeWithTerminator);
		strcpy_s(cstr, sizeWithTerminator, str);

		Value v{};
		v.s = cstr;
		return v;
	}

#pragma region
	StackValue operator+(const StackValue& f, const StackValue& s)
	{
		if (f.is_int32())
		{
			if (s.is_int32())
				return StackValue{ f.value.i + s.value.i };

			if (s.is_float())
				return StackValue{ f.value.i + s.value.f };

			if (s.is_char())
				return StackValue{ (int32_t)(f.value.i + s.value.c) };
		}

		if (f.is_float())
		{
			if (s.is_int32())
				return StackValue{ f.value.f + s.value.i };

			if (s.is_float())
				return StackValue{ f.value.f + s.value.f };

			//if (s.is_char())
			//	return StackValue{ value( f.value.f + s.value.f} , StackDataType::Float };
		}

		if (f.is_char())
		{
			if (s.is_char())
				return StackValue{ (char32_t)(f.value.c + s.value.c) };
			if (s.is_int32())
				return StackValue{ (char32_t)(f.value.c + s.value.i) };
		}

		char msg[512] = { 0 };
		sprintf_s(msg, 512,
			"Unable to add stack values together, unsupported operation for f:%d and s:%d", (uint16_t)f.tag, (uint16_t)s.tag);

		throw exceptions::SDInterpreterException(msg);
	}

	StackValue operator-(const StackValue& f, const StackValue& s)
	{
		if (f.is_int32())
		{
			if (s.is_int32())
				return StackValue{ f.value.i - s.value.i };

			if (s.is_float())
				return StackValue{ f.value.i - s.value.f };

			if (s.is_char())
				return StackValue{ (int32_t)(f.value.i - s.value.c) };
		}

		if (f.is_float())
		{
			if (s.is_int32())
				return StackValue{ f.value.f - s.value.i };

			if (s.is_float())
				return StackValue{ f.value.f - s.value.f };

			//if (s.is_char())
			//	return StackValue{ value( f.value.f - s.value.f} , StackDataType::Float };
		}

		if (f.is_char())
		{
			if (s.is_char())
				return StackValue{ (char32_t)(f.value.c - s.value.c) };
			if (s.is_int32())
				return StackValue{ (char32_t)(f.value.c - s.value.i) };
		}

		char msg[512] = { 0 };
		sprintf_s(msg, 512,
			"Unable to subtract stack values together, unsupported operation for f:%d and s:%d", (uint16_t)f.tag, (uint16_t)s.tag);

		throw exceptions::SDInterpreterException(msg);
	}

	StackValue operator*(const StackValue& f, const StackValue& s)
	{
		if (f.is_int32())
		{
			if (s.is_int32())
				return StackValue{ f.value.i * s.value.i };

			if (s.is_float())
				return StackValue{ f.value.i * s.value.f };

			if (s.is_char())
				return StackValue{ (int32_t)(f.value.i * s.value.c) };
		}

		if (f.is_float())
		{
			if (s.is_int32())
				return StackValue{ f.value.f * s.value.i };

			if (s.is_float())
				return StackValue{ f.value.f * s.value.f };

			//if (s.is_char())
			//	return StackValue{ value( f.value.f * s.value.f} , StackDataType::Float };
		}

		char msg[512] = { 0 };
		sprintf_s(msg, 512,
			"Unable to multiply stack values together, unsupported operation for f:%d and s:%d", (uint16_t)f.tag, (uint16_t)s.tag);

		throw exceptions::SDInterpreterException(msg);
	}

	StackValue operator/(const StackValue& f, const StackValue& s)
	{
		if (f.is_int32())
		{
			if (s.is_int32())
				return StackValue{ f.value.i / s.value.i };

			if (s.is_float())
				return StackValue{ f.value.i / s.value.f };

			if (s.is_char())
				return StackValue{ (int32_t)(f.value.i / s.value.c) };
		}

		if (f.is_float())
		{
			if (s.is_int32())
				return StackValue{ f.value.f / s.value.i };

			if (s.is_float())
				return StackValue{ f.value.f / s.value.f };

			//if (s.is_char())
			//	return StackValue{ value( f.value.f / s.value.f} , StackDataType::Float };
		}

		char msg[512] = { 0 };
		sprintf_s(msg, 512,
			"Unable to divide stack values together, unsupported operation for f:%d and s:%d", (uint16_t)f.tag, (uint16_t)s.tag);

		throw exceptions::SDInterpreterException(msg);
	}
#pragma endregion Operators
}
