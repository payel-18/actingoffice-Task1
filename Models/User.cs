﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RoleBaseApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public required string Role { get; set; }
    }
}
