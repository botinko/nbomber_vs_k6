use actix_web::{web, App, HttpResponse, HttpServer};
use actix_web_prom::PrometheusMetricsBuilder;
use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
struct Person {
    id: u32,
    name: String,
    email: String,
    age: u32,
    city: String,
    occupation: String,
}

#[derive(Debug, Serialize, Deserialize)]
struct ApiResponse {
    success: bool,
    data: Person,
    message: String,
}

async fn health_check() -> HttpResponse {
    HttpResponse::Ok().json(serde_json::json!({
        "status": "healthy",
        "service": "rust-benchmark-app"
    }))
}

async fn echo_json(person: web::Json<Person>) -> HttpResponse {
    let response = ApiResponse {
        success: true,
        data: person.into_inner(),
        message: "Data processed successfully".to_string(),
    };

    HttpResponse::Ok().json(response)
}

async fn get_person() -> HttpResponse {
    let person = Person {
        id: 1,
        name: "John Doe".to_string(),
        email: "john.doe@example.com".to_string(),
        age: 30,
        city: "New York".to_string(),
        occupation: "Software Engineer".to_string(),
    };

    HttpResponse::Ok().json(person)
}

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    env_logger::init_from_env(env_logger::Env::new().default_filter_or("info"));
    
    println!("🚀 Starting Rust benchmark server on 0.0.0.0:8080");
    
    // Create Prometheus metrics registry
    let prometheus = PrometheusMetricsBuilder::new("rust_app")
        .endpoint("/metrics")
        .build()
        .unwrap();
    
    HttpServer::new(move || {
        App::new()
            // Add Prometheus metrics middleware
            .wrap(prometheus.clone())
            .route("/health", web::get().to(health_check))
            .route("/api/person", web::get().to(get_person))
            .route("/api/person", web::post().to(echo_json))
    })
    .bind(("0.0.0.0", 8080))?
    .workers(4)
    .run()
    .await
}
