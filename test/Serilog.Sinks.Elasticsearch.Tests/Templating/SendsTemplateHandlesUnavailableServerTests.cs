﻿using System;
using System.IO;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Serilog.Debugging;

namespace Serilog.Sinks.Elasticsearch.Tests.Templating
{
    [TestFixture(Category = "Integration test")]
    public class SendsTemplateHandlesUnavailableServerTests : ElasticsearchSinkTestsBase
    {
        [Test]
        public void Should_not_crash_when_server_is_unavaiable()
        {
            // If this crashes, the test will fail
            CreateLoggerThatCrashes();
        }

        [Test]
        public void Should_write_error_to_self_log()
        {
            var selfLogMessages = new StringBuilder();
            SelfLog.Out = new StringWriter(selfLogMessages);

            // Exception occurs on creation - should be logged
            CreateLoggerThatCrashes();

            var selfLogContents = selfLogMessages.ToString();
            selfLogContents.Should().Contain("Failed to create the template");
            selfLogContents.Should().Contain("WebException");
        }

        private static ILogger CreateLoggerThatCrashes()
        {
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:31234"))
                {
                    AutoRegisterTemplate = true,
                    TemplateName = "crash"
                });

            return loggerConfig.CreateLogger();
        }
    }
}