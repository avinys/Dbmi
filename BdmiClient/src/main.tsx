import React from "react";
import ReactDOM from "react-dom/client";
import {
	MutationCache,
	QueryCache,
	QueryClient,
	QueryClientProvider,
} from "@tanstack/react-query";
import { BrowserRouter } from "react-router-dom";
import "./index.css";
import App from "./App";
import toast from "react-hot-toast";
import { getApiErrorMessage } from "./lib/api-error";
import axios from "axios";

const qc = new QueryClient({
	queryCache: new QueryCache({
		onError: (error, query) => {
			// allow per-query opt-out
			if (query?.meta?.suppressToast) return;
			if (axios.isCancel(error)) return; // skip canceled requests
			toast.error(getApiErrorMessage(error));
		},
	}),
	mutationCache: new MutationCache({
		onError: (error, _vars, _ctx, mutation) => {
			if (mutation?.meta?.suppressToast) return;
			if (axios.isCancel(error)) return;
			toast.error(getApiErrorMessage(error));
		},
	}),
	defaultOptions: {
		queries: { retry: 1 },
	},
});

ReactDOM.createRoot(document.getElementById("root")!).render(
	<React.StrictMode>
		<QueryClientProvider client={qc}>
			<BrowserRouter>
				<App />
			</BrowserRouter>
		</QueryClientProvider>
	</React.StrictMode>
);
