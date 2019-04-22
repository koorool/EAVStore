using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EAVStore.Api.ViewModels
{
    [DataContract]
    public class CreateOperationVm
    {
        [DataMember]
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 3)]
        public string OperationName { get; set; }

        [DataMember]
        [Required]
        public Guid? PatientId { get; set; }
    }
}