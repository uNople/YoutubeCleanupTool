﻿using Autofac;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using YoutubeCleanupTool;
using YoutubeCleanupTool.DataAccess;

namespace YoutubeCleanupConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<YouTubeCleanupToolModule>();
            builder.RegisterModule<YoutubeCleanupConsoleModule>();

            var dbContextBuilder = new DbContextOptionsBuilder<YoutubeCleanupToolDbContext>();
            dbContextBuilder.UseSqlite("Data Source=Application.db");
            builder.RegisterInstance(dbContextBuilder.Options);
            var youtubeCleanupToolDbContext = new YoutubeCleanupToolDbContext(dbContextBuilder.Options);
            youtubeCleanupToolDbContext.Migrate();
            builder.RegisterInstance<IYoutubeCleanupToolDbContext>(youtubeCleanupToolDbContext);
            var container = builder.Build();
            try
            {
                await container.Resolve<IConsoleUi>().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex}");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
