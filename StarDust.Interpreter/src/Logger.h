#pragma once

#ifndef _SDVM_LOGGING_H_
#define _SDVM_LOGGING_H_

#ifdef _SDVM_LOGGING_ENABLED_

// General
#define _SDVM_LOGF(message, ...)			::sdi::Logger::log_generic_normal(sdu::format(message, __VA_ARGS__))
#define _SDVM_LOGVF(message, ...)			::sdi::Logger::log_generic_verbose(sdu::format(message, __VA_ARGS__))
// Warn
#define _SDVM_LOGWF(message, ...)			::sdi::Logger::log_warning_normal(sdu::format(message, __VA_ARGS__))
// Error
#define _SDVM_LOGEF(message, ...)			::sdi::Logger::log_error_normal(sdu::format(message, __VA_ARGS__))

#define _SDVM_LOG(message)				_SDVM_LOGF(message, "")
#define _SDVM_LOGV(message)				_SDVM_LOGVF(message, "")

#define _SDVM_LOGW(message)				_SDVM_LOGWF(message, "")
#define _SDVM_LOGE(message)				_SDVM_LOGEF(message, "")

#else

#define _LOGF(message, ...)
#define _LOGFV(message, ...)
// Warn
#define _LOGWF(message, ...)
// Error
#define _LOGEF(message, ...)

#define _LOGG(message)
#define _LOGV(message)

#define _LOGWV(message)
#define _LOGEV(message)

#endif // _SDVM_LOGGING_ENABLED_

#ifdef  _SDVM_LOGGING_ENABLED_
namespace sdi
{
	class Logger
	{
	public:
		enum class _SDVM_API LogLevel
		{
			Unknown, // Placeholder
			Off = 0,
			Normal = 1,  // Always logged
			Verbose1 = 2,// Logged if on -> todo
			Verbose2 = 3, // Logged if on -> todo
		};
		/// <summary>
		/// Prepare logging
		/// </summary>
		/// <param name="logPath">The path without file name where to create the log file</param>
		_SDVM_API static void init(std::string logPath);

		static void log_generic_normal(std::string msg) { get()->log_generic(sdi::Logger::LogLevel::Normal, msg); }
		static void log_generic_verbose(std::string msg) { get()->log_generic(sdi::Logger::LogLevel::Verbose1, msg); }

		static void log_warning_normal(std::string msg) { get()->log_warning(sdi::Logger::LogLevel::Normal, msg); }
		static void log_error_normal(std::string msg) { get()->log_error(sdi::Logger::LogLevel::Normal, msg); }

		static void set_log_level(LogLevel newLvl) { get()->m_eCurrentLogLevel = (newLvl); }

		static std::ostream& get_fstream() { return get()->m_sDiagnosticFile; }
	private:
		const std::string current_date_time();
		const std::string get_log_prefix(LogLevel lvl, char type);
		void create_log_file();

		void log(LogLevel lvl, char type, std::string msg);
		inline void log_generic(LogLevel lvl, std::string msg) { log(lvl, 'G', msg); }
		inline void log_warning(LogLevel lvl, std::string msg) { log(lvl, 'W', msg); }
		inline void log_error(LogLevel lvl, std::string msg) { log(lvl, 'E', msg); }

	private:
		LogLevel m_eCurrentLogLevel = LogLevel::Off;
		std::fstream m_sDiagnosticFile;

		static std::string m_logPath;

		static bool m_InstanceInit;
		static bool m_InitRan;

		static std::shared_ptr<Logger> m_ptrDiagnostics;

		static std::shared_ptr<Logger>& get()
		{
			if (!m_InitRan)
				throw std::exception("Must call Init() method before invoking any log function!");

			if (!m_InstanceInit) {
				m_ptrDiagnostics = std::make_shared<Logger>();
				m_ptrDiagnostics->create_log_file();
				m_InstanceInit = true;
			}

			return m_ptrDiagnostics;
		}
	};
}
#else
// Empty definitions used by other DLLs which dont need logging
namespace sdi
{
	class _SDVM_API Logger
	{
	public:
		static void init(std::string str) {}
	};
}
#endif

#endif // !_SDVM_LOGGING_H_
