using NetOpenAI.Models;

namespace NetOpenApi.Api
{
    public class FieldDefinitions
    {
        public static List<FieldDefinition> Definitions => new List<FieldDefinition>
        {
            new FieldDefinition
            {
                FieldName= "Device type",
                FieldJsonToken = "vendor",
                FieldDescription = "Contains manufacturer of the device user requests action for",
                PossibleValues = new List<string> { "fujifilm", "ricoh" }
            },
            new FieldDefinition
            {
                FieldName= "IP address",
                FieldJsonToken = "ipAddress",
                FieldDescription = "Contains IP address of the device",
            },
            new FieldDefinition
            {
                FieldName= "Serial Number",
                FieldJsonToken = "serialNumber",
                FieldDescription = "Serial number of the device",
            },
            new FieldDefinition
            {
                FieldName= "Output type",
                FieldJsonToken = "outputType",
                FieldDescription = "information about desired print protocol on the device via which the user wants to print",
                PossibleValues = new List<string> { "PostScript", "PCL", "PDF" }
            },
            new FieldDefinition
            {
                FieldName= "Authentication method",
                FieldJsonToken = "authType",
                FieldDescription = "information about authentication type on the device, it refers to way user wants to login to the device",
                PossibleValues = new List<string> { "Card", "CardOrPin", "UsernameAndPassword" }
            },
            new FieldDefinition
            {
                FieldName= "Printer name",
                FieldJsonToken = "name",
                FieldDescription = "What should be the printers name in the system",
            }
        };
    }
}
