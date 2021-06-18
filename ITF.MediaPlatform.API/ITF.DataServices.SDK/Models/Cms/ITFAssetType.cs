using System.ComponentModel.DataAnnotations;

namespace ITF.DataServices.SDK.Models.Cms
{
    public class ITFAssetType
    {
        public string assemblyName { get; set; }

        public short? assetTypeGroupId { get; set; }

        [Key]
        public short assetTypeId { get; set; }

        public string assetTypeName { get; set; }
        public string displayControlPath { get; set; }
        public string interfaceName { get; set; }
        public string searchControlPath { get; set; }
        public string typeName { get; set; }
    }
}
