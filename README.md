# Vosaio AI Travel API - Husam

This project is an AI-powered travel itinerary API built with .NET Core. It integrates the OpenAI API (GPT-based itinerary generation) into a B2B travel platform to generate detailed travel itineraries based on user preferences.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Setup & Installation](#setup--installation)
- [API Usage Guide](#api-usage-guide)
- [AI Integration Explanation](#ai-integration-explanation)
- [Project Structure & Architecture](#project-structure--architecture)
- [Testing](#testing)
- [Deployment (Bonus)](#deployment-bonus)
- [Follow-Up Questions](#follow-up-questions)

---

## Project Overview

The Vosaio AI Travel API generates travel itineraries based on user input. Users provide details such as destination, travel dates, budget, and interests, and the API returns a structured itinerary (including hotels, activities, restaurants, estimated costs, and timings) generated by an AI model.

Key features:
- REST API built with ASP.NET Core Web API.
- AI integration using the OpenAI API.
- Data persistence via Entity Framework Core (using an in-memory database for testing).
- Comprehensive error handling, validation, and logging.
- Unit testing using NUnit and Moq.
- Swagger for API documentation.

---

## Setup & Installation

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or latest)
- [Git](https://git-scm.com/)
- Optional: [Docker](https://www.docker.com/) for containerization

### Steps

1. **Clone the Repository**

   ```bash
   git clone https://github.com/hussam/Vosaio-AI-Travel-API-hussam.git
   cd Vosaio-AI-Travel-API-hussam
   ```

2. **Restore Packages**

   Run the following command in the project root:

   ```bash
   dotnet restore
   ```

3. **Build the Project**

   ```bash
   dotnet build
   ```

4. **Run the API**

   ```bash
   dotnet run --project Vosaio.AI.Travel.API
   ```

   The API will start (by default on https://localhost:5001). Swagger UI is available in development mode at [https://localhost:5001/swagger](https://localhost:5001/swagger).

---

## API Usage Guide

### Endpoint

**POST** `/api/itinerary/generate`

### Request Body

Send a JSON payload with the following structure:

```json
{
  "destination": "Tokyo",
  "travelDates": ["2025-06-01", "2025-06-10"],
  "budget": 2000,
  "interests": ["history", "food", "adventure"]
}
```

### Example cURL Request

```bash
curl --location --request POST 'https://localhost:5001/api/itinerary/generate' \
--header 'Content-Type: application/json' \
--data-raw '{
  "destination": "Tokyo",
  "travelDates": ["2025-06-01", "2025-06-10"],
  "budget": 2000,
  "interests": ["history", "food", "adventure"]
}'
```

### Response

On success, the API returns a JSON object that includes:

- Destination
- TravelDates (start and end dates)
- Itinerary details:
  - Hotels
  - Activities
  - Restaurants
  - TotalEstimatedCost

---

## AI Integration Explanation

The API uses the OpenAI API to generate itineraries. Key points:

- **AI Model:**  
  We leverage a GPT-based model (via OpenAI API) to generate itineraries. The prompt is built dynamically based on the user’s input.

- **Prompt Building:**  
  A dedicated `PromptBuilder` (implementing `IPromptBuilder`) constructs a prompt that instructs the AI to return a JSON output conforming to a predefined schema. This ensures consistency and eases deserialization.

- **Error Handling:**  
  The AI integration service (`AIIntegrationService`) handles errors from the AI API, validating responses, and ensuring the JSON output matches the expected schema.

- **Abstraction:**  
  To support unit testing and maintainability, we created abstractions (`IOpenAIClient` and `IChatEndpoint`) along with adapters (`OpenAIClientAdapter`, `ChatEndpointAdapter`), which allow us to decouple our code from the sealed OpenAI library classes.

---

## Project Structure & Architecture

```
Vosaio-AI-Travel-API-YourName/
│
├── Vosaio.AI.Travel.API/                # Main API project
│   ├── Controllers/                     # API controllers (e.g., ItineraryController)
│   ├── Models/                          # Domain and DTO models (TravelRequest, ItineraryResponse, etc.)
│   ├── OpenAI/                          # Abstractions and adapters (IOpenAIClient, IChatEndpoint, PromptBuilder)
│   ├── Services/                        # Business logic (AIIntegrationService, ItineraryService)
│   ├── Program.cs                       # Application entry point
│   └── appsettings.json                 # Configuration file
│   ├── openai/             
│   ├── repo/          
│   └── ...                              # (Other data models or migrations if needed)
│
├── Vosaio.AI.Travel.API.Tests/          # Unit tests (NUnit)
│   ├── Services/                        # Tests for services (e.g., AIIntegrationServiceTests)
│   └── ... 
│
├── README.md                            # This file
└── Dockerfile (optional)                # For containerization
```

**Key Points:**
- **Separation of Concerns:**  
  Controllers handle HTTP interactions, Services encapsulate business logic (including AI integration), and Repositories manage data persistence.
- **Dependency Injection:**  
  All components are registered via DI in Program.cs.
- **SOLID Principles:**  
  Each class/interface has a single responsibility, and abstractions are used to decouple components.

---

## Testing

- **Unit Tests:**  
  Tests are implemented using NUnit and Moq. They cover key scenarios for AI integration, business logic, and data access.
- **Running Tests:**  
  You can run tests using the following command:

  ```bash
  dotnet test
  ```

---

## Things to Improve

While the current implementation meets the challenge requirements, here are several areas for improvement:

1. **Persistent Storage:**  
   - **Current State:**  
     Uses an in-memory database via EF Core.  
   - **Improvement:**  
     Switch to a persistent database (e.g., SQLite or SQL Server) for production. This can be achieved by updating the EF Core configuration based on environment settings.

2. **Enhanced Error Handling & Resilience:**  
   - **Current State:**  
     Basic error handling is implemented in services.  
   - **Improvement:**  
     Implement global exception handling middleware, improve logging with structured and correlation IDs, and consider retry policies for external API calls.

3. **Caching & Performance Optimization:**  
   - **Current State:**  
     No caching is implemented.  
   - **Improvement:**  
     Implement caching strategies (e.g., in-memory caching or Redis) to avoid re-fetching itineraries or redundant AI API calls.

4. **Scalability:**  
   - **Current State:**  
     Basic API design that handles requests sequentially.  
   - **Improvement:**  
     Explore asynchronous processing and load balancing techniques. Consider moving AI requests to background tasks or using message queues for high-load scenarios.

5. **Security Enhancements:**  
   - **Current State:**  
     API key and configuration are loaded from appsettings.  
   - **Improvement:**  
     Secure sensitive configuration using user secrets, environment variables, or Azure Key Vault. Add authentication and authorization to protect endpoints.

6. **Extensibility of AI Integration:**  
   - **Current State:**  
     Uses a single AI integration method.  
   - **Improvement:**  
     Consider adding support for multiple AI providers (like Azure OpenAI or Cognitive Services) or enabling model selection via configuration.

7. **API Documentation & Client SDK:**  
   - **Current State:**  
     Swagger is enabled for API documentation.  
   - **Improvement:**  
     Expand API documentation and provide an SDK or Postman collection to facilitate integration by third-party developers.

8. **Testing & Code Coverage:**  
   - **Current State:**  
     Unit tests cover core functionality.  
   - **Improvement:**  
     Increase test coverage for edge cases, integration tests for the API endpoints, and consider performance/load testing to ensure scalability.

---

## Deployment (Bonus)

- **Dockerfile:**  
  (Optional) A Dockerfile is provided for containerization.  

---

## Follow-Up Questions

In our **FollowUp_Answers.md**, we answered the following questions:
- **Code & Architecture:**  
  How your solution adheres to SOLID principles; your API’s structure (Controllers, Services, Repositories).
- **AI Integration:**  
  Which AI model you used and why; how the API processes user input to generate an itinerary.
- **Error Handling & Validation:**  
  How your API handles invalid input and key edge cases.
- **Performance & Scalability:**  
  How your API manages concurrent requests and potential improvements for scalability.
- **Additional Innovations:**  
  Any extra features or optimizations implemented to enhance the itinerary generation experience.

---

## Conclusion

This project provides a robust, testable, and maintainable solution for generating AI-powered travel itineraries. The project structure, adherence to best practices, and clear separation of concerns ensure that the API is scalable, easy to maintain, and ready for production deployment.

Feel free to reach out with any questions or improvements.
#   - V o s a i o - A I - T r a v e l - A P I - h u s s a m 
 
 