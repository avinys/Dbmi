import axios from "axios";

export type ApiErrorShape = {
	error?: string;
	message?: string;
	title?: string;
	detail?: string;
	errors?: Record<string, string[] | string>;
};

export function getApiErrorMessage(err: unknown): string {
	if (axios.isAxiosError(err)) {
		const data = err.response?.data as ApiErrorShape | string | undefined;
		if (typeof data === "string") return data;
		if (data?.error) return data.error;
		if (data?.message) return data.message;
		if (data?.title || data?.detail)
			return [data.title, data.detail].filter(Boolean).join(": ");
		if (data?.errors) {
			const first = Object.values(data.errors)[0];
			return Array.isArray(first) ? first[0] : String(first);
		}
		return err.message || "Unexpected error";
	}
	return (err as Error)?.message ?? "Unexpected error";
}
