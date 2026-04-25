package main

import (
	"fmt"

	"github.com/brinestone/data-mount/app/db"
	"github.com/brinestone/data-mount/app/logger"
	"github.com/caarlos0/env/v11"
	"github.com/gin-gonic/gin"
)

func loadDbConfig() (db.DbConfig, error) {
	cfg := db.DbConfig{}
	err := env.Parse(&cfg)
	return cfg, err
}

func main() {
	c, err := loadApiConfig()
	if err != nil {
		panic(err)
	}
	l := logger.NewLogger(logger.LoggerConfig{
		Level:  logger.ParseLogLevel(c.LogLevel),
		LogDir: c.LogDir,
	})
	dbCfg, err := loadDbConfig()
	if err != nil {
		panic(err)
	}
	conn := db.NewPool(&dbCfg)
	defer conn.ClosePool()
	if c.Environment == "dev" {
		gin.SetMode(gin.DebugMode)
	} else {
		gin.SetMode(gin.ReleaseMode)
	}
	r := gin.Default()

	modules := loadModules(&ApiResources{
		Logger:   l,
		Database: conn,
	})
	configureApi(r, modules)

	r.Run(fmt.Sprintf(":%s", c.Port))
}
