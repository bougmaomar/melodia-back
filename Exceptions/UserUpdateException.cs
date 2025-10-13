namespace melodia_api.Exceptions;

public class UserUpdateException : Exception
{
    public UserUpdateException(string? message = null) : base(message ?? "An error occurred while updating the user.")
    {
    }
}