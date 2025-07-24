global using System;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.RateLimiting;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using StackExchange.Redis;

global using MatchMakingService.Data;
global using MatchMakingService.Data.DataTransferObjects;
global using MatchMakingService.Data.KafkaTopicModels;
global using MatchMakingService.Services.InfrastructureServices;
global using MatchMakingService.Services.PayloadServices;