#include "DataStack.h"

namespace sdi
{
	StackValue::~StackValue()
	{
		if (is_string())
		{
			delete[] value.s;
		}
	}

	StackValue::StackValue(const StackValue& o)
	{
		this->value = o.value;
		this->tag = o.tag;

		if (this->tag == StackDataType::String)
		{
			// Copy string data
			size_t len = strlen(this->value.s);
			this->value = create_value(this->value.s, (int)len);
		}
	}

	StackValue::StackValue(StackValue&& o) noexcept
	{
		this->value = o.value;
		this->tag = o.tag;

		o.tag = StackDataType::Err;
	}

	Value StackValue::create_value(const char* str, int len)
	{
		char* cstr = new char[len];
		strcpy_s(cstr, len, str);

		Value v;
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
