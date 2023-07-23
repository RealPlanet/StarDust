#include "stdafx.h"

#include "Logger.h"

#include "Date.h"

namespace sdi
{
	std::shared_ptr<Logger> Logger::m_ptrDiagnostics;
	std::string				Logger::m_logPath		= "";
	bool					Logger::m_InitRan		= false;
	bool					Logger::m_InstanceInit	= false;

	void Logger::init(std::string logPath)
	{
		if (m_InitRan)
			return;

		m_logPath = logPath;
		m_InitRan = true;
	}

	const std::string Logger::current_date_time()
	{
		return date::format("%F %R", std::chrono::system_clock::now());
	}

	const std::string Logger::get_log_prefix(LogLevel lvl, char type)
	{
		auto a = std::chrono::system_clock::now();
		return ">> [" + current_date_time() + "] - [" + type + "] - <<";
	}

	void Logger::create_log_file()
	{
		m_sDiagnosticFile.open(m_logPath + "\\SDVMDiagnostic.log", std::ios::out | std::ios::app);
	}

	void Logger::log(LogLevel lvl, char type, std::string msg)
	{
		if (m_eCurrentLogLevel < lvl)
			return;

		Logger::get()->m_sDiagnosticFile << get_log_prefix(lvl, type) << " " << msg << '\n';
		Logger::get()->m_sDiagnosticFile.flush();
	}
}
