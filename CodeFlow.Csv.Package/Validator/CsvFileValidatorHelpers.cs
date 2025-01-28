using CsvHelper.Configuration;
using System.Collections.Generic;

namespace CodeFlow.Csv.Package.Validator;

public static class CsvFileValidatorHelpers
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="classMap"></param>
    /// <returns></returns>
    public static IEnumerable<string> GetExpectedHeaders(ClassMap classMap)
    {
        var headers = new List<string>();

        foreach (var memberMap in classMap.MemberMaps)
        {
            headers.AddRange(memberMap.Data.Names);
        }

        foreach (var referenceMap in classMap.ReferenceMaps)
        {
            headers.AddRange(GetExpectedHeaders(referenceMap.Data.Mapping));
        }

        foreach (var parameterMap in classMap.ParameterMaps)
        {
            headers.AddRange(parameterMap.Data.Names);
        }

        return headers;
    }
}