using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EAVStore.Api.ViewModels
{
    [DataContract]
    public class CreatePatientVm
    {
        [DataMember]
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 3)]
        public string Name { get; set; }

        [DataMember]
        [Range(1, 120)]
        public ushort Age { get; set; }
    }
}