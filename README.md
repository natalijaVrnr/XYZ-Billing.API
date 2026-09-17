# Create Payment Endpoint

## Summary

`POST /api/payments` creates a payment for a given order through a configurable
payment gateway (currently **Stripe** or **Montonio**). The endpoint is
idempotency-aware: if a payment for the same order number is already in
progress or has already succeeded, it will not trigger a second charge at the
gateway. It returns:

- **200 OK** — payment succeeded, with the gateway's payment ID and timestamp.
- **409 Conflict** — a payment for this order is already in progress.
- **502 Bad Gateway** — the gateway rejected or failed to process the payment.
- **400 Bad Request** — the requested `GatewayId` is not a supported gateway.
- **500 Internal Server Error** — an unexpected payment status was returned
  (defensive fallback; should not occur in practice).

Note the request is **not** wired to a cancellation token. Once a payment has
been dispatched to a gateway, we let it run to completion even if the calling
client disconnects, rather than leaving the payment state ambiguous.

## Assumptions

1. The system currently supports the **Stripe** and **Montonio** gateways,
   with further gateways expected to be added over time.
2. A payment can be in one of four states: `NotStarted`, `InProgress`,
   `Succeeded`, `Failed`.
3. Every gateway generates its own unique payment ID per payment record, and
   that ID is gateway-specific (not the same as our order number).
4. Not all gateways provide their own idempotency guarantees, so idempotency
   cannot be delegated to the gateway layer alone.
5. The API is expected to scale out horizontally, so idempotency/state
   tracking must work across multiple instances (not just in-process).
6. Target p99 latency for this endpoint is **5 seconds**.

## Decisions

- **Redis for in-flight de-duplication.** When a payment is started, the
  order number is written to Redis with a **60 second TTL**. This prevents
  duplicate/concurrent requests for the same order from reaching the gateway
  twice while a payment is in flight. The key is deleted as soon as the
  payment succeeds. If a payment fails, the key is left to expire naturally
  via TTL, allowing a retry.
- **Database for completed payments.** Since gateway-issued payment IDs are
  not the same as our order numbers, a successful payment record (order
  number ↔ gateway payment ID ↔ gateway type ↔ timestamp) is persisted to a
  database. Without this, we would have no durable way to map an order back
  to the payment that fulfilled it.
- **Lookup order: cache → database → gateway.** On each request, we first
  check Redis (is a payment currently in progress for this order?), then the
  database (has this order already succeeded?), and only if neither has a
  record do we call out to the gateway. This avoids duplicate charges and
  avoids unnecessary gateway calls for orders that are already settled.

## Example

### Request

```http
POST /api/payments
Content-Type: application/json

{
  "orderNumber": "ORD-10293",
  "userId": "usr_84621",
  "amount": 49.99,
  "currency": "EUR",
  "gatewayId": "Stripe",
  "description": "Order #10293 - Premium subscription"
}
```

### Responses

**Success — 200 OK**

```json
{
  "orderNumber": "ORD-10293",
  "amount": 49.99,
  "timestamp": "2026-09-17T14:32:05.112Z",
  "paymentId": "pi_3PabcXYZ1234567890"
}
```

**Duplicate in-flight request — 409 Conflict**

```json
{
  "message": "Payment already in progress for this order"
}
```

**Gateway failure — 502 Bad Gateway**

```json
{
  "title": "The gateway failed to process payment",
  "status": 502
}
```

**Invalid gateway — 400 Bad Request**

```json
{
  "Message": "Invalid payment gateway type: 3"
}
```

## Running locally (Aspire)

This service is orchestrated via .NET Aspire, which spins up the API
alongside its Redis cache and database dependencies.

```bash
dotnet run --project ./src/AppHost
```

Once the Aspire dashboard is up, the API endpoint will be available (by
default) at:

```
POST https://localhost:<port>/api/payments
```

Check the Aspire dashboard console output / launch URL for the exact
port assigned to the API and Redis/DB resources in your environment.
