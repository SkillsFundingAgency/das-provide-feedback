﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ESFA.DAS.FeedbackDataAccess.Models
{
    public class EmployerFeedback
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "ukprn")]
        public long Ukprn { get; set; }

        [JsonProperty(PropertyName = "accountId")]
        public long AccountId { get; set; }

        [JsonProperty(PropertyName = "userRef")]
        public Guid UserRef { get; set; }

        [JsonProperty(PropertyName = "dateTimeCompleted")]
        public DateTime DateTimeCompleted { get; set; }

        [JsonProperty(PropertyName = "providerAttributes")]
        public List<ProviderAttribute> ProviderAttributes { get; set; }

        [JsonProperty(PropertyName = "providerRating")]
        public string ProviderRating { get; set; }
    }
}
