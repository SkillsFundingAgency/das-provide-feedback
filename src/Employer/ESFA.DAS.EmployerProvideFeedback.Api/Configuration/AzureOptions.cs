﻿namespace ESFA.DAS.EmployerProvideFeedback.Configuration
{
    public class AzureOptions
    {
        public string CosmosEndpoint { get; set; }

        public string CosmosKey { get; set; }

        public string DatabaseName { get; set; }

        public string EmployerFeedbackCollection { get; set; }

        public string TokenCollection { get; set; }
    }
}