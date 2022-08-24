using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Domains
{
    public abstract class MongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; protected init; }

        [BsonElement("createdDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [BsonElement("lastModifiedDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? LastModifiedDate { get; set; }
    }
}
