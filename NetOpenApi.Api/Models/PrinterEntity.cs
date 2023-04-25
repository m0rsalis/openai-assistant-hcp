using System.Text.Json.Serialization;

namespace NetOpenApi.Api.Models
{
    public class PrinterEntity
    {
        [JsonPropertyName("vendor")]
        public string Vendor { get; set; }
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }
        [JsonPropertyName("outputType")]
        public string OutputType { get; set; }
        [JsonPropertyName("authType")]
        public string AuthType { get; set; }
        [JsonPropertyName("name")] 
        public string Name { get; set; }

        public List<string> GetEmptyFieldNames()
        {
            var emptyFieldNames = new List<string>();

            if (string.IsNullOrEmpty(Vendor))
            {
                emptyFieldNames.Add(nameof(Vendor));
            }
            if (string.IsNullOrEmpty(IpAddress))
            {
                emptyFieldNames.Add(nameof(IpAddress));
            }
            if (string.IsNullOrEmpty(SerialNumber))
            {
                emptyFieldNames.Add(nameof(SerialNumber));
            }
            if (string.IsNullOrEmpty(OutputType))
            {
                emptyFieldNames.Add(nameof(OutputType));
            }
            if (string.IsNullOrEmpty(AuthType))
            {
                emptyFieldNames.Add(nameof(AuthType));
            }
            if (string.IsNullOrEmpty(Name))
            {
                emptyFieldNames.Add(nameof(Name));
            }

            return emptyFieldNames;
        }
    }
}
