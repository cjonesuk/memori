using Memori;
using Spectre.Console.Cli;
using static Memori.Ui;

ShowHeader();

var app = new CommandApp();

app.Configure(config =>
{
    config.AddCommand<IngestCommand>("ingest");

    config.PropagateExceptions();
    config.ValidateExamples();
});

app.SetDefaultCommand<IngestCommand>();

await app.RunAsync(args);