using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Plugin.ExternalSuppliers.Bonvita.Models
{
    public class BonvitaModel
    {
        [Display(Name = "Endpoint address")]
        public string FtpHost { get; set; }

        [Display(Name = "Username")]
        public string User { get; set; }

        [Display(Name = "Password")]
        public string Pass { get; set; }

        [Display(Name = "CSV file name")]
        public string CSVFileName { get; set; }

        public string ErrorMessage { get; set; }
    }
}
