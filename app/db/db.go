package db

import (
	"database/sql"
	"fmt"

	"github.com/golang-migrate/migrate/v4"
	"github.com/golang-migrate/migrate/v4/database/postgres"
	_ "github.com/golang-migrate/migrate/v4/source/file"
	_ "github.com/jackc/pgx/v5/stdlib"
)

type DBConn struct {
	*DbConfig
	Pool *sql.DB
}

func NewPool(config *DbConfig) *DBConn {
	var db *sql.DB
	cs := fmt.Sprintf("host=%s port=%d user=%s password=%s dbname=%s sslmode=disable",
		config.DbHost, config.DbPort, config.DbUser, config.DbPwd, config.DbName)
	db, err := sql.Open("pgx", cs)
	if err != nil {
		config.Logger.Error("error opening database connection: %v", err)
		panic(err)
	}

	if err := db.Ping(); err != nil {
		config.Logger.Error("error pinging database: %v", err)
		panic(err)
	}

	if config.migrationsPath != nil {
		if err := runMigration(db, *config.migrationsPath); err != nil {
			config.Logger.Error("error running migrations: %v", err)
			panic(err)
		}
	}
	return &DBConn{Pool: db, DbConfig: config}
}

// ClosePool closes the database connection pool.
func (db *DBConn) ClosePool() error {
	if db.Pool != nil {
		return db.Pool.Close()
	}
	return nil
}

// Run database migration files only if migration path is set
func runMigration(db *sql.DB, path string) error {
	driver, err := postgres.WithInstance(db, &postgres.Config{})
	if err != nil {
		return err
	}

	m, err := migrate.NewWithDatabaseInstance(
		fmt.Sprintf("file://%s", path),
		"postgres",
		driver,
	)
	if err != nil {
		return err
	}

	// Migrate up to the latest active version
	if err := m.Up(); err != nil && err != migrate.ErrNoChange {
		return err
	}

	return nil
}
