#ifndef _SDVM_CONSTANTS_H_
#define _SDVM_CONSTANTS_H_

#define _SDVM_CONSTRUCT_METHOD		"method"

#define _SDVM_COMMENT_PREFIX		"//"
#define _SDVM_LABEL_PREFIX			"L_"

#define _SDVM_TOKEN_OPEN_PARENTHESIS	'('
#define _SDVM_TOKEN_CLOSE_PARENTHESIS	')'
#define _SDVM_TOKEN_OPEN_BRACKET		'{'
#define _SDVM_TOKEN_CLOSE_BRACKET		'}'
#define _SDVM_TOKEN_COLON				':'
#define _SDVM_TOKEN_COMMA				','
#define _SDVM_THROW_IF_NULL(x) if(x == NULL) throw std::runtime_error("Null assert failed!")
#define _SDVM_THROW_IF(cond) if(cond) throw std::runtime_error("Conditional assert failed!")
#define _SDVM_THROW_IF_W_ERR(cond, s) if(cond) throw std::runtime_error(s)
#define _SDVM_GENERIC_NEWLINE '\n'
#endif