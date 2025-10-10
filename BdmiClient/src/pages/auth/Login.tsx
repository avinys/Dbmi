import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { api } from "@/lib/api";
import { useAuth } from "@/auth/useAuth";
import { useNavigate } from "react-router-dom";
import { userFromAccessToken } from "@/auth/tokenUser";

const schema = z.object({
	username: z.string().min(3),
	password: z.string().min(6),
});
type FormData = z.infer<typeof schema>;

export default function Login() {
	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
	} = useForm<FormData>({ resolver: zodResolver(schema) });

	const { login } = useAuth();
	const nav = useNavigate();

	async function onSubmit(values: FormData) {
		// Your backend expects { username, password }
		const { data: tokens } = await api.post("/api/auth/login", values);

		// Try /users/me if you have it; otherwise decode the JWT:
		let me;
		try {
			const r = await api.get("/api/users/me");
			me = r.data;
		} catch {
			me = userFromAccessToken(tokens.accessToken);
		}

		login(me, tokens);
		nav("/");
	}

	return (
		<div className="mx-auto max-w-md card">
			<h1 className="text-2xl font-bold mb-4">Sign in</h1>
			<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
				<div>
					<label className="label">Username</label>
					<input
						className="input"
						type="text"
						{...register("username")}
					/>
					{errors.username && (
						<p className="text-red-400 text-sm">
							{errors.username.message}
						</p>
					)}
				</div>
				<div>
					<label className="label">Password</label>
					<input
						className="input"
						type="password"
						{...register("password")}
					/>
					{errors.password && (
						<p className="text-red-400 text-sm">
							{errors.password.message}
						</p>
					)}
				</div>
				<button className="btn-primary" disabled={isSubmitting}>
					{isSubmitting ? "Signing in…" : "Sign in"}
				</button>
			</form>
		</div>
	);
}
