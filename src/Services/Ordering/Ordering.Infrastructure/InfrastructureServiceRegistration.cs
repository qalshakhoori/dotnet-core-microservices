﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Infrastructure.Mail;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;

namespace Ordering.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config) 
        {
            services.AddDbContext<OrderContext>(options =>
                options.UseSqlServer(config.GetConnectionString("OrderingConnectionString")));

            services.AddScoped(typeof(IAsyncRepo<>), typeof(RepoBase<>));
            services.AddScoped<IOrderRepo, OrderRepo>();

            services.Configure<EmailSettings>(c => config.GetSection("EmailSettings"));
            services.AddSingleton<IEmailService, EmailService>();

            return services;
        }
    }
}
