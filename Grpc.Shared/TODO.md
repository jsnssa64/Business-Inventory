# Grpc.Shared TODO

Start here before implementing `AuthenticationService`.

Mark done with `[x]` or `[done]`.

---

- [ ] `Protos/authentication.proto` is the gRPC Hello World template (`SayHello`, `HelloRequest`, `HelloReply`). The real contract needs to be designed and written from scratch.
- [ ] `GrpcServices` in the `.csproj` is set to `"Server"` — generates server-side stubs only. For a shared contract library it should be `"Both"` so consumers can generate client stubs, or `"None"` if stubs are generated per-consumer.
- [ ] Distribution is via a manually copied `.nupkg` in `bin/Release/`. A project reference or a proper local NuGet feed would be less fragile.
