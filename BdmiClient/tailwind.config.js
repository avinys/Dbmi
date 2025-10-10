/** @type {import('tailwindcss').Config} */
export default {
	darkMode: "class",
	content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
	theme: {
		extend: {
			colors: {
				background: {
					DEFAULT: "#0b0f15",
					100: "#0b0f15",
					200: "#0f1520",
				},
				surface: { 100: "#131a24", 200: "#16202c" },
				primary: { DEFAULT: "#6ee7b7", 600: "#34d399" },
				accent: { DEFAULT: "#93c5fd" },
			},
		},
		container: { center: true, padding: "1rem" },
	},
	plugins: [],
};
