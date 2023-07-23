#include "stdafx.h"
#include "MachineDiagnostics.h"
namespace sdi
{
	void MachineDiagnostics::dump(std::ostream& stream) const
	{
		stream << "Begin diagnostic dump!" << '\n';
		for (auto& log : m_loggedDiagnostics)
			stream << log << '\n';

		stream << "End diagnostic dump!" << '\n';
	}
}

