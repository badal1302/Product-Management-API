# Product Management API with Basic Authentication

This API now includes simple Basic Authentication to secure your endpoints.

## How Basic Authentication Works

Basic Authentication sends username and password in the HTTP header. The credentials are encoded in base64 format.

**Format**: `Authorization: Basic {base64(username:password)}`

## Test Users

- **Username**: `admin`, **Password**: `admin123`
- **Username**: `user`, **Password**: `user123`

## API Endpoints

### Public (No Authentication Required)
- `GET /api/test/public` - Public endpoint

### Protected (Basic Authentication Required)
- `GET /api/test/protected` - Test authentication
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

## How to Test

### 1. Test Public Endpoint (No Auth)
```bash
curl -X GET "https://localhost:5001/api/test/public"
```

### 2. Test Protected Endpoint (With Auth)
```bash
# Encode "admin:admin123" in base64 = "YWRtaW46YWRtaW4xMjM="
curl -X GET "https://localhost:5001/api/test/protected" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM="
```

### 3. Test Products API
```bash
# Get all products
curl -X GET "https://localhost:5001/api/products" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM="

# Create a product
curl -X POST "https://localhost:5001/api/products" \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM=" \
  -H "Content-Type: application/json" \
  -d '{"name": "Test Product", "description": "Test", "price": 99.99, "inStock": true}'
```

## Using Swagger UI

1. Run the application: `dotnet run`
2. Navigate to `/swagger` in your browser
3. Click "Authorize" button
4. Enter: `Basic YWRtaW46YWRtaW4xMjM=`
5. Test the protected endpoints

## What Happens Without Authentication

If you try to access a protected endpoint without the `Authorization` header or with invalid credentials, you'll get:
- **Status Code**: 401 Unauthorized
- **Response**: Empty body

## Security Notes

⚠️ **Important**: Basic Authentication sends credentials in base64 encoding, which is not secure for production use over HTTP. 

**For Production:**
- Always use HTTPS
- Consider implementing JWT tokens for better security
- Store user credentials in a database with proper hashing
- Implement proper password policies

## How It Works

1. **Request comes in** with `Authorization: Basic {encoded_credentials}`
2. **BasicAuthAttribute** intercepts the request
3. **Decodes** the base64 credentials to get username and password
4. **Validates** against the hardcoded user list
5. **If valid**: Request continues, user info stored in `HttpContext.Items["User"]`
6. **If invalid**: Returns 401 Unauthorized

This is a simple, educational implementation. In real-world applications, you'd want more sophisticated authentication mechanisms.
