package logger

import (
	"log/slog"
	"os"
	"path"
)

type Logger struct {
	*slog.Logger
	level LogLevel
}

// func (l *Logger) HttpMiddleware() gin.HandlerFunc {
// 	return func(c *gin.Context) {
// 		go func() {
// 		<- c.Request.Context().Done()

// 		err := c.Request.Context().Err()
// 		if err != nil {
// 			if c.Request.Response.StatusCode >= 400 && c.Request.Response.StatusCode < 500 {
// 				l.Warn()
// 			}
// 		}
// 		}()
// 		c.Next()
// 	}
// }

func (l *Logger) WithGroup(name string) *Logger {
	return &Logger{
		Logger: l.Logger.WithGroup(name),
		level:  l.level,
	}
}

func NewLogger(config LoggerConfig) *Logger {
	wd, _ := os.Getwd()
	ld := path.Join(wd, config.LogDir)
	err := os.MkdirAll(ld, 0755)
	if err != nil {
		panic(err)
	}
	errorFile, _ := os.OpenFile(path.Join(ld, "error.log"), os.O_CREATE|os.O_WRONLY|os.O_APPEND, 0666)
	accesFile, _ := os.OpenFile(path.Join(ld, "access.log"), os.O_CREATE|os.O_WRONLY|os.O_APPEND, 0666)

	nonErrorHandler := slog.NewMultiHandler(
		slog.NewJSONHandler(os.Stdout, &slog.HandlerOptions{
			Level: slog.LevelInfo,
		}),
		slog.NewJSONHandler(accesFile, &slog.HandlerOptions{
			Level: slog.LevelInfo,
		}),
	)
	errorHandler := slog.NewMultiHandler(
		slog.NewJSONHandler(os.Stderr, &slog.HandlerOptions{
			Level: slog.LevelError,
		}),
		slog.NewJSONHandler(errorFile, &slog.HandlerOptions{
			Level: slog.LevelError,
		}),
	)

	logger := slog.New(slog.NewMultiHandler(nonErrorHandler, errorHandler))
	return &Logger{
		Logger: logger,
		level:  config.Level,
	}
}
