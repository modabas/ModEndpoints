import http from "k6/http";
import { check, sleep } from "k6";

// Test configuration
export const options = {
  thresholds: {
    // Assert that 99% of requests finish within 10ms.
    http_req_duration: ["p(99) < 10"],
  },
  // Ramp the number of virtual users up and down
  stages: [
    { duration: "30s", target: 100 },
    { duration: "1m", target: 100 },
    { duration: "20s", target: 0 },
  ],
};

// Simulated user behavior
export default function () {
  let res = http.get("http://localhost:5102/RegularEndpoints/BasicTest/");
  // Validate response status
  check(res, { "status was 200": (r) => r.status == 200 });
}