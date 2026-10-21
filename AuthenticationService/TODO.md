# AuthenticationService TODO

This service's purpose is to seed the initial admin user into the database at container startup via gRPC. Nothing is implemented yet.

Mark done with `[x]` or `[done]`.

---

- [ ] Proto contract in `Grpc.Shared` needs to be defined before anything here can be implemented. Start there. See `Grpc.Shared/TODO.md`.
- [ ] `Services/AuthenticationService.cs` overrides no RPC methods. Any gRPC call throws unimplemented. Admin seeding logic needs to be built here once the proto is in place.
- [ ] `AuthService.csproj` has `DockerfileContext` pointing to `..\InventoryDb` — incorrect path for this service.
