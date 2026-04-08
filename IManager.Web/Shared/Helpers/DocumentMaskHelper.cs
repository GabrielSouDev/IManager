using System.Globalization;
using System.Text;

namespace IManager.Web.Shared.Helpers;

public static class DocumentMaskHelper
{
    public static string MaskCpf(string value)
    {
        var digits = OnlyDigits(value).PadLeft(11, '0');

        if (digits.Length != 11)
            throw new ArgumentException("CPF inválido.");

        return $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..11]}";
    }

    public static string MaskCnpj(string value)
    {
        var digits = OnlyDigits(value).PadLeft(14, '0');

        if (digits.Length != 14)
            throw new ArgumentException("CNPJ inválido.");

        return $"{digits[..2]}.{digits[2..5]}.{digits[5..8]}/{digits[8..12]}-{digits[12..14]}";
    }

    public static string MaskDocument(string value)
    {
        var digits = OnlyDigits(value);

        return digits.Length switch
        {
            <= 11 => MaskCpf(digits),
            <= 14 => MaskCnpj(digits),
            _ => value
        };
    }

    private static string OnlyDigits(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var sb = new StringBuilder(value.Length);

        foreach (var c in value)
            if (char.IsDigit(c))
                sb.Append(c);

        return sb.ToString();
    }
}