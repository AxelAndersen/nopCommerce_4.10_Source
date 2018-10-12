using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.ModelBinders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Api.Models.CustomersParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ManufacturerParametersModel>))]
    public class ManufacturerParametersModel
    {

    }
}
