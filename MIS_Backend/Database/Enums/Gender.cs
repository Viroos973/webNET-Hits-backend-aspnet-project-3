﻿using System.Text.Json.Serialization;

namespace MIS_Backend.Database.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male,
        Female
    }
}
