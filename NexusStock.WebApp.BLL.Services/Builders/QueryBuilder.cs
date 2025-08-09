using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace NexusStock.WebApp.BLL.Services.Builders
{
    internal class QueryBuilder
    {
        private readonly List<KeyValuePair<string, string>> _params = new();
        private readonly Uri _baseAddress;
        private readonly string _basePath;
        private readonly ILogger? _logger;
        private CultureInfo? _culture;
        private bool _normalizeNames = false;
        private bool _useEnumStringValue = false;

        public QueryBuilder(Uri baseAddress, string basePath, ILogger? logger = null)
        {
            _baseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));
            _basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
            _logger = logger;
            _culture = CultureInfo.InvariantCulture;
            _normalizeNames = false;
            _useEnumStringValue = false;
        }

        /// <summary>
        /// Включает автоматическую нормализацию имен параметров (в нижний регистр)
        /// </summary>
        public QueryBuilder WithNameNormalization(bool normalize = true)
        {
            _normalizeNames = normalize;
            return this;
        }

        /// <summary>
        /// Устанавливает культуру для форматирования значений
        /// </summary>
        public QueryBuilder WithCulture(CultureInfo culture)
        {
            _culture = culture;
            return this;
        }

        /// <summary>
        /// Использовать строковые значения перечислений вместо числовых
        /// </summary>
        public QueryBuilder UseEnumStringValues(bool useStringValue = true)
        {
            _useEnumStringValue = useStringValue;
            return this;
        }

        public QueryBuilder AddDateParam(string name, DateTimeOffset? date, string format = "O")
        {
            if (date.HasValue)
            {
                AddParam(name, date.Value, format);
            }
            return this;
        }

        public QueryBuilder AddListParam<T>(string name, IEnumerable<T>? values)
        {
            if (values?.Any() == true)
            {
                foreach (var value in values)
                {
                    AddParam(name, value);
                }
            }
            return this;
        }

        public QueryBuilder AddParam<T>(string name, T? value, string? format = null)
        {
            if (value == null) return this;

            string processedName = ProcessName(name);
            string stringValue = FormatValue(value, format);

            _params.Add(new KeyValuePair<string, string>(processedName, stringValue));
            return this;
        }

        public QueryBuilder AddEnumParam<T>(string name, T? value) where T : struct, Enum
        {
            if (!value.HasValue) return this;

            string stringValue = _useEnumStringValue
                ? value.Value.ToString()
                : Convert.ToInt32(value.Value).ToString();

            return AddParam(name, stringValue);
        }

        public QueryBuilder AddBoolParam(string name, bool? value, string trueValue = "true", string falseValue = "false")
        {
            if (!value.HasValue) return this;
            return AddParam(name, value.Value ? trueValue : falseValue);
        }

        public Uri Build()
        {
            var baseUri = new Uri(_baseAddress, _basePath);
            string uriString = baseUri.ToString();

            if (_params.Count > 0)
            {
                uriString = QueryHelpers.AddQueryString(uriString, _params!);
            }

            _logger?.LogDebug("[QueryBuilder] Built URL: {Url}", uriString);
            return new Uri(uriString);
        }

        private string ProcessName(string name)
        {
            return _normalizeNames ? name.ToLowerInvariant() : name;
        }

        private string FormatValue(object value, string? format = null)
        {
            return value switch
            {
                IFormattable formattable =>
                    formattable.ToString(format, _culture),
                _ => value.ToString() ?? string.Empty
            };
        }
    }
}
