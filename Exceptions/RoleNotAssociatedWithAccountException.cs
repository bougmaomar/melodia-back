namespace melodia_api.Exceptions;

public class RoleNotAssociatedWithAccountException : Exception
{
    public RoleNotAssociatedWithAccountException(string email, string roleName) : base(
        $"The account associated with the email:'{email}' has no role: '{roleName}'")
    {
    }
}