namespace IManager.Web.Presentation.Validators.Shared;

public static class DocumentValidator
{
    private static string OnlyDigits(string value)
        => new(value.Where(char.IsDigit).ToArray());

    public static bool IsCpfValid(string cpf)
    {
        cpf = OnlyDigits(cpf);
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
            return false;

        int[] firstMultipliers = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] secondMultipliers = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        var numbers = cpf.Select(c => c - '0').ToArray();

        var sum = numbers.Take(9)
            .Select((n, i) => n * firstMultipliers[i])
            .Sum();
        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        sum = numbers.Take(9)
            .Concat([firstDigit])
            .Select((n, i) => n * secondMultipliers[i])
            .Sum();
        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return numbers[9] == firstDigit && numbers[10] == secondDigit;
    }

    public static bool IsCnpjValid(string cnpj)
    {
        cnpj = OnlyDigits(cnpj);

        if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1)
            return false;

        int[] firstMultipliers = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] secondMultipliers = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var sum = cnpj.Take(12).Select((d, i) => (d - '0') * firstMultipliers[i]).Sum();
        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        sum = cnpj.Take(13).Select((d, i) => (d - '0') * secondMultipliers[i]).Sum();
        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return cnpj[12] - '0' == firstDigit && cnpj[13] - '0' == secondDigit;
    }
}