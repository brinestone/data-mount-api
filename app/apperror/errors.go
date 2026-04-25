package apperror

type AppError struct {
	Status  int    `json:"status"`
	Message string `json:"message"`
}

type BadRequestError struct {
	AppError
}

func NewBadRequestError(message string) *BadRequestError {
	return &BadRequestError{
		AppError: AppError{
			Status:  400,
			Message: message,
		},
	}
}
