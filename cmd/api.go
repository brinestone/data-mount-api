package main

import (
	"github.com/brinestone/data-mount/app/db"
	"github.com/brinestone/data-mount/app/logger"
	"github.com/brinestone/data-mount/internal"
	db_contracts "github.com/brinestone/data-mount/internal/contracts/db"
	log_contracts "github.com/brinestone/data-mount/internal/contracts/logger"
	"github.com/brinestone/data-mount/internal/user"
	"github.com/caarlos0/env/v11"
	"github.com/gin-gonic/gin"
)

type apiConfig struct {
	Port        string `env:"PORT"`
	LogLevel    string `env:"LOG_LEVEL"`
	Environment string `env:"ENV"`
	LogDir      string `env:"LOG_DIR" envDefault:"logs"`
}

type ApiResources struct {
	Logger   *logger.Logger
	Database *db.DBConn
}

func loadApiConfig() (*apiConfig, error) {
	cfg := apiConfig{}
	err := env.Parse(&cfg)
	return &cfg, err
}

func loadModules(r *ApiResources) []internal.Module {
	return []internal.Module{
		user.NewUserModule(user.UserModuleConfig{
			UsesDatabase: db_contracts.UsesDatabase{
				Db: r.Database,
			},
			UsesLogger: log_contracts.UsesLogger{
				Logger: r.Logger,
			},
		}),
	}
}

func configureApi(engine *gin.Engine, modules []internal.Module) {
	for _, mod := range modules {
		r := engine.Group(mod.RootPath())
		mod.Mount(r)
	}
}
