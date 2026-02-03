# Design Philosophy

## Purpose

**ModEndpoints** exists to solve a focused problem:

> **How to keep ASP.NET Core Minimal APIs structured, discoverable, and scalable without replacing ASP.NET itself?**

It is not a controller framework.
It is not a DSL for HTTP APIs.
It is not a platform that competes with ASP.NET Core.

ModEndpoints is a **structuring layer** that helps Minimal APIs grow without losing their clarity.

---

## Layered Endpoint Types

ModEndpoints provides **multiple endpoint base types** with increasing levels of opinion.

At the foundation is:

* **`MinimalEndpoint`** — explicit, ASP.NET-native, minimally abstracted

On top of that foundation, the project offers **opt-in endpoint types** that add conventions for specific use cases, such as:

* **`WebResultEndpoint`** — Business result → HTTP mapping
* **`BusinessResultEndpoint`** — Business-oriented return types
* **`ServiceEndpoint`** — Simplified remote service integration

These higher-level endpoint types intentionally trade some explicitness for convenience.
They **do not redefine the core philosophy** — they build on it.

> The design philosophy of ModEndpoints is anchored in `MinimalEndpoint`
> and extended by other endpoint types through deliberate, opt-in abstraction.

---

## Core Principles

### 1. ASP.NET Core Is the Platform

ModEndpoints does not abstract away ASP.NET Core — it **embraces it**.

* Endpoints use native `RouteHandlerBuilder`
* Endpoints integrate with ASP.NET routing, filters, metadata, and OpenAPI
* Execution happens in the ASP.NET Core pipeline
* No custom runtime or parallel HTTP pipeline is introduced

If you understand ASP.NET Core Minimal APIs, you already understand ModEndpoints.

> Anything you learn while using ModEndpoints should transfer directly to plain ASP.NET Core.

---

### 2. Explicit Over Clever

ModEndpoints favors **explicit configuration** over hidden conventions.

```csharp
builder.MapPost("/users");
```

This is intentional.

The project avoids:

* DSL-style routing APIs
* Inferred HTTP verbs or routes
* Convention-driven magic that hides behavior

Explicit code:

* Is easier to debug
* Is easier to reason about
* Ages better over time

Some higher-level endpoint types may reduce explicitness for convenience,
but this is always an **opt-in trade-off**, never a requirement.

---

### 3. Minimal Abstraction, Not a New Pipeline

ModEndpoints does not introduce:

* A custom HTTP execution model
* A framework-specific request/response pipeline
* Controller-style indirection

Endpoints are:

* Discovered
* Configured using ASP.NET primitives
* Executed by ASP.NET Core itself

This preserves:

* Familiar performance characteristics
* Compatibility with middleware and tooling
* Clear escape hatches when lower-level control is needed

---

### 4. Endpoints Are Units of Composition

Each endpoint is:

* A single class
* With a single responsibility
* Fully dependency-injection friendly
* Easy to reason about in isolation

Endpoints are not controllers.
They are not services.

They are **thin HTTP adapters**.

This model aligns naturally with:

* Vertical slice architecture
* Feature-based folder structures
* Modular and package-level reuse

---

### 5. Opt-In Conventions, Not Mandatory Ones

ModEndpoints deliberately avoids enforcing:

* A single error format
* A single result shape
* A single status-code mapping strategy

Different teams have different needs.

Instead:

* Core abstractions stay small
* Conventions can be layered on top
* Teams choose how opinionated they want to be

> Consistency should be a **team decision**, not a framework mandate.
