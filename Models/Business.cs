using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RoleBaseApi.Models
{
    public class Business
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;             // Individual, Partnership, Limited Partnership
        public string Name { get; set; } = string.Empty;             // Business name
        //public string Building { get; set; } = string.Empty;
        //public string Street { get; set; } = string.Empty;
        //public string City { get; set; } = string.Empty;
        //public string State { get; set; } = string.Empty;
        //public string Country { get; set; } = string.Empty;
        //public string PostalCode { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;        // User Email who add the buisness
    }
}
