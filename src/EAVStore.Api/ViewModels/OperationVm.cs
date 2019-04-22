using System;
using System.Runtime.Serialization;

namespace EAVStore.Api.ViewModels
{
    [DataContract]
    public class OperationVm
    {
        [DataMember]
        public Guid OperationId { get; set; }

        [DataMember]
        public string OperationName { get; set; }
    }
}