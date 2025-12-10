using System.Text;
using System.Text.Json;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using NBomber.Sinks.Prometheus;

var targetUrl = Environment.GetEnvironmentVariable("TARGET_URL") ?? "http://rust-app:8080";

Console.WriteLine($"NBomber starting - Target: {targetUrl}");

var person = new
{
    id = 1,
    name = "Jane Smith",
    email = "jane.smith@test.com",
    age = 28,
    city = "San Francisco",
    occupation = "DevOps Engineer"
};
var stringContent = new StringContent(JsonSerializer.Serialize(person), Encoding.UTF8, "application/json");

HttpClient httpClient = Http.CreateDefaultClient();

// Scenario 1: GET requests
var getScenario = Scenario.Create("get_person", async context =>
{
    var response = await httpClient.GetAsync($"{targetUrl}/api/person");
    //await response.Content.ReadAsByteArrayAsync();

    return response.IsSuccessStatusCode
        ? Response.Ok()
        : Response.Fail();
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.RampingInject(rate: 5000, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(3))
);

// Scenario 2: POST requests with JSON
var postScenario = Scenario.Create("post_person", async context =>
    {
        var response = await httpClient.PostAsync($"{targetUrl}/api/person", stringContent);
        //await response.Content.ReadAsByteArrayAsync();

        return response.IsSuccessStatusCode
            ? Response.Ok()
            : Response.Fail();
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.RampingInject(rate: 5000, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(3))
);

// Configure Prometheus sink
var prometheusSink = new PrometheusSink();

NBomberRunner
    .RegisterScenarios(getScenario, postScenario)
    .WithReportFolder("reports")
    //.WithReportFormats(ReportFormat.Html, ReportFormat.Md)
    .WithReportingSinks(prometheusSink)
    .WithReportingInterval(TimeSpan.FromSeconds(5))
    .LoadInfraConfig("infra-config.json")
    .Run();

Console.WriteLine("NBomber test completed");
