// src/auth/AuthContext.tsx
import React, {
	createContext,
	useEffect,
	useMemo,
	useState,
	useCallback,
	type ReactNode,
} from "react";
import {
	api,
	ensureFreshToken,
	setAuthTokens,
	clearAuthTokens,
	getStoredTokens,
} from "../lib/api";
import { userFromAccessToken } from "./tokenUser";
import { normalizeRole } from "./normalizeRole";

export type Role = "User" | "Admin";
export type User = { id: number; username: string; email: string; role: Role };
export type TokenResponseDto = {
	accessToken: string;
	refreshToken: string;
	expiresAt: string; // ISO
};

type AuthState = {
	user: User | null;
	tokens: TokenResponseDto | null;
	login: (u: User, t: TokenResponseDto) => void;
	logout: () => void;
};

export const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
	// bootstrap from storage so it survives reloads
	const [tokens, setTokensState] = useState<TokenResponseDto | null>(
		getStoredTokens()
	);
	const [user, setUser] = useState<User | null>(null);

	// keep axios Authorization header in sync
	useEffect(() => {
		if (tokens) setAuthTokens(tokens);
	}, [tokens]);

	const loginFn = useCallback((u: User, t: TokenResponseDto) => {
		setTokensState(t);
		setUser(u);
		localStorage.setItem("bdmi.tokens", JSON.stringify(t));
		localStorage.setItem("bdmi.user", JSON.stringify(u));
	}, []);

	const logoutFn = useCallback(() => {
		clearAuthTokens();
		setTokensState(null);
		setUser(null);
		localStorage.removeItem("bdmi.tokens");
		localStorage.removeItem("bdmi.user");
	}, []);

	// Rehydrate session on mount
	useEffect(() => {
		(async () => {
			const stored = getStoredTokens();
			if (stored) setTokensState(stored);

			try {
				await ensureFreshToken();
			} catch {}

			try {
				// Backend may return role as number (1 = admin)
				const { data: me } = await api.get<any>("/api/users/me");

				const finalUser: User = {
					id: Number(me.id),
					username: me.username ?? me.name ?? "",
					email: me.email ?? "",
					role: normalizeRole(me.role, "User"), // <-- key line
				};

				if (stored) loginFn(finalUser, stored);
			} catch {
				if (stored) {
					try {
						const u = userFromAccessToken(stored.accessToken);
						loginFn(u, stored);
					} catch {}
				}
			}
		})();
	}, [loginFn]);

	const value = useMemo<AuthState>(
		() => ({
			user,
			tokens,
			login: loginFn,
			logout: logoutFn,
		}),
		[user, tokens, loginFn, logoutFn]
	);

	return (
		<AuthContext.Provider value={value}>{children}</AuthContext.Provider>
	);
}
