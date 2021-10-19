using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;
using Microsoft.Extensions.Logging;

namespace Demo
{
    public class ConsoleQueryLogger
        : ExecutionDiagnosticEventListener
    {
        private static Stopwatch _queryTimer;
        private readonly ILogger<ConsoleQueryLogger> _logger;

        public ConsoleQueryLogger(ILogger<ConsoleQueryLogger> logger)
        {
            _logger = logger;
        }

        public override IDisposable ExecuteRequest(IRequestContext context)
        {
            return new RequestScope(_logger, context);
        }

        public override void RequestError(IRequestContext context, Exception exception)
        {
            this._logger.LogError(exception, "Unhandled request error in GraphQL");
        }

        public override void TaskError(IExecutionTask task, IError error)
        {
            this._logger.LogError(error.Exception, "Unhandled task error in GraphQL: {Message}", error.Message);
        }

        public override void ResolverError(IMiddlewareContext context, IError error)
        {
            this._logger.LogError(error.Exception, "Unhandled resolver error in GraphQL: {Message}", error.Message);
        }

        private class RequestScope : IDisposable
        {
            private readonly IRequestContext _context;
            private readonly ILogger<ConsoleQueryLogger> _logger;

            public RequestScope(ILogger<ConsoleQueryLogger> logger, IRequestContext context)
            {
                _logger = logger;
                _context = context;
                _queryTimer = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                if (_context.Document is not null)
                {
                    var stringBuilder = new StringBuilder(_context.Document.ToString(true)).AppendLine();
                    var variables = _context.Variables?.ToList();
                    if (variables?.Count > 0)
                    {
                        stringBuilder.Append("Variables ").AppendLine();
                        try
                        {
                            foreach (var variableValue in variables)
                            {
                                stringBuilder.AppendFormat(
                                        $"  {FixWidth(variableValue.Name, 20)} :  {FixWidth(variableValue.Value.ToString(), 20)}: {variableValue.Type}")
                                    .AppendLine();
                            }
                        }
                        catch
                        {
                            // all input type records will land here.
                            stringBuilder.Append("  Formatting Variables Error. Continuing...").AppendLine();
                        }
                    }

                    _queryTimer.Stop();
                    stringBuilder.AppendFormat(
                        $"Elapsed time for query is {_queryTimer.Elapsed.TotalMilliseconds:0.#} milliseconds.");
                    _logger.LogInformation(stringBuilder.ToString());
                }

                string FixWidth(string existingString, int length)
                {
                    if (string.IsNullOrEmpty(existingString))
                        return string.Empty.PadRight(length);
                    if (existingString.Length > length)
                        return existingString[..length];
                    return existingString + " ".PadRight(length - existingString.Length);
                }
            }
        }
    }
}
