#include "BranchInstructions.h"
#include "DataStack.h"
#include "ExecutionScope.h"

namespace sdi
{
	size_t validate_branch_arguments(InstructionArguments* arguments)
	{
		assert(arguments->size() == 1);
		std::string& label = arguments->at(0);

		assert(label.rfind(_SDVM_LABEL_PREFIX, 0) == 0);

		std::string labelNum = label.substr(2);

		return std::stoi(labelNum, 0, 16);
	}

	std::shared_ptr<InstructionVariant> UnconditionalJumpDefinition::create_variant(InstructionArguments* arguments)
	{
		size_t labelTarget = validate_branch_arguments(arguments);
		return std::shared_ptr<InstructionVariant>(new UnconditionalJumpVariant(labelTarget));
	}

	std::shared_ptr<InstructionVariant> BranchLessThanOrEqualDefinition::create_variant(InstructionArguments* arguments)
	{
		size_t labelTarget = validate_branch_arguments(arguments);
		return std::shared_ptr<InstructionVariant>(new BranchLessThanOrEqualVariant(labelTarget));
	}

	std::shared_ptr<InstructionVariant> BranchLessThanDefinition::create_variant(InstructionArguments* arguments)
	{
		size_t labelTarget = validate_branch_arguments(arguments);
		return std::shared_ptr<InstructionVariant>(new BranchLessThanVariant(labelTarget));
	}

	std::shared_ptr<InstructionVariant> BranchGreaterThanDefinition::create_variant(InstructionArguments* arguments)
	{
		size_t labelTarget = validate_branch_arguments(arguments);
		return std::shared_ptr<InstructionVariant>(new BranchGreaterThanVariant(labelTarget));
	}

	std::shared_ptr<InstructionVariant> BranchNotEqualsDefinition::create_variant(InstructionArguments* arguments)
	{
		size_t labelTarget = validate_branch_arguments(arguments);
		return std::shared_ptr<InstructionVariant>(new BranchNotEqualsVariant(labelTarget));
	}

	std::shared_ptr<InstructionVariant> BranchEqualsDefinition::create_variant(InstructionArguments* arguments)
	{
		size_t labelTarget = validate_branch_arguments(arguments);
		return std::shared_ptr<InstructionVariant>(new BranchEqualsVariant(labelTarget));
	}

	std::shared_ptr<InstructionVariant> BranchGreaterThanOrEqualDefinition::create_variant(InstructionArguments* arguments)
	{
		size_t labelTarget = validate_branch_arguments(arguments);
		return std::shared_ptr<InstructionVariant>(new BranchGreaterThanOrEqualVariant(labelTarget));
	}

	int UnconditionalJumpVariant::execute(ExecutionScope* scope)
	{
		scope->set_label_number(this->theTargetLabel);
		return 0;
	}

	int BranchEqualsVariant::execute(ExecutionScope* scope)
	{
		DataStack* dataStack = scope->get_data_stack();
		StackValue val1, val2;
		scope->get_data_stack()->pop_into(val1);
		scope->get_data_stack()->pop_into(val2);


		if (val1 == val2)
			scope->set_label_number(this->theTargetLabel);

		return 0;
	}


	int BranchNotEqualsVariant::execute(ExecutionScope* scope)
	{
		DataStack* dataStack = scope->get_data_stack();
		StackValue val1, val2;
		scope->get_data_stack()->pop_into(val1);
		scope->get_data_stack()->pop_into(val2);


		if (val1 != val2)
			scope->set_label_number(this->theTargetLabel);

		return 0;
	}

	int BranchLessThanVariant::execute(ExecutionScope* scope)
	{
		DataStack* dataStack = scope->get_data_stack();
		StackValue val1, val2;
		scope->get_data_stack()->pop_into(val1);
		scope->get_data_stack()->pop_into(val2);


		if (val1 < val2)
			scope->set_label_number(this->theTargetLabel);

		return 0;
	}

	int BranchLessThanOrEqualVariant::execute(ExecutionScope* scope)
	{
		DataStack* dataStack = scope->get_data_stack();
		StackValue val1, val2;
		scope->get_data_stack()->pop_into(val1);
		scope->get_data_stack()->pop_into(val2);


		if (val1 <= val2)
			scope->set_label_number(this->theTargetLabel);

		return 0;
	}

	int BranchGreaterThanVariant::execute(ExecutionScope* scope)
	{
		DataStack* dataStack = scope->get_data_stack();
		StackValue val1, val2;
		scope->get_data_stack()->pop_into(val1);
		scope->get_data_stack()->pop_into(val2);


		if (val1 > val2)
			scope->set_label_number(this->theTargetLabel);

		return 0;
	}

	int BranchGreaterThanOrEqualVariant::execute(ExecutionScope* scope)
	{
		DataStack* dataStack = scope->get_data_stack();
		StackValue val1, val2;
		scope->get_data_stack()->pop_into(val1);
		scope->get_data_stack()->pop_into(val2);


		if (val1 >= val2)
			scope->set_label_number(this->theTargetLabel);

		return 0;
	}
}
