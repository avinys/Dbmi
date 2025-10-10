// src/auth/tokenUser.ts
export type Role = "User" | "Admin";
export type User = { id: number; username: string; email: string; role: Role };

function parseJwt(token: string): any {
	const [, payload] = token.split(".");
	// atob expects base64 (not url-safe); replace chars then decode
	const json = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
	return JSON.parse(decodeURIComponent(escape(json)));
}

export function userFromAccessToken(accessToken: string): User {
	const p = parseJwt(accessToken);

	const id =
		Number(
			p["nameid"] ??
				p[
					"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
				] ??
				p["sub"]
		) || 0;

	const role: Role = (p["role"] ??
		p["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
		"User") as Role;

	const username = (p["unique_name"] ??
		p["name"] ??
		p["username"] ??
		"user") as string;
	const email = (p["email"] ?? p["emailaddress"] ?? "") as string;

	return { id, username, email, role };
}
