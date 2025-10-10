import type { Role } from "./AuthContext";

export function normalizeRole(input: unknown, fallback: Role = "User"): Role {
	if (typeof input === "number") return input === 1 ? "Admin" : "User";
	if (typeof input === "string") {
		const s = input.trim().toLowerCase();
		return s === "admin" ? "Admin" : "User";
	}
	return fallback;
}
