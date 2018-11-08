﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     //
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GLSReference
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://gls.dk/webservices/", ConfigurationName="GLSReference.wsShopFinderSoap")]
    public interface wsShopFinderSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://gls.dk/webservices/GetOneParcelShop", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<GLSReference.PakkeshopData> GetOneParcelShopAsync(string ParcelShopNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://gls.dk/webservices/GetAllParcelShops", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<GLSReference.GetAllParcelShopsResponse> GetAllParcelShopsAsync(GLSReference.GetAllParcelShopsRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://gls.dk/webservices/GetParcelShopsInZipcode", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<GLSReference.GetParcelShopsInZipcodeResponse> GetParcelShopsInZipcodeAsync(GLSReference.GetParcelShopsInZipcodeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://gls.dk/webservices/SearchNearestParcelShops", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<GLSReference.ParcelShopSearchResult> SearchNearestParcelShopsAsync(string street, string zipcode, string countryIso3166A2, int Amount);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://gls.dk/webservices/GetParcelShopDropPoint", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<GLSReference.ParcelShopSearchResult> GetParcelShopDropPointAsync(string street, string zipcode, string countryIso3166A2, int Amount);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gls.dk/webservices/")]
    public partial class PakkeshopData
    {
        
        private string numberField;
        
        private string companyNameField;
        
        private string streetnameField;
        
        private string streetname2Field;
        
        private string zipCodeField;
        
        private string cityNameField;
        
        private string countryCodeField;
        
        private string countryCodeISO3166A2Field;
        
        private string telephoneField;
        
        private string longitudeField;
        
        private string latitudeField;
        
        private int distanceMetersAsTheCrowFliesField;
        
        private Weekday[] openingHoursField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string CompanyName
        {
            get
            {
                return this.companyNameField;
            }
            set
            {
                this.companyNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Streetname
        {
            get
            {
                return this.streetnameField;
            }
            set
            {
                this.streetnameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string Streetname2
        {
            get
            {
                return this.streetname2Field;
            }
            set
            {
                this.streetname2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string ZipCode
        {
            get
            {
                return this.zipCodeField;
            }
            set
            {
                this.zipCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string CityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string CountryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public string CountryCodeISO3166A2
        {
            get
            {
                return this.countryCodeISO3166A2Field;
            }
            set
            {
                this.countryCodeISO3166A2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public string Telephone
        {
            get
            {
                return this.telephoneField;
            }
            set
            {
                this.telephoneField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=9)]
        public string Longitude
        {
            get
            {
                return this.longitudeField;
            }
            set
            {
                this.longitudeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=10)]
        public string Latitude
        {
            get
            {
                return this.latitudeField;
            }
            set
            {
                this.latitudeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=11)]
        public int DistanceMetersAsTheCrowFlies
        {
            get
            {
                return this.distanceMetersAsTheCrowFliesField;
            }
            set
            {
                this.distanceMetersAsTheCrowFliesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=12)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Weekday[] OpeningHours
        {
            get
            {
                return this.openingHoursField;
            }
            set
            {
                this.openingHoursField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gls.dk/webservices/")]
    public partial class Weekday
    {
        
        private Day dayField;
        
        private Hour openAtField;
        
        private Break[] breaksField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public Day day
        {
            get
            {
                return this.dayField;
            }
            set
            {
                this.dayField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public Hour openAt
        {
            get
            {
                return this.openAtField;
            }
            set
            {
                this.openAtField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=2)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Break[] breaks
        {
            get
            {
                return this.breaksField;
            }
            set
            {
                this.breaksField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gls.dk/webservices/")]
    public enum Day
    {
        
        /// <remarks/>
        Monday,
        
        /// <remarks/>
        Tuesday,
        
        /// <remarks/>
        Wednesday,
        
        /// <remarks/>
        Thursday,
        
        /// <remarks/>
        Friday,
        
        /// <remarks/>
        Saturday,
        
        /// <remarks/>
        Sunday,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gls.dk/webservices/")]
    public partial class Hour
    {
        
        private string fromField;
        
        private string toField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string From
        {
            get
            {
                return this.fromField;
            }
            set
            {
                this.fromField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string To
        {
            get
            {
                return this.toField;
            }
            set
            {
                this.toField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gls.dk/webservices/")]
    public partial class ParcelShopSearchResult
    {
        
        private AccuracyLevel accuracylevelField;
        
        private PakkeshopData[] parcelshopsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public AccuracyLevel accuracylevel
        {
            get
            {
                return this.accuracylevelField;
            }
            set
            {
                this.accuracylevelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=1)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public PakkeshopData[] parcelshops
        {
            get
            {
                return this.parcelshopsField;
            }
            set
            {
                this.parcelshopsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gls.dk/webservices/")]
    public enum AccuracyLevel
    {
        
        /// <remarks/>
        EXACT,
        
        /// <remarks/>
        STREET,
        
        /// <remarks/>
        ZIP,
        
        /// <remarks/>
        UNKNOWN,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gls.dk/webservices/")]
    public partial class Break
    {
        
        private string startField;
        
        private string endField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Start
        {
            get
            {
                return this.startField;
            }
            set
            {
                this.startField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string End
        {
            get
            {
                return this.endField;
            }
            set
            {
                this.endField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAllParcelShops", WrapperNamespace="http://gls.dk/webservices/", IsWrapped=true)]
    public partial class GetAllParcelShopsRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gls.dk/webservices/", Order=0)]
        public string countryIso3166A2;
        
        public GetAllParcelShopsRequest()
        {
        }
        
        public GetAllParcelShopsRequest(string countryIso3166A2)
        {
            this.countryIso3166A2 = countryIso3166A2;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAllParcelShopsResponse", WrapperNamespace="http://gls.dk/webservices/", IsWrapped=true)]
    public partial class GetAllParcelShopsResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gls.dk/webservices/", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public GLSReference.PakkeshopData[] GetAllParcelShopsResult;
        
        public GetAllParcelShopsResponse()
        {
        }
        
        public GetAllParcelShopsResponse(GLSReference.PakkeshopData[] GetAllParcelShopsResult)
        {
            this.GetAllParcelShopsResult = GetAllParcelShopsResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetParcelShopsInZipcode", WrapperNamespace="http://gls.dk/webservices/", IsWrapped=true)]
    public partial class GetParcelShopsInZipcodeRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gls.dk/webservices/", Order=0)]
        public string zipcode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gls.dk/webservices/", Order=1)]
        public string countryIso3166A2;
        
        public GetParcelShopsInZipcodeRequest()
        {
        }
        
        public GetParcelShopsInZipcodeRequest(string zipcode, string countryIso3166A2)
        {
            this.zipcode = zipcode;
            this.countryIso3166A2 = countryIso3166A2;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetParcelShopsInZipcodeResponse", WrapperNamespace="http://gls.dk/webservices/", IsWrapped=true)]
    public partial class GetParcelShopsInZipcodeResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gls.dk/webservices/", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public GLSReference.PakkeshopData[] GetParcelShopsInZipcodeResult;
        
        public GetParcelShopsInZipcodeResponse()
        {
        }
        
        public GetParcelShopsInZipcodeResponse(GLSReference.PakkeshopData[] GetParcelShopsInZipcodeResult)
        {
            this.GetParcelShopsInZipcodeResult = GetParcelShopsInZipcodeResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface wsShopFinderSoapChannel : GLSReference.wsShopFinderSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class wsShopFinderSoapClient : System.ServiceModel.ClientBase<GLSReference.wsShopFinderSoap>, GLSReference.wsShopFinderSoap
    {
        
    /// <summary>
    /// Implement this partial method to configure the service endpoint.
    /// </summary>
    /// <param name="serviceEndpoint">The endpoint to configure</param>
    /// <param name="clientCredentials">The client credentials</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public wsShopFinderSoapClient(EndpointConfiguration endpointConfiguration) : 
                base(wsShopFinderSoapClient.GetBindingForEndpoint(endpointConfiguration), wsShopFinderSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public wsShopFinderSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(wsShopFinderSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public wsShopFinderSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(wsShopFinderSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public wsShopFinderSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<GLSReference.PakkeshopData> GetOneParcelShopAsync(string ParcelShopNumber)
        {
            return base.Channel.GetOneParcelShopAsync(ParcelShopNumber);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<GLSReference.GetAllParcelShopsResponse> GLSReference.wsShopFinderSoap.GetAllParcelShopsAsync(GLSReference.GetAllParcelShopsRequest request)
        {
            return base.Channel.GetAllParcelShopsAsync(request);
        }
        
        public System.Threading.Tasks.Task<GLSReference.GetAllParcelShopsResponse> GetAllParcelShopsAsync(string countryIso3166A2)
        {
            GLSReference.GetAllParcelShopsRequest inValue = new GLSReference.GetAllParcelShopsRequest();
            inValue.countryIso3166A2 = countryIso3166A2;
            return ((GLSReference.wsShopFinderSoap)(this)).GetAllParcelShopsAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<GLSReference.GetParcelShopsInZipcodeResponse> GLSReference.wsShopFinderSoap.GetParcelShopsInZipcodeAsync(GLSReference.GetParcelShopsInZipcodeRequest request)
        {
            return base.Channel.GetParcelShopsInZipcodeAsync(request);
        }
        
        public System.Threading.Tasks.Task<GLSReference.GetParcelShopsInZipcodeResponse> GetParcelShopsInZipcodeAsync(string zipcode, string countryIso3166A2)
        {
            GLSReference.GetParcelShopsInZipcodeRequest inValue = new GLSReference.GetParcelShopsInZipcodeRequest();
            inValue.zipcode = zipcode;
            inValue.countryIso3166A2 = countryIso3166A2;
            return ((GLSReference.wsShopFinderSoap)(this)).GetParcelShopsInZipcodeAsync(inValue);
        }
        
        public System.Threading.Tasks.Task<GLSReference.ParcelShopSearchResult> SearchNearestParcelShopsAsync(string street, string zipcode, string countryIso3166A2, int Amount)
        {
            return base.Channel.SearchNearestParcelShopsAsync(street, zipcode, countryIso3166A2, Amount);
        }
        
        public System.Threading.Tasks.Task<GLSReference.ParcelShopSearchResult> GetParcelShopDropPointAsync(string street, string zipcode, string countryIso3166A2, int Amount)
        {
            return base.Channel.GetParcelShopDropPointAsync(street, zipcode, countryIso3166A2, Amount);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.wsShopFinderSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.wsShopFinderSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpsTransportBindingElement httpsBindingElement = new System.ServiceModel.Channels.HttpsTransportBindingElement();
                httpsBindingElement.AllowCookies = true;
                httpsBindingElement.MaxBufferSize = int.MaxValue;
                httpsBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpsBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.wsShopFinderSoap))
            {
                return new System.ServiceModel.EndpointAddress("https://www.gls.dk/webservices_v4/wsShopFinder.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.wsShopFinderSoap12))
            {
                return new System.ServiceModel.EndpointAddress("https://www.gls.dk/webservices_v4/wsShopFinder.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            wsShopFinderSoap,
            
            wsShopFinderSoap12,
        }
    }
}
