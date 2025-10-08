namespace OPME.StockManagement.Application.Exceptions;

public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message) : base(message) { }
}

public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(string entityName, Guid id) 
        : base($"{entityName} com ID {id} não encontrado") { }
}

public class EntityAlreadyExistsException : ApplicationException
{
    public EntityAlreadyExistsException(string entityName, string field, string value) 
        : base($"{entityName} com {field} '{value}' já existe") { }
}
