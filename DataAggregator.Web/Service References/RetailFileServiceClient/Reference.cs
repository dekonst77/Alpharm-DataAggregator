﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAggregator.Web.RetailFileServiceClient {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RetailFileServiceClient.IService")]
    public interface IService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/UpdateFolderStrucuture", ReplyAction="http://tempuri.org/IService/UpdateFolderStrucutureResponse")]
        bool UpdateFolderStrucuture();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/UpdateFolderStrucuture", ReplyAction="http://tempuri.org/IService/UpdateFolderStrucutureResponse")]
        System.Threading.Tasks.Task<bool> UpdateFolderStrucutureAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/SendFileToWork", ReplyAction="http://tempuri.org/IService/SendFileToWorkResponse")]
        bool SendFileToWork(long[] fileIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/SendFileToWork", ReplyAction="http://tempuri.org/IService/SendFileToWorkResponse")]
        System.Threading.Tasks.Task<bool> SendFileToWorkAsync(long[] fileIds);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : DataAggregator.Web.RetailFileServiceClient.IService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceClient : System.ServiceModel.ClientBase<DataAggregator.Web.RetailFileServiceClient.IService>, DataAggregator.Web.RetailFileServiceClient.IService {
        
        public ServiceClient() {
        }
        
        public ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool UpdateFolderStrucuture() {
            return base.Channel.UpdateFolderStrucuture();
        }
        
        public System.Threading.Tasks.Task<bool> UpdateFolderStrucutureAsync() {
            return base.Channel.UpdateFolderStrucutureAsync();
        }
        
        public bool SendFileToWork(long[] fileIds) {
            return base.Channel.SendFileToWork(fileIds);
        }
        
        public System.Threading.Tasks.Task<bool> SendFileToWorkAsync(long[] fileIds) {
            return base.Channel.SendFileToWorkAsync(fileIds);
        }
    }
}
