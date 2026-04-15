namespace InvestmentControl.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key) : base($"Сущность '{name}' c id {key} не найдена.") { }
}
