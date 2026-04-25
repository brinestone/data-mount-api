package logger

import "log/slog"

type LogLevel string

const (
	LevelDebug   LogLevel = "debug"
	LevelInfo    LogLevel = "info"
	LevelWarning LogLevel = "warning"
	LevelError   LogLevel = "error"
)

func ParseLogLevel(level string) LogLevel {
	switch level {
	case "info":
		return LevelInfo
	case "warning":
		return LevelWarning
	case "error":
		return LevelError
	default:
		return LevelDebug
	}
}

func (l LogLevel) toSlogLevel() slog.Level {
	switch l {
	case LevelInfo:
		return slog.LevelInfo
	case LevelWarning:
		return slog.LevelWarn
	case LevelError:
		return slog.LevelError
	default:
		return slog.LevelDebug
	}
}

type LoggerConfig struct {
	Level    LogLevel
	Handlers []slog.Handler
	LogDir   string
}
