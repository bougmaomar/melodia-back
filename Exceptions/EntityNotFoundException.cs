namespace melodia_api.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entity, string propertyName, string propertyValue) : base(
        $"The {entity} with the {propertyName}:'{propertyValue}' is not found.🤦‍♂️")
    {
    }
}