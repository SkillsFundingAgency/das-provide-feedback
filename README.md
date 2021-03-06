# Digital Apprenticeships Service
## das-provider-feedback-employer
|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png)|Tasks|
| Build | ![Build badge](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/1090/badge) |

## Provide Feedback (Beta)
This solution represents the Provider Feedback code base currently in alpha.

## Developer setup

### Requirements

* [.NET Core SDK >= 2.1.302](https://www.microsoft.com/net/download/)
* [Docker for X](https://docs.docker.com/install/#supported-platforms) (not required for emailer functions)
* [Azure Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator) (not required for emailer functions)
* Azure Service Bus

### Environment Setup

The default development environment uses docker containers to host the following dependencies.

* Redis
* Elasticsearch
* Logstash

On first setup run the following command from _**/setup/containers/**_ to create the docker container images:

`docker-compose build`

To start the containers run:

`docker-compose up -d`

You can view the state of the running containers using:

`docker ps -a`

Run Azure Cosmos DB Emulator

#### Setting up emailer functions to run locally
##### Add local.settings.json to ESFA.DAS.ProviderFeedback.Employer.Functions.Emailer
Please note all the connection string and secrets to API have been removed. This will need updating.
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "ASPNETCORE_ENVIRONMENT": "DEV",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "FUNCTIONS_EXTENSION_VERSION": "2.0.12673.0",
    "AppInsights_InstrumentationKey": "",
    "FeedbackSiteBaseUrl": "localhost:5030",
    "EmailBatchSize": "500",
    "NotificationApiBaseUrl": "https://at-notifications.apprenticeships.sfa.bis.gov.uk",
    "ClientToken": "",
    "InviteEmailerSchedule": "0 */10 10-11 * * MON-FRI",
    "ReminderEmailerSchedule": "0 3/10 10-11 * * MON-FRI",
    "DataRefreshSchedule": "0 0 3 * * MON-FRI",
    "AppName": "das-provide-feedback-emailer",
    "LogDir": "C:\\Logs\\ESFA\\Provide Feedback\\Employer",
    "ServiceBusConnection": "",
    "RetrieveFeedbackAccountsQueueName": "retrieve-employer-accounts",
    "ProcessActiveFeedbackQueueName": "process-active-feedback",
    "GenerateSurveyInviteMessageQueueName": "generate-survey-invite",
    "AccountRefreshQueueName": "refresh-account",
    "RetrieveProvidersQueueName": "retrieve-providers"
  },

  "EmailSettings": {
    "BatchSize": 5,
    "FeedbackSiteBaseUrl": "localhost:5030",
    "ReminderDays": 14,
    "InviteCycleDays": 30
  },
  "ConnectionStrings": {
    "EmployerEmailStoreConnection": "Data Source=(localdb)\\ProjectsV13;Initial Catalog=ESFA.DAS.EmployerFeedbackEmail.Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultipleActiveResultSets=true;MultiSubnetFailover=False",
    "Redis": "localhost:6379"
  },
  "NotificationApi": {
    "BaseUrl": "https://at-notifications.apprenticeships.sfa.bis.gov.uk",
    "ClientToken": "abc123"
  },
  "AccountApi": {
    "ApiBaseUrl": "https://test-accounts.apprenticeships.education.gov.uk",
    "ClientId": "",
    "ClientSecret": "",
    "IdentifierUri": "https://citizenazuresfabisgov.onmicrosoft.com/eas-api",
    "Tenant": "citizenazuresfabisgov.onmicrosoft.com"
  },
  "CommitmentApi": {
    "BaseUrl": "https://test-commitments.apprenticeships.education.gov.uk/",
    "ClientToken": ""
  },
  "ProviderApi": {
    "BaseUrl": "https://test-fatapi.apprenticeships.education.gov.uk/"
  }
}
```
##### Publish database (ESFA.DAS.EmployerFeedbackEmail.Database) to sql server.
- Set the connection string to the database in the local.settings.json file under `ConnectionStrings.EmployerEmailStoreConnection`
##### Setting up service bus
- Create a service bus account on azure subscription
- Set the connection string to service bus account in the local.settings.json file under `Values.ServiceBusConnection`
- Create following queues on the service bus account
  - retrieve-employer-accounts
  - process-active-feedback
  - generate-survey-invite
  - refresh-account
  - retrieve-providers

### Application logs
Application logs are logged to [Elasticsearch](https://www.elastic.co/products/elasticsearch) and can be viewed using [Kibana](https://www.elastic.co/products/kibana) at http://localhost:5601

## License

Licensed under the [MIT license](LICENSE)
