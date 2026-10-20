# Identity (`org`) decisions

How this context is designed and why. Not a change history; git covers that.

Shared conventions (DB-001 to DB-007, DB-013) and cross-boundary decisions: [`../DECISIONS.md`](../DECISIONS.md). The ones that matter most here are **DB-008** (other contexts refer to businesses and staff by their PublicId) and **DB-010** (this schema stays in Inventory).

---

## ID-001: Business names are unique forever
- **Decision:** `UQ_Business_Name` covers all rows, including deactivated businesses.
- **Why:** history and reports never show two businesses with the same name.
- **Rejected:** a filtered unique index (`WHERE DeactivatedAt IS NULL`), which would free names for reuse.
- **Consequences:** a deactivated business's name can't be taken by a new one. The default collation is case-insensitive, so "Acme" = "acme".

## ID-002: No Role table
- **Decision:** there's no `org.Role` table and no `StaffMember.RoleId`. Every staff member has the same access to their business.
- **Why:** Staff vs Admin inside a business is too thin to model yet. Supplier people use supplier-side services, never this context. The platform operator works across businesses, so it isn't a StaffMember.
- **Rejected:** a Role lookup table; `IsAdmin BIT`; "operator vs business" as a role on StaffMember (an operator has no single business).
- **Consequences:** permissions within a business come later. Adding a role means a new table and one column on StaffMember, with no data migration. Platform operators will need their own table or will live in auth.

## ID-003: Staff email is unique across the whole system, forever
- **Decision:** `UQ_StaffMember_Email` covers all rows, including deactivated staff.
- **Why:** one email = one business; no multi-business access to deal with.
- **Rejected:** a filtered unique index on active staff only, which would let someone leave one business and join another with the same email.
- **Consequences:** a person can never reuse their email at a second business. If that becomes a real need, split it into person + membership tables rather than loosening this constraint.
