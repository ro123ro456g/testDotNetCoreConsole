using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoreTestConsoleApp.Model
{
    public class PackageConfigModel
    {
        [JsonPropertyName("Package Version")]
        public string PackageVersion { get; set; }

        [JsonPropertyName("Software Amount")]
        public int SoftwareAmount { get; set; }

        [JsonPropertyName("Software List")]
        public List<SoftwareModel> SoftwareList { get; set; }
    }

    public class SoftwareModel
    {
        [JsonPropertyName("Software Name")]
        public string SoftwareName { get; set; }

        [JsonPropertyName("Software Version")]
        public string SoftwareVersion { get; set; }

        [JsonPropertyName("Software Path")]
        public string SoftwarePath { get; set; }

        [JsonPropertyName("Software Launch")]
        public bool SoftwareLaunch { get; set; }
    }
}
