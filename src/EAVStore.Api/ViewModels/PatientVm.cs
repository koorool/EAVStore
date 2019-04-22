using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EAVStore.Api.ViewModels
{
    [DataContract]
    public class PatientVm
    {
        [DataMember]
        public Guid PatientId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        [Range(1, 120)]
        public ushort Age { get; set; }

        [DataMember]
        public IReadOnlyCollection<OperationVm> Operations { get; set; }
    }
}