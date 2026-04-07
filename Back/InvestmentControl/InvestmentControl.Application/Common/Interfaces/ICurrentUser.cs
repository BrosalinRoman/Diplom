namespace InvestmentControl.Application.Common.Interfaces;

public interface ICurrentUser
{
    int UserId { get; }
    string Role { get; }
}
