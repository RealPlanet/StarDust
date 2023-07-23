#pragma once

#ifndef _SDVM_MACHINE_DIAGNOSTICS_H_
#define _SDVM_MACHINE_DIAGNOSTICS_H_

#include <string>
#include <vector>

namespace sdi
{
	class MachineDiagnostics
	{
		std::vector<std::string> m_loggedDiagnostics;

	public:
		void log(std::string msg) { m_loggedDiagnostics.push_back(msg); }
		void reset() { m_loggedDiagnostics.clear(); }
		void dump(std::ostream& stream) const;
	};
}

#endif // !_SDVM_MACHINE_DIAGNOSTICS_H_
