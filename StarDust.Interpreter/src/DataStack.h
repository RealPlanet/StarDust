#pragma once

#ifndef _SDVM_DATASTACK_H_
#define _SDVM_DATASTACK_H_

#include "stdafx.h"
#include "SDInterpreterException.h"

#define assert_stackvalue  assert(is_valid())

namespace sdi
{
	enum class StackDataType
		: uint16_t
	{
		Err,

		Bool,
		Char,
		Int32,
		Float,
		String,
	};

	static StackDataType stodt(std::string& s)
	{
		if (s.compare("int32") == 0)
			return StackDataType::Int32;
		if (s.compare("float") == 0)
			return StackDataType::Float;
		if (s.compare("char") == 0)
			return StackDataType::Char;
		if (s.compare("bool") == 0)
			return StackDataType::Bool;
		if (s.compare("string") == 0)
			return StackDataType::String;

		return StackDataType::Err;
	}

	typedef union {
		bool			b;
		char32_t		c;
		int32_t			i;
		float_t			f;
		const char* s;
	} Value;

	class StackValue
	{
	public:
		_SDVM_API ~StackValue();
		_SDVM_API StackValue()					: tag{ StackDataType::Err } {}
		_SDVM_API StackValue(bool v)			: value{ create_value(v) }, tag{ StackDataType::Bool } {}
		_SDVM_API StackValue(char32_t v)		: value{ create_value(v) }, tag{ StackDataType::Char } {}
		_SDVM_API StackValue(int32_t v)			: value{ create_value(v) }, tag{ StackDataType::Int32 } {}
		_SDVM_API StackValue(float_t v)			: value{ create_value(v) }, tag{ StackDataType::Float } {}
		_SDVM_API StackValue(std::string v)		: value{ create_value(v) }, tag{ StackDataType::String } {}
		_SDVM_API StackValue(StackValue&& o) noexcept;
		_SDVM_API StackValue(const StackValue& o);

		bool is_valid()			const
		{
			if (tag == StackDataType::String)
				return value.s != nullptr;

			return tag != StackDataType::Err;
		}

		bool is_bool()			const { return tag == StackDataType::Bool; }
		bool is_char()			const { return tag == StackDataType::Char; }
		bool is_int32()			const { return tag == StackDataType::Int32; }
		bool is_float()			const { return tag == StackDataType::Float; }
		bool is_string()		const { return tag == StackDataType::String && value.s != nullptr; }

		bool		as_bool()	const { assert_stackvalue; return value.b; }
		char32_t	as_char()	const { assert_stackvalue; return value.c; }
		int32_t		as_int32()	const { assert_stackvalue; return value.i; }
		float_t		as_float()	const { assert_stackvalue; return value.f; }
		// Allocates a new string each time
		std::string	as_string()	const { assert_stackvalue; return value.s; }

		Value value = { 0 };
		StackDataType tag = StackDataType::Err;

		void copy_memory_if_needed(const StackValue* from, StackValue* to);
		_SDVM_API StackValue& operator=(const StackValue& o);

#pragma region
		bool operator==(const StackValue& o)
		{
			if (!is_valid() || tag != o.tag)
				return false;

			if (is_bool())
				return as_bool() == o.as_bool();
			if (is_char())
				return as_char() == o.as_char();
			if (is_int32())
				return as_int32() == o.as_int32();
			if (is_float())
				return as_float() == o.as_float();

			_SDVM_THROW_IF(true);
		}

		bool operator!=(const StackValue& o) { return !(*this == o); }

		bool operator >=(const StackValue& o) { return *this > o || *this == o; }

		bool operator <=(const StackValue& o) { return *this < o || *this == o; }

		bool operator >(const StackValue& o)
		{
			if (!is_valid() || tag != o.tag)
				return false;

			if (is_char())
				return as_char() > o.as_char();
			if (is_int32())
				return as_int32() > o.as_int32();
			if (is_float())
				return as_float() > o.as_float();

			_SDVM_THROW_IF(true);
		}

		bool operator <(const StackValue& o)
		{
			if (!is_valid() || tag != o.tag)
				return false;

			if (is_char())
				return as_char() < o.as_char();
			if (is_int32())
				return as_int32() < o.as_int32();
			if (is_float())
				return as_float() < o.as_float();

			_SDVM_THROW_IF(true);
		}
#pragma endregion Operators

	private:
		Value create_value(bool val) { Value v; v.b = val; return v; }
		Value create_value(char32_t val) { Value v; v.c = val; return v; }
		Value create_value(int32_t val) { Value v; v.i = val; return v; }
		Value create_value(float_t val) { Value v; v.f = val; return v; }
		Value create_value(std::string str) { return create_value(str.c_str(), (int)str.length()); }
		Value create_value(const char* str, int len);
	};

	class DataStack
	{
	private:
		std::stack<StackValue> m_Stack;

	public:
		StackValue& top() { return m_Stack.top(); }
		size_t		size() { return m_Stack.size(); }

		void		push(const StackValue& val) { m_Stack.emplace(val); }
		void		push(int32_t val) { m_Stack.emplace(val); }
		void		push(float_t val) { m_Stack.emplace(val); }
		void		push(char32_t val) { m_Stack.emplace(val); }
		void		push(bool val) { m_Stack.emplace(val); }
		void		push(const std::string& val) { m_Stack.emplace(val); }

		void pop_into(StackValue& dest)
		{
			StackValue val = top();
			dest = val;

			m_Stack.pop();
		}

		StackValue pop()
		{
			StackValue val = top();
			m_Stack.pop();
			return val;
		}
	};

#pragma region
	StackValue operator+(const StackValue& f, const StackValue& s);

	StackValue operator-(const StackValue& f, const StackValue& s);

	StackValue operator*(const StackValue& f, const StackValue& s);

	StackValue operator/(const StackValue& f, const StackValue& s);
#pragma endregion StackValue Operators
}

#endif // !_SDVM_DATASTACK_H_
