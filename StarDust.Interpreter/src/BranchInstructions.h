#pragma once

#ifndef _SDVM_BRANCH_INSTRUCTIONS_H_
#define _SDVM_BRANCH_INSTRUCTIONS_H_

#include "InstructionDefinition.h"

namespace sdi
{
	class BranchBaseVariant
		: public InstructionVariant
	{
	protected:
		size_t theTargetLabel;

		BranchBaseVariant(size_t targetLabel)
			: theTargetLabel{ targetLabel }
		{}
	};

	// Unconditional Jump
	class UnconditionalJumpDefinition
		: public InstructionDefinition
	{
	public:
		UnconditionalJumpDefinition()
			: InstructionDefinition(_SDVM_JMP_INSTRUCTION_NAME) {}
		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class UnconditionalJumpVariant
		: public BranchBaseVariant
	{
		friend class UnconditionalJumpDefinition;

		UnconditionalJumpVariant(size_t targetLabel)
			: BranchBaseVariant(targetLabel) {}
	public:
		// Inherited via BranchBaseVariant
		virtual int execute(ExecutionScope* scope) override;
	};

	// BE
	class BranchEqualsDefinition
		: public InstructionDefinition
	{
	public:
		BranchEqualsDefinition()
			: InstructionDefinition(_SDVM_BE_INSTRUCTION_NAME) {}
		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class BranchEqualsVariant
		: public BranchBaseVariant
	{
		friend class BranchEqualsDefinition;

		BranchEqualsVariant(size_t targetLabel)
			: BranchBaseVariant(targetLabel) {}
	public:
		// Inherited via BranchBaseVariant
		virtual int execute(ExecutionScope* scope) override;
	};

	// BNE
	class BranchNotEqualsDefinition
		: public InstructionDefinition
	{
	public:
		BranchNotEqualsDefinition()
			: InstructionDefinition(_SDVM_BNE_INSTRUCTION_NAME) {}
		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class BranchNotEqualsVariant
		: public BranchBaseVariant
	{
		friend class BranchNotEqualsDefinition;
		BranchNotEqualsVariant(size_t targetLabel)
			: BranchBaseVariant(targetLabel) {}
	public:
		virtual int execute(ExecutionScope* scope) override;
	};

	// BGE
	class BranchGreaterThanOrEqualDefinition
		: public InstructionDefinition
	{
	public:
		BranchGreaterThanOrEqualDefinition()
			: InstructionDefinition(_SDVM_BGE_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class BranchGreaterThanOrEqualVariant
		: public BranchBaseVariant
	{
		friend class BranchGreaterThanOrEqualDefinition;

		BranchGreaterThanOrEqualVariant(size_t targetLabel)
			: BranchBaseVariant(targetLabel) {}

	public:
		virtual int execute(ExecutionScope* scope) override;
	};
	// BGT
	class BranchGreaterThanDefinition
		: public InstructionDefinition
	{
	public:
		BranchGreaterThanDefinition()
			: InstructionDefinition(_SDVM_BGT_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class BranchGreaterThanVariant
		: public BranchBaseVariant
	{
		friend class BranchGreaterThanDefinition;

		BranchGreaterThanVariant(size_t targetLabel)
			: BranchBaseVariant(targetLabel) {}

	public:
		virtual int execute(ExecutionScope* scope) override;
	};

	// BLE
	class BranchLessThanOrEqualDefinition
		: public InstructionDefinition
	{
	public:
		BranchLessThanOrEqualDefinition()
			: InstructionDefinition(_SDVM_BLE_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class BranchLessThanOrEqualVariant
		: public BranchBaseVariant
	{
		friend class BranchLessThanOrEqualDefinition;

		BranchLessThanOrEqualVariant(size_t targetLabel)
			: BranchBaseVariant(targetLabel) {}

	public:
		virtual int execute(ExecutionScope* scope) override;
	};

	// BLT
	class BranchLessThanDefinition
		: public InstructionDefinition
	{
	public:
		BranchLessThanDefinition()
			: InstructionDefinition(_SDVM_BLT_INSTRUCTION_NAME) {}

		// Inherited via InstructionDefinition
		virtual std::shared_ptr<InstructionVariant> create_variant(InstructionArguments* arguments) override;
	};

	class  BranchLessThanVariant
		: public BranchBaseVariant
	{
		friend class BranchLessThanDefinition;

		BranchLessThanVariant(size_t targetLabel)
			: BranchBaseVariant(targetLabel) {}
	public:
		virtual int execute(ExecutionScope* scope) override;
	};
}

#endif // !_SDVM_BRANCH_INSTRUCTIONS_H_
