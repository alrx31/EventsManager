namespace EventManagement.Middlewares
{
    public class NotFoundException : Exception
    {
        private const string DefaultMessage = "The requested resource was not found.";

        public NotFoundException() : base(DefaultMessage) { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class AlreadyExistsException : Exception
    {
        private const string DefaultMessage = "The resource already exists.";

        public AlreadyExistsException() : base(DefaultMessage) { }
        public AlreadyExistsException(string message) : base(message) { }
        public AlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ValidationException : Exception
    {
        private const string DefaultMessage = "Validation failed.";

        public ValidationException() : base(DefaultMessage) { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UnauthorizedException : Exception
    {
        private const string DefaultMessage = "Unauthorized access.";

        public UnauthorizedException() : base(DefaultMessage) { }
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ForbiddenException : Exception
    {
        private const string DefaultMessage = "Forbidden access.";

        public ForbiddenException() : base(DefaultMessage) { }
        public ForbiddenException(string message) : base(message) { }
        public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BadRequestException : Exception
    {
        private const string DefaultMessage = "Bad request.";

        public BadRequestException() : base(DefaultMessage) { }
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
    }

    

    public class InternalServerErrorException : Exception
    {
        private const string DefaultMessage = "Internal server error.";

        public InternalServerErrorException() : base(DefaultMessage) { }
        public InternalServerErrorException(string message) : base(message) { }
        public InternalServerErrorException(string message, Exception innerException) : base(message, innerException) { }
    }
}