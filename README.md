# NBomber vs k6 Benchmark Comparison

A comprehensive benchmark comparison project between [NBomber](https://nbomber.com/) (C#) and [k6](https://k6.io/) (JavaScript) load testing tools. This project tests both tools against a Rust web application to evaluate performance, resource usage, and metrics collection capabilities.

## 🏗️ Architecture

The project consists of the following components:

- **Rust Application** (`rust-app`): A high-performance Actix-web REST API serving as the target application
- **NBomber** (`nbomber`): C# load testing tool with Prometheus metrics export
- **k6** (`k6`): JavaScript-based load testing tool with Prometheus remote write
- **Prometheus**: Metrics collection and storage
- **Grafana**: Visualization and dashboards
- **cAdvisor**: Container resource metrics
- **dotnet-monitor**: .NET runtime metrics for NBomber

## 📋 Prerequisites

- [Docker](https://www.docker.com/get-started) (version 20.10 or later)
- [Docker Compose](https://docs.docker.com/compose/install/) (version 2.0 or later)
- At least 8GB of available RAM
- 4+ CPU cores recommended

## 🚀 Quick Start

1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd nbomber_vs_k6
   ```

2. **Start all services**:
   ```bash
   docker-compose up -d
   ```

3. **Run a specific load test**:
   
   For **NBomber**:
   ```bash
   docker-compose up nbomber
   ```
   
   For **k6**:
   ```bash
   docker-compose up k6
   ```

4. **Access monitoring dashboards**:
   - **Grafana**: http://localhost:3000 (username: `admin`, password: `admin`)
   - **Prometheus**: http://localhost:9090
   - **cAdvisor**: http://localhost:8081

5. **View test reports**:
   - NBomber reports are saved in `nbomber/reports/` directory
   - k6 metrics are exported to Prometheus and visible in Grafana

## 📁 Project Structure

```
nbomber_vs_k6/
├── docker-compose.yml          # Docker Compose orchestration
├── rust-app/                   # Target Rust application
│   ├── src/main.rs            # Actix-web REST API
│   ├── Cargo.toml             # Rust dependencies
│   └── Dockerfile             # Rust app container
├── nbomber/                    # NBomber load testing tool
│   ├── Program.cs             # NBomber test scenarios
│   ├── NBomberTest.csproj     # .NET project file
│   ├── infra-config.json      # NBomber infrastructure config
│   ├── reports/               # Generated test reports
│   └── Dockerfile             # NBomber container
├── k6/                        # k6 load testing tool
│   └── load-test.js           # k6 test scenarios
├── prometheus/                # Prometheus configuration
│   └── prometheus.yml         # Scrape configs
└── grafana/                   # Grafana configuration
    ├── dashboards/            # Pre-configured dashboards
    └── provisioning/          # Auto-provisioning configs
```

## 🧪 Test Scenarios

Both load testing tools execute identical scenarios:

### Scenario 1: GET Person
- **Endpoint**: `GET /api/person`
- **Load Pattern**: Ramping arrival rate
- **Target Rate**: 5,000 requests/second
- **Duration**: 3 minutes
- **Max VUs**: 500

### Scenario 2: POST Person
- **Endpoint**: `POST /api/person`
- **Payload**: JSON person object
- **Load Pattern**: Ramping arrival rate
- **Target Rate**: 5,000 requests/second
- **Duration**: 3 minutes
- **Max VUs**: 500

## 📊 Monitoring & Metrics

### Available Metrics

- **Application Metrics**: Response times, request rates, error rates
- **Container Metrics**: CPU, memory, network I/O (via cAdvisor)
- **Runtime Metrics**: .NET GC, thread pool (via dotnet-monitor)
- **Prometheus Metrics**: Custom metrics from both load testers

### Grafana Dashboards

Pre-configured dashboards are available in Grafana:
- Benchmark comparison dashboard
- Container resource usage
- Application performance metrics

## ⚙️ Configuration

### Environment Variables

- `TARGET_URL`: Target application URL (default: `http://rust-app:8080`)
- `K6_PROMETHEUS_RW_SERVER_URL`: Prometheus remote write endpoint
- `DOTNET_EnableDiagnostics`: Enable .NET diagnostics

### Resource Limits

Services are configured with resource limits:
- **rust-app**: 2 CPUs, 2GB RAM
- **nbomber**: 2 CPUs, 4GB RAM
- **k6**: 2 CPUs, 4GB RAM

### Network Optimization

The setup includes TCP optimizations for high-throughput scenarios:
- TCP reuse and timestamps enabled
- Increased connection limits (250,000 file descriptors)
- Optimized TCP backlog and fast open settings

## 📈 Running Benchmarks

### Run Both Tests Sequentially

```bash
# Run NBomber test
docker-compose up nbomber

# Wait for completion, then run k6
docker-compose up k6
```

### Run Tests in Isolation

To ensure clean results, stop all services between tests:

```bash
# Stop all services
docker-compose down

# Run NBomber
docker-compose up nbomber

# Stop and clean up
docker-compose down -v

# Run k6
docker-compose up k6
```

### View Real-time Metrics

While tests are running, access:
- Grafana: http://localhost:3000
- Prometheus: http://localhost:9090
- cAdvisor: http://localhost:8081

## 📝 Reports

### NBomber Reports

NBomber generates comprehensive reports in multiple formats:
- HTML reports: `nbomber/reports/nbomber_report_*.html`
- Markdown reports: `nbomber/reports/nbomber_report_*.md`
- CSV reports: `nbomber/reports/nbomber_report_*.csv`
- Text reports: `nbomber/reports/nbomber_report_*.txt`

### k6 Metrics

k6 metrics are exported directly to Prometheus and can be queried or visualized in Grafana.

## 🔧 Troubleshooting

### Services Not Starting

1. Check Docker resources:
   ```bash
   docker system df
   docker stats
   ```

2. Verify ports are available:
   - 8080 (rust-app)
   - 3000 (Grafana)
   - 9090 (Prometheus)
   - 8081 (cAdvisor)

### High Resource Usage

If you experience resource constraints:
- Reduce `maxVUs` in `k6/load-test.js`
- Reduce `rate` in `nbomber/Program.cs`
- Adjust resource limits in `docker-compose.yml`

### Target Service Not Ready

Both load testers include health check logic. If tests fail to start:
- Check rust-app logs: `docker-compose logs rust-app`
- Verify health endpoint: `curl http://localhost:8080/health`

## 🛠️ Development

### Modifying Test Scenarios

**k6**: Edit `k6/load-test.js` to modify load patterns, scenarios, or endpoints.

**NBomber**: Edit `nbomber/Program.cs` to modify scenarios, load simulations, or HTTP clients.

### Rebuilding Containers

After making changes, rebuild the containers:

```bash
docker-compose build
docker-compose up
```

### Adding New Metrics

1. Add metric collection in your load tester
2. Update Prometheus scrape configs in `prometheus/prometheus.yml`
3. Create or update Grafana dashboards in `grafana/dashboards/`

## 📚 Additional Resources

- [NBomber Documentation](https://nbomber.com/docs)
- [k6 Documentation](https://k6.io/docs/)
- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [Actix-web Documentation](https://actix.rs/)

## 📄 License

This project is provided as-is for benchmarking and comparison purposes.

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to:
- Report bugs
- Suggest improvements
- Submit pull requests

---

**Note**: This benchmark is designed to compare NBomber and k6 under similar conditions. Results may vary based on hardware, network conditions, and system configuration.

