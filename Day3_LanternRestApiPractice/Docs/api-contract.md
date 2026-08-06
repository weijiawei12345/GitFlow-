# Lantern REST API Contract

## Conventions

- Base path: `/api/v1`.
- Responses use JSON and `Content-Type: application/json; charset=utf-8`.
- Successful responses use HTTP `200` and a JSON `data` field.
- Error responses use the corresponding HTTP status and an `error` object.
- Collection query parameters are optional. `page` starts at `1`; `pageSize` is `1` through `100`.

## List temples

`GET /api/v1/temples?keyword=&page=&pageSize=`

Example success response:

```json
{
  "data": [
    {
      "id": 1,
      "name": "Longshan Temple",
      "introduction": "A temple for the REST API practice project.",
      "imageUrl": "",
      "longitude": 121.499,
      "latitude": 25.036,
      "isDirectionEnabled": true
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 1,
    "totalPages": 1
  }
}
```

## Get a temple

`GET /api/v1/temples/{id}`

Returns `200` with `{ "data": { ... } }`, or `404` with the error envelope.

## Error envelope

```json
{
  "error": {
    "code": "temple_not_found",
    "message": "Temple 42 was not found."
  }
}
```

The mock server can return `401`, `429`, or `500` when its `MOCK_FAILURE_STATUS`
environment variable is set. This keeps error testing separate from the public API contract.
