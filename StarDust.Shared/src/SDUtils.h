#pragma once
#ifndef _SDVM_HEADER_UTILS_H_
#define _SDVM_HEADER_UTILS_H_

#ifdef SDVM_COMPILE_DLL
#define _SDVM_API __declspec(dllexport)
#else
#define _SDVM_API __declspec(dllimport)
#endif // !SDVM_COMPILE_DLL

#ifndef _SDVM_TAB 
#define _SDVM_TAB "    "
#endif // !_SDVM_TAB 


#endif // !_SDVM_HEADER_UTILS_H_


