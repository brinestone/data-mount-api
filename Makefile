.PHONY: clean dev drop model-gen down
ifneq (,$(wildcard ./.env))
    include .env
    export
endif
DB_PORT ?=5432
DB_SSLMODE ?= disable
DB_HOST ?= localhost
DB_URL = "postgresql://$(DB_USER):$(DB_PWD)@$(DB_HOST):$(DB_PORT)/$(DB_NAME)?sslmode=$(DB_SSLMODE)"
model-gen:
	@sqlc generate
drop:
	@migrate -database "$(DB_URL)" -path sql/users/migrations drop
down:
	@migrate -database "$(DB_URL)" -path sql/users/migrations down 1
up:
	@migrate -database "$(DB_URL)" -path sql/users/migrations up
migrate:
	@migrate create -ext sql -dir $(M_DIR) $(M_NAME)
dev:
	@air
clean:
	$(RM) -r bin temp
