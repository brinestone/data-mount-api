package user

import (
	"github.com/brinestone/data-mount/internal"
	db_contracts "github.com/brinestone/data-mount/internal/contracts/db"
	log_contracts "github.com/brinestone/data-mount/internal/contracts/logger"
	user_data "github.com/brinestone/data-mount/internal/user/data"
	"github.com/gin-gonic/gin"
)

type userModule struct {
	UserModuleConfig
	queries *user_data.Queries
}

type UserModuleConfig struct {
	db_contracts.UsesDatabase
	log_contracts.UsesLogger
}

func NewUserModule(c UserModuleConfig) internal.Module {
	return &userModule{
		UserModuleConfig: c,
		queries:          user_data.New(c.Db.Pool),
	}
}

func (m *userModule) RootPath() string {
	return "/users"
}

func (m *userModule) Mount(r *gin.RouterGroup) {
	r.GET("", m.listUsers)
}

func (m *userModule) listUsers(c *gin.Context) {
	var args GetUsersRequest
	err := c.BindQuery(&args)
	if err != nil {
		c.AbortWithStatus(400)
		return
	}
	result, err := m.queries.ListUsers(c.Request.Context(), user_data.ListUsersParams{
		Page: int32(args.Page),
		Size: int32(args.Limit),
	})
	if err != nil {
		c.AbortWithStatus(500)
		return
	}
	c.JSON(200, result)
}
