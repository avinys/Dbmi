/// <reference types="vite/client" />

// (optional) declare the vars you use for better typing:
interface ImportMetaEnv {
	readonly VITE_API_URL?: string;
}
interface ImportMeta {
	readonly env: ImportMetaEnv;
}
