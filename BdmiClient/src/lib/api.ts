import axios, { AxiosRequestConfig, AxiosRequestHeaders } from "axios";
import dayjs from "dayjs";

export type TokenResponseDto = {
	accessToken: string;
	refreshToken: string;
	expiresAt: string; // ISO
};

const STORAGE_KEY = "bdmi.tokens";
const BASE_URL = import.meta.env.VITE_API_URL ?? "/"; // ← same-origin by default

let tokens: TokenResponseDto | null = null;

// bootstrap from storage
try {
	const raw = localStorage.getItem(STORAGE_KEY);
	if (raw) tokens = JSON.parse(raw);
} catch {}

export function setAuthTokens(t: TokenResponseDto) {
	tokens = t;
	localStorage.setItem(STORAGE_KEY, JSON.stringify(t));
}
export function clearAuthTokens() {
	tokens = null;
	localStorage.removeItem(STORAGE_KEY);
}
export function getStoredTokens(): TokenResponseDto | null {
	try {
		const raw = localStorage.getItem(STORAGE_KEY);
		return raw ? JSON.parse(raw) : null;
	} catch {
		return null;
	}
}

export const api = axios.create({ baseURL: BASE_URL });
// a bare client without interceptors for refresh
const refreshClient = axios.create({ baseURL: BASE_URL });

// attach bearer
api.interceptors.request.use((config) => {
	if (!tokens) tokens = getStoredTokens();
	if (tokens?.accessToken) {
		config.headers = (config.headers ?? {}) as AxiosRequestHeaders;
		(config.headers as any).Authorization = `Bearer ${tokens.accessToken}`;
	}
	return config;
});

let refreshing = false;
let waiters: Array<(t: string) => void> = [];

async function refreshAccessToken(): Promise<string> {
	if (!tokens?.refreshToken) throw new Error("No refresh token");
	const { data } = await refreshClient.post<TokenResponseDto>(
		"/auth/refresh",
		{ refreshToken: tokens.refreshToken }
	);
	setAuthTokens(data);
	return data.accessToken;
}

// 401 -> refresh -> retry
api.interceptors.response.use(
	(r) => r,
	async (error) => {
		const original: AxiosRequestConfig & { _retry?: boolean } =
			error.config ?? {};
		if (error.response?.status === 401 && !original._retry) {
			original._retry = true;

			if (refreshing) {
				const token = await new Promise<string>((res) =>
					waiters.push(res)
				);
				original.headers = original.headers ?? {};
				(original.headers as any).Authorization = `Bearer ${token}`;
				return api(original);
			}

			try {
				refreshing = true;
				const newToken = await refreshAccessToken();
				waiters.forEach((w) => w(newToken));
				waiters = [];
				original.headers = original.headers ?? {};
				(original.headers as any).Authorization = `Bearer ${newToken}`;
				return api(original);
			} finally {
				refreshing = false;
			}
		}
		throw error;
	}
);

// keep tabs in sync
window.addEventListener("storage", (e) => {
	if (e.key === STORAGE_KEY) tokens = getStoredTokens();
});

export async function ensureFreshToken() {
	if (!tokens) return;
	const secondsLeft = dayjs(tokens.expiresAt).diff(dayjs(), "second");
	if (secondsLeft < 30) await refreshAccessToken();
}
