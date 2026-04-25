package db

import (
	"github.com/brinestone/data-mount/app/logger"
	logger_contracts "github.com/brinestone/data-mount/internal/contracts/logger"
)

// DbConfig holds the database configuration for the application.
type DbConfig struct {
	logger_contracts.UsesLogger
	DbUser         string  `env:"DB_USER,required"`
	DbPwd          string  `env:"DB_PWD,required"`
	DbHost         string  `env:"DB_HOST" envDefault:"localhost"`
	DbName         string  `env:"DB_NAME,required"`
	DbPort         uint    `env:"DB_PORT" envDefault:"5432"`
	DbSslMode      string  `env:"DB_SSL_MODE" envDefault:"disable"`
	migrationsPath *string `env:"DB_MIGRATIONS_PATH"`
}

func NewDbConfig(l *logger.Logger) *DbConfig {
	return &DbConfig{
		UsesLogger: logger_contracts.UsesLogger{Logger: l},
	}
}

func (c *DbConfig) WithMigrations(path string) *DbConfig {
	c.migrationsPath = &path
	return c
}
