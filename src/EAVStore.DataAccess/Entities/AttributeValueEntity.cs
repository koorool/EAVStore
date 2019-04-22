using System;
using EAVStore.DataAccess.Enums;

namespace EAVStore.DataAccess.Entities
{
    public class AttributeValueEntity
    {
        public Guid Id { get; set; }
        public EavEntity Entity { get; set; }
        public AttributeType AttributeType { get; set; }
        public string Value { get; set; }
    }
}