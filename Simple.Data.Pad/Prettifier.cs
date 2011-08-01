using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Simple.Data.Pad
{
    public class Prettifier
    {
        private readonly PluralizationService _pluralization = PluralizationService.CreateService(CultureInfo.CurrentCulture);
        private readonly Regex _nonAlphaNumeric = new Regex("[^0-9a-zA-Z]+");
        private readonly ConcurrentDictionary<string, string> _prettifiedCache = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string> _prettifiedPluralCache = new ConcurrentDictionary<string, string>();

        public string Prettify(string source)
        {
            return _prettifiedCache.GetOrAdd(source, PrettifyImpl);
        }

        private string PrettifyImpl(string source)
        {
            if (!_nonAlphaNumeric.IsMatch(source))
                return _pluralization.IsSingular(source) ? source : _pluralization.Singularize(source);

            var parts = _nonAlphaNumeric.Replace(source, " ").Split(' ');
            parts[parts.Length - 1] = _pluralization.IsSingular(source)
                                          ? parts[parts.Length - 1]
                                          : _pluralization.Singularize(parts[parts.Length - 1]);

            return string.Join("", parts.Select(ToPascalCase));
        }

        public string PluralizeAndPrettify(string source)
        {
            return _prettifiedPluralCache.GetOrAdd(source, PluralizeAndPrettifyImpl);
        }

        private string PluralizeAndPrettifyImpl(string source)
        {
            if (!_nonAlphaNumeric.IsMatch(source))
                return ToPascalCase(_pluralization.IsPlural(source) ? source : _pluralization.Pluralize(source));

            var parts = _nonAlphaNumeric.Replace(source, " ").Split(' ');
            parts[parts.Length - 1] = _pluralization.IsPlural(source)
                                          ? parts[parts.Length - 1]
                                          : _pluralization.Pluralize(parts[parts.Length - 1]);

            return string.Join("", parts.Select(ToPascalCase));
        }

        private static string ToPascalCase(string source)
        {
            return char.ToUpper(source[0], CultureInfo.CurrentCulture) +
                   source.Substring(1).ToLower(CultureInfo.CurrentCulture);
        }
    }
}
