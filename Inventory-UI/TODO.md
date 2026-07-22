# Inventory-UI TODO

Mark done with `[x]` or `[done]`.

---

## Authentication & Routing

- [ ] `Login.tsx` is an empty shell — renders a blank div. No form, no fields, no submit handler.
- [ ] No login or registration route defined in `App.tsx`. The app has no entry point for unauthenticated users.
- [ ] No protected route wrapper. Any unauthenticated user can navigate directly to any route.
- [ ] `useLogout` hook exists but is never called. The logout nav link routes to a missing page (`/Logout`) instead of triggering the logout service.
- [ ] `ProfileNavBar` links to `/profile`, `/settings`, `/Logout` — none of these routes exist in `App.tsx`.

---

## Forms

- [ ] `react-hook-form` and `zod` are installed but used **nowhere** in the codebase. No `useForm`, no `zodResolver`, no `z.object` schema exists anywhere.
- [ ] No form component exists for: login, registration, add product, edit product, update inventory quantity, change password, update user profile.

---

## Inventory Page

- [ ] Real inventory data never loads. `useAllInventory()` is never called — the table renders two hardcoded test rows.
- [ ] Permission level is hardcoded (`Role: "write_delete"`) — the real `GetUser` API call is commented out.
- [ ] DataTable action callbacks (`onEdit`, `onDelete`, `onView`, `onCreate`) are not passed — they navigate to routes that don't exist.
- [ ] Tile/summary components have placeholder text and empty props. No real metrics are shown.
- [ ] Pagination is fully hardcoded — no real page count comes from the API.

---

## Products

- [ ] No product page of any kind exists as a component — no list, add, or edit view.
- [ ] The product service and all product hooks (`useRemoveProduct`, `useNewProduct`, `useUpdateProduct`) are implemented but have nothing calling them.
- [ ] `productService.RemoveProduct` uses `GET` for a destructive operation — should be `DELETE` or `POST`.

---

## User Management

- [ ] No user list, user detail, or role assignment page exists.
- [ ] Most user hooks are built but have no component calling them: `useAssignUserRole`, `useDisableUser`, `useEnableUser`, `useRegisterUserWithRole`, forgot-password and reset-password variants.
- [ ] `useUpdateUser` hook is missing — `userService.UpdateUser` has no hook wrapper.
- [ ] `useGetUsers` hook is missing.

---

## Real-time / SignalR

- [ ] Both SignalR components are never imported or mounted anywhere in the app.
- [ ] Neither component has cleanup (`connection.stop()`) on unmount — connections leak on every remount.
- [ ] Hub URL in the component (`/hubs/general/${hubName}`) does not match the backend path (`/hub`).

---

## Models / Types & Config

- [ ] `useRole.tsx` is a copy of `useProduct.tsx` — it wraps the product service instead of the role service. All role hooks are broken.
- [ ] `roleService.tsx` exports a constant named `inventoryService` (copy-paste error).
- [ ] `InventoryItemKeys` lists `"quantity"` but `InventoryItem` interface has `initialQuantity`. Key mismatch.
- [ ] `axiosInstance` reads `process.env.BASE_URL` — no webpack env config or `.env` file defines this. All API calls resolve against an undefined base URL.
