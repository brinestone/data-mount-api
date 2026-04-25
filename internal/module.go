package internal

import "github.com/gin-gonic/gin"

type Module interface {
	Mount(*gin.RouterGroup)
	RootPath() string
}
