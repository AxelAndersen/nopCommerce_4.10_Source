using System;

namespace Nop.Plugin.ExternalSuppliers.Intersurf.Components
{
    internal class Validation
    {
        internal static void ValidateSettings(IntersurfSettings intersurfSettings)
        {
            if (intersurfSettings == null)
            {
                throw new Exception("No Intersurf settings found, aborting task");
            }

            if (string.IsNullOrEmpty(intersurfSettings.EndpointAddress))
            {
                throw new Exception("No EndpointAddress found in Intersurf settings, aborting task");
            }

            if (string.IsNullOrEmpty(intersurfSettings.Username))
            {
                throw new Exception("No Username found in Intersurf settings, aborting task");
            }

            if (string.IsNullOrEmpty(intersurfSettings.Password))
            {
                throw new Exception("No Password found in Intersurf settings, aborting task");
            }

            if (string.IsNullOrEmpty(intersurfSettings.CSVFileName))
            {
                throw new Exception("No CSVFileName found in Intersurf settings, aborting task");
            }

            if (intersurfSettings.CSVFileName.EndsWith(".csv") == false)
            {
                throw new Exception("CSVFileName must end with '.csv', aborting task");
            }
        }
    }
}
