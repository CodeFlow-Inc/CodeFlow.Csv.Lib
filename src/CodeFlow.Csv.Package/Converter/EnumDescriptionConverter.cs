using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CodeFlow.Csv.Package.Converter;

/// <summary>
/// Converts an enum value to and from its description attribute or name.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EnumDescriptionConverter<T> : DefaultTypeConverter where T : struct, Enum
{
    /// <summary>
    /// Converts a string to an enum value based on the description attribute or name.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <param name="row">The CSV reader row.</param>
    /// <param name="memberMapData">The member map data.</param>
    /// <returns>The corresponding enum value.</returns>
    /// <exception cref="ArgumentException">Thrown when the description does not match any enum value.</exception>
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        ArgumentNullException.ThrowIfNull(text);

        string normalizedText = EnumDescriptionConverter<T>.NormalizeString(text);

        foreach (var field in typeof(T).GetFields())
        {
            if (field != null && Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (EnumDescriptionConverter<T>.NormalizeString(attribute.Description) == normalizedText)
                {
                    return (T)field.GetValue(null)!;
                }
            }
            else
            {
                if (field != null && EnumDescriptionConverter<T>.NormalizeString(field.Name) == normalizedText)
                {
                    return (T)field.GetValue(null)!;
                }
            }
        }
        throw new ArgumentException($"Descrição inválida '{text}' para o enum '{typeof(T).Name}'");
    }

    /// <summary>
    /// Converts an enum value to its description attribute or name for CSV output.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <param name="row">The CSV writer row.</param>
    /// <param name="memberMapData">The member map data.</param>
    /// <returns>The corresponding description or name as a string.</returns>
    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value is T enumValue)
        {
            var field = typeof(T).GetField(enumValue.ToString());
            if (field != null && Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }
            return enumValue.ToString();
        }
        return base.ConvertToString(value, row, memberMapData);
    }

    /// <summary>
    /// Normalizes a string by removing diacritics and converting to lowercase.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string NormalizeString(string input)
    {
        return new string(input
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray())
            .ToLowerInvariant();
    }
}
