-- name: ListUsers :many
SELECT
    *
FROM
    users
WHERE
    deleted_at IS NULL
ORDER BY
    added_at DESC,
    updated_at DESC
LIMIT
    @size
OFFSET
    @page;