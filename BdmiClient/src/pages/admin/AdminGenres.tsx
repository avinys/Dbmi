import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { Genre } from "@/lib/types";
import { useState } from "react";
import Modal from "@/components/Modal";

export default function AdminGenres() {
	const q = useQueryClient();
	const { data: genres } = useQuery<Genre[]>({
		queryKey: ["genres"],
		queryFn: async () => (await api.get("/api/genres")).data,
	});
	const [open, setOpen] = useState(false);
	const [editing, setEditing] = useState<Genre | null>(null);

	const save = useMutation({
		mutationFn: async (g: Genre) =>
			g.id
				? (await api.put(`/api/genres/${g.id}`, g)).data
				: (await api.post("/api/genres", g)).data,
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["genres"] });
			setOpen(false);
			setEditing(null);
		},
	});
	const del = useMutation({
		mutationFn: async (id: number) =>
			(await api.delete(`/api/genres/${id}`)).data,
		onSuccess: () => q.invalidateQueries({ queryKey: ["genres"] }),
	});

	return (
		<div className="space-y-4">
			<div className="flex items-center justify-between">
				<h1 className="text-2xl font-bold">Admin · Genres</h1>
				<button
					className="btn-primary"
					onClick={() => {
						setEditing({ id: 0, name: "" });
						setOpen(true);
					}}
				>
					New genre
				</button>
			</div>
			<ul className="grid gap-3">
				{genres?.map((g) => (
					<li
						key={g.id}
						className="card flex items-center justify-between"
					>
						<span>{g.name}</span>
						<div className="flex gap-2">
							<button
								className="btn-ghost"
								onClick={() => {
									setEditing(g);
									setOpen(true);
								}}
							>
								Edit
							</button>
							<button
								className="btn-ghost"
								onClick={() => del.mutate(g.id)}
							>
								Delete
							</button>
						</div>
					</li>
				))}
			</ul>

			<Modal
				open={open}
				onClose={() => {
					setOpen(false);
					setEditing(null);
				}}
				title={editing?.id ? "Edit genre" : "New genre"}
			>
				{editing && (
					<GenreForm
						value={editing}
						onSubmit={(g) => save.mutate(g)}
					/>
				)}
			</Modal>
		</div>
	);
}

function GenreForm({
	value,
	onSubmit,
}: {
	value: Genre;
	onSubmit: (g: Genre) => void;
}) {
	const [form, setForm] = useState<Genre>(value);
	return (
		<form
			className="space-y-3"
			onSubmit={(e) => {
				e.preventDefault();
				onSubmit(form);
			}}
		>
			<div>
				<label className="label">Name</label>
				<input
					className="input"
					value={form.name}
					onChange={(e) => setForm({ ...form, name: e.target.value })}
				/>
			</div>
			<button className="btn-primary">Save</button>
		</form>
	);
}
