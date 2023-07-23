#pragma once

#ifndef _SDVM_INST_NAMES_H_
#define _SDVM_INST_NAMES_H_

#define _SDVM_NOP_INSTRUCTION_NAME			"nop"
#define _SDVM_CALL_INSTRUCTION_NAME			"call"
#define _SDVM_RET_INSTRUCTION_NAME			"ret"
#define _SDVM_ADD_INSTRUCTION_NAME			"add"
#define _SDVM_SUB_INSTRUCTION_NAME			"sub"
#define _SDVM_LDCI4_INSTRUCTION_NAME		"ldc.i4"
#define _SDVM_LDCST_INSTRUCTION_NAME		"ldc.s"

#define _SDVM_LDLOC_INSTRUCTION_NAME		"ldloc"
#define _SDVM_LDARG_INSTRUCTION_NAME		"ldarg"

#define _SDVM_STLOC_INSTRUCTION_NAME		"stloc"
#define _SDVM_STARG_INSTRUCTION_NAME		"starg"


// Branches
#define _SDVM_JMP_INSTRUCTION_NAME			"jmp"
#define _SDVM_BE_INSTRUCTION_NAME			"be"
#define _SDVM_BNE_INSTRUCTION_NAME			"bne"

#define _SDVM_BGT_INSTRUCTION_NAME			"bgt"
#define _SDVM_BGE_INSTRUCTION_NAME			"bge"

#define _SDVM_BLT_INSTRUCTION_NAME			"lgt"
#define _SDVM_BLE_INSTRUCTION_NAME			"lge"

#endif // !_SDVM_INST_NAMES_H_
