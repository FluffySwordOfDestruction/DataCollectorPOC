﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataRelay.WcfHostTester.DataRelayWcf {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DataRelayWcf.IDataRelayService")]
    public interface IDataRelayService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataRelayService/Send", ReplyAction="http://tempuri.org/IDataRelayService/SendResponse")]
        bool Send(byte[] package);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataRelayService/KnockKnock", ReplyAction="http://tempuri.org/IDataRelayService/KnockKnockResponse")]
        bool KnockKnock(byte[] package);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataRelayServiceChannel : DataRelay.WcfHostTester.DataRelayWcf.IDataRelayService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataRelayServiceClient : System.ServiceModel.ClientBase<DataRelay.WcfHostTester.DataRelayWcf.IDataRelayService>, DataRelay.WcfHostTester.DataRelayWcf.IDataRelayService {
        
        public DataRelayServiceClient() {
        }
        
        public DataRelayServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DataRelayServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataRelayServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataRelayServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool Send(byte[] package) {
            return base.Channel.Send(package);
        }
        
        public bool KnockKnock(byte[] package) {
            return base.Channel.KnockKnock(package);
        }
    }
}