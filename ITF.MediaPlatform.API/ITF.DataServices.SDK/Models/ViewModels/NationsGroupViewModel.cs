using System;

namespace ITF.DataServices.SDK.Models.ViewModels
{
    public class NationsGroupViewModel : NationCoreViewModelOld
    {
        public string DivisionCode { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneCodeMaster { get; set; }
    }
}
