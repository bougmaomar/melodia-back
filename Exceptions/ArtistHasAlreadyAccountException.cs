namespace melodia_api.Exceptions;

public class ArtistHasAlreadyAccountException : Exception
{
    public ArtistHasAlreadyAccountException(long? artistId) : base(
        $"The Artist with the {artistId} has already an account")
    {
    }
}