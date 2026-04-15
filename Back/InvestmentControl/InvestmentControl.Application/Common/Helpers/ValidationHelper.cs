namespace InvestmentControl.Application.Common.Helpers;

public static class ValidationHelper
{
    public static void EnsurePositiveIds(List<int>? ids, string fieldName)
    {
        if (ids == null) return;
        if (ids.Any(id => id <= 0))
            throw new ArgumentException($"{fieldName} не могут содержать отрицательные или нулевые значения.");
    }
}
