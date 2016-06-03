﻿#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Autofac;
using Pipeline.Contracts;
using System;
using System.Threading;

namespace Pipeline.Command {
    class Program {
        static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        static void Main(string[] args) {

            Console.CancelKeyPress += (sender, eArgs) => {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            var options = new Options();

            if (CommandLine.Parser.Default.ParseArguments(args, options)) {
                Environment.ExitCode = 0;
                var builder = new ContainerBuilder();
                builder.RegisterModule(new ScheduleModule(options));

                using (var scope = builder.Build().BeginLifetimeScope()) {
                    var scheduler = scope.Resolve<IScheduler>();
                    scheduler.Start();
                    if (scheduler is QuartzNowScheduler) {
                        scheduler.Stop();
                    } else {
                        QuitEvent.WaitOne();
                        Console.WriteLine("Stopping...");
                        scheduler.Stop();
                    }
                }
            } else {
                Environment.ExitCode = 1;
            }

            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();
        }
    }
}