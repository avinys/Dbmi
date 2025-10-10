import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { api } from "@/lib/api";
import { useNavigate } from "react-router-dom";

const schemaR = z.object({
	username: z.string().min(3),
	email: z.string().email(),
	password: z.string().min(6),
});
type R = z.infer<typeof schemaR>;

export default function Register() {
	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
	} = useForm<R>({ resolver: zodResolver(schemaR) });

	const nav = useNavigate();

	async function onSubmit(values: R) {
		await api.post("/api/auth/register", values);
		nav("/login");
	}

	return (
		<div className="mx-auto max-w-md card">
			<h1 className="text-2xl font-bold mb-4">Create account</h1>
			<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
				<div>
					<label className="label">Username</label>
					<input className="input" {...register("username")} />
					{errors.username && (
						<p className="text-red-400 text-sm">
							{errors.username.message}
						</p>
					)}
				</div>
				<div>
					<label className="label">Email</label>
					<input
						className="input"
						type="email"
						{...register("email")}
					/>
					{errors.email && (
						<p className="text-red-400 text-sm">
							{errors.email.message}
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
					{isSubmitting ? "Creating…" : "Create account"}
				</button>
			</form>
		</div>
	);
}
