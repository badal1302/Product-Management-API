# Product Management API with Role-Based Authentication

This API includes comprehensive Role-Based Authentication with JWT tokens and Basic Authentication support to secure your endpoints with different access levels.

## Authentication Methods

### 1. JWT Token Authentication (Primary)
- **Login**: `POST /api/auth/login` with username/password
- **Response**: JWT token with user role information
- **Usage**: `Authorization: Bearer {jwt_token}`

### 2. Basic Authentication (Alternative)
- **Format**: `Authorization: Basic {base64(username:password)}`
- **Usage**: Direct username/password in header

## Test Users & Roles

- **Admin User**: `username: "admin"`, `password: "admin123"`, `role: "Admin"`
- **Regular User**: `username: "user"`, `password: "user123"`, `role: "User"`

## API Endpoints

### Public (No Authentication Required)
- `GET /api/test/public` - Public endpoint
- `POST /api/auth/login` - User login

### User & Admin Access (Requires Authentication)
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/test/protected` - Test authentication
- `GET /api/auth/profile` - Get user profile

### Admin Only Access (Requires Admin Role)
- `POST /api/products` - Create product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product
- `GET /api/auth/users` - Get all users
- `POST /api/auth/users` - Create new user
- `PUT /api/auth/users/{username}` - Update user
- `DELETE /api/auth/users/{username}` - Delete user
- `GET /api/test/users` - Admin test endpoint

## How to Test

### 1. Login and Get JWT Token
```bash
# Login as Admin
curl -X POST "http://localhost:5210/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "admin123"}'

# Login as User
curl -X POST "http://localhost:5210/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "user", "password": "user123"}'
```

### 2. Test Public Endpoint (No Auth)
```bash
curl -X GET "http://localhost:5210/api/test/public"
```

### 3. Test User/Admin Endpoints (With JWT Token)
```bash
# Get all products (User or Admin)
curl -X GET "http://localhost:5210/api/products" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"

# Get user profile (User or Admin)
curl -X GET "http://localhost:5210/api/auth/profile" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### 4. Test Admin Only Endpoints (With Admin JWT Token)
```bash
# Create a product (Admin only)
curl -X POST "http://localhost:5210/api/products" \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"name": "Test Product", "description": "Test", "price": 99.99, "inStock": true}'

# Delete a product (Admin only)
curl -X DELETE "http://localhost:5210/api/products/1" \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN_HERE"

# Get all users (Admin only)
curl -X GET "http://localhost:5210/api/auth/users" \
  -H "Authorization: Bearer ADMIN_JWT_TOKEN_HERE"
```

### 5. Test Role-Based Access Control
```bash
# Try to create product as regular user (should fail with 403)
curl -X POST "http://localhost:5210/api/products" \
  -H "Authorization: Bearer USER_JWT_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"name": "Test Product", "description": "Test", "price": 99.99, "inStock": true}'
```

## Using Swagger UI

1. Run the application: `dotnet run`
2. Navigate to `http://localhost:5210/swagger` in your browser
3. Click "Authorize" button
4. Choose authentication method:
   - **JWT**: Login first to get token, then use `Bearer {token}`
   - **Basic**: Enter `Basic YWRtaW46YWRtaW4xMjM=` (admin:admin123)
5. Test the protected endpoints

## Role-Based Access Control

### Access Levels
- **Public**: No authentication required
- **User/Admin**: Requires valid JWT token or Basic Auth
- **Admin Only**: Requires Admin role in JWT token

### HTTP Status Codes
- **200 OK**: Request successful
- **401 Unauthorized**: No authentication or invalid credentials
- **403 Forbidden**: Valid authentication but insufficient role permissions

## Security Features

### JWT Token Security
- **Signed tokens** with secret key
- **Role-based claims** embedded in token
- **Configurable expiration** (default: 60 minutes)
- **Secure token validation**

### Dual Authentication Support
- **JWT Bearer tokens** (primary method)
- **Basic Authentication** (alternative method)
- **Automatic fallback** between methods

## Configuration

### JWT Settings (appsettings.json)
```json
{
  "Jwt": {
    "Key": "YourSecretKeyHere",
    "Issuer": "ProductManagementApi",
    "Audience": "ProductManagementApiUsers",
    "ExpiresInMinutes": 60
  }
}
```

## How It Works

### JWT Authentication Flow
1. **Login**: User sends credentials to `/api/auth/login`
2. **Validation**: Server validates username/password
3. **Token Generation**: Server creates JWT with user role claims
4. **Token Response**: Client receives JWT token
5. **API Requests**: Client sends `Authorization: Bearer {token}`
6. **Role Validation**: Server validates token and checks role permissions

### Role-Based Authorization
1. **Request arrives** with JWT token
2. **Token validation** extracts user role from claims
3. **Role checking** against endpoint requirements
4. **Access granted/denied** based on role permissions

## Production Considerations

⚠️ **Important Security Notes:**
- Always use HTTPS in production
- Store JWT secret key securely
- Implement proper password hashing
- Add rate limiting for login endpoints
- Consider token refresh mechanisms
- Implement proper logging and monitoring
