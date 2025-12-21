## WebResultEndpoint Implicit Operators and Quality of Life (QoL) Features

`WebResult` and `WebResult<TValue>` classes provide implicit operators and quality of life (QoL) features to simplify the creation and handling of results in your code. These features allow you to easily convert between different types and create `WebResult` without needing to explicitly call factory methods.

### Implicit Operators

- `Result<TValue>` to default implementation of `WebResult<TValue>` (equivalent to calling `WebResults.FromResult()`),
- `Result` to default implementation of `WebResult` (equivalent to calling `WebResults.FromResult()`),
- `TValue` to default implementation of `WebResult<TValue>` encapsulating a successful `Result<TValue>` (equivalent to calling `WebResults.FromResult()`),
- `WebResult<TValue>` to `WebResult`.

### QoL Features

- Static factory methods under `WebResults` class for creating various types of `WebResult` instances.
- `WebResult<TValue>` to `WebResult` using `ToWebResult()` method,
