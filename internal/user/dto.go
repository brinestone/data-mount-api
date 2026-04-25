package user

type GetUsersRequest struct {
	Page  int `json:"page" default:"0" min:"0"`
	Limit int `json:"limit" default:"100" min:"1"`
}
