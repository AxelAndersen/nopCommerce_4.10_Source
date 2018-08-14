using Nop.Core.Configuration;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.POS.Kaching
{
    public class POSKachingService
    {       
        private POSKachingSettings _posKachingSettings;        

        public POSKachingService(POSKachingSettings kachingSettings)
        {           
            _posKachingSettings = kachingSettings;
        }

        public void SaveProduct(string json)
        {
            //var test = _settingService.GetSettingByKey<POSKachingSettings>("Test");

            var posKaChingAccountToken = _posKachingSettings.POSKaChingAccountToken;

            var test = "";
            //if(string.IsNullOrEmpty(posKaChingAccountToken.Value))
            //{
            //    _posKachingSettings.POSKaChingAccountToken = "dd";
            //    _settingService.SetSetting("POSKaChingAccountToken", _posKachingSettings.POSKaChingAccountToken);
            //}
            

            
        }
    }
}
