#pragma once

#define DLL_BOUNDARY_DEF(nativeClass, dllExternalName)\
	class nativeClass;\
	class dllExternalName{\
		private:\
			nativeClass* __ptr;\
		public:\
			dllExternalName(nativeClass* ptr) : __ptr{ptr} { }\
			nativeClass* get() { return __ptr; }\
	};\

namespace sdi {
	DLL_BOUNDARY_DEF(StackValue, ValueHandle)
};
