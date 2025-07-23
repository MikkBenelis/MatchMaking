global using System;
global using System.Collections.Concurrent;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

global using Confluent.Kafka;
global using JetBrains.Annotations;
global using Newtonsoft.Json;
global using StackExchange.Redis;

global using MatchMakingService.Data;
global using MatchMakingService.Data.DataTransferObjects;
global using MatchMakingService.Data.KafkaTopicModels;
global using MatchMakingService.Services.InfrastructureServices;