using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeFlow.Csv.Package.Validator;

/// <summary>
/// 
/// </summary>
/// <param name="classMap"></param>
public class CsvFileValidator(ClassMap classMap)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool IsCsvFile(string fileName)
    {
        return fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileStream"></param>
    /// <param name="errors"></param>
    /// <returns></returns>
    public bool HasValidHeaders(Stream fileStream, CsvConfiguration csvConfiguration, out List<string> errors)
    {
        errors = [];

        using var reader = new StreamReader(fileStream, leaveOpen: true);
        var csv = new CsvReader(reader, csvConfiguration);
        csv.Context.RegisterClassMap(classMap);
        csv.Read();
        csv.ReadHeader();
        var headerRecord = csv.HeaderRecord;

        if (headerRecord == null)
        {
            errors.Add("The CSV file does not contain headers.");
            return false;
        }

        var headerRecordNormalized = headerRecord
            .SelectMany(header => header.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Select(header => header.Trim().ToLowerInvariant().Replace("\"", ""))
            .ToList();

        var expectedHeaders = CsvFileValidatorHelpers.GetExpectedHeaders(classMap);
        var expectedHeadersNormalized = expectedHeaders
            .Select(header => header.Trim().ToLowerInvariant().Replace("\"", ""))
            .ToList();

        if (!expectedHeadersNormalized.All(headerRecordNormalized.Contains))
        {
            errors.Add("The CSV file headers are incorrect.");
            return false;
        }

        return true;
    }
}
