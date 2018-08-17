﻿namespace ESFA.DAS.EmployerProvideFeedback.Infrastructure
{
    public interface ISessionService
    {
        void Set(string key, object value);
        void Set(string key, string stringValue);
        void Remove(string key);
        string Get(string key);
        T Get<T>(string key);
    }
}