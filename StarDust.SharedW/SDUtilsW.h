#pragma once

#include <string>

using namespace System;
using namespace System::Runtime::InteropServices;

namespace StarDust {
	namespace Shared {
		public ref class Utility abstract sealed
		{
		public:
			static String^ stos(std::string& str)
			{
				return gcnew String(str.c_str());
			}
		};
	}
}
