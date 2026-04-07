namespace InvestmentControl.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Доступ запрещён.") { }
    public ForbiddenAccessException(string message) : base(message) { }
}
