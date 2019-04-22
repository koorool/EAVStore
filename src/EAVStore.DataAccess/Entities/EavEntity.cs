using System;
using System.Collections.Generic;
using EAVStore.DataAccess.Enums;

namespace EAVStore.DataAccess.Entities
{
    public class EavEntity
    {
        public Guid Id { get; set; }
        public EntityType EntityType { get; set; }
        public List<AttributeValueEntity> AttributeValues { get; set; }
    }
}