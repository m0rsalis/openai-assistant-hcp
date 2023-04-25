using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetOpenAI.Models
{
    public class FieldDefinition
    {
        public string FieldName { get; set; }
        public string FieldJsonToken { get; set; }
        public string FieldDescription { get; set; }
        public List<string> PossibleValues { get; set; }

        public override string ToString()
        {
            var possibleValuesStr = PossibleValues != null && PossibleValues.Any()
                ? "It should have one of the following values: " + string.Join(",", PossibleValues)
                : "";

            return $"""Field is named {FieldName} and should be represented in json as {FieldJsonToken}. This field contains {FieldDescription}. {possibleValuesStr}\n""";
        }
    }
}
