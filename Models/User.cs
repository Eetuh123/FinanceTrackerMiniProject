﻿using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using static FinanceTracker.Models.DatabaseManipulator;

namespace FinanceTracker.Models
{
    public class User : IMongoDocument
    {
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        [Required(ErrorMessage = "Username is required")]
        public string username { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }

    }
}
