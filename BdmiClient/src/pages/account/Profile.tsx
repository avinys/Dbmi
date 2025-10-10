import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useAuth } from "@/auth/useAuth";
import { api } from "@/lib/api";

const S = z.object({ username: z.string().min(3), email: z.string().email() });

type F = z.infer<typeof S>;

export default function Profile() {
	const { user } = useAuth();
	const {
		register,
		handleSubmit,
		formState: { errors, isSubmitting },
		reset,
	} = useForm<F>({
		resolver: zodResolver(S),
		defaultValues: { username: user!.username, email: user!.email },
	});

	async function onSubmit(values: F) {
		const { data } = await api.put(`/api/users/${user!.id}`, values);
		localStorage.setItem(
			"moviehub.user",
			JSON.stringify({ ...user!, ...values })
		);
		reset(values);
	}

	return (
		<div className="mx-auto max-w-lg card">
			<h1 className="text-2xl font-bold mb-4">Profile</h1>
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
				<button className="btn-primary" disabled={isSubmitting}>
					Save changes
				</button>
			</form>
		</div>
	);
}
