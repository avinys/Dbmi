// src/pages/admin/AdminUsers.tsx
import { useAuth } from "@/auth/useAuth";
import ConfirmDelete from "@/components/ConfirmDelete";
import Modal from "@/components/Modal";
import { api } from "@/lib/api";
import { UserDetailsDTO, UserListItemDTo } from "@/lib/types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";

type UpdateUserDto = {
	username: string;
	email: string;
};

function roleLabel(role: unknown): "Admin" | "User" {
	if (typeof role === "number") return role === 1 ? "Admin" : "User";
	if (typeof role === "string")
		return role.trim().toLowerCase() === "admin" ? "Admin" : "User";
	return "User";
}

export default function AdminUsers() {
	const q = useQueryClient();
	const { user: me } = useAuth();

	const { data: users = [] } = useQuery<UserListItemDTo[]>({
		queryKey: ["users-all"],
		queryFn: async () => (await api.get("/api/users")).data,
	});

	const [openEdit, setOpenEdit] = useState(false);
	const [editing, setEditing] = useState<UserListItemDTo | null>(null);

	const [openConfirmDelete, setOpenConfirmDelete] = useState(false);
	const [deleting, setDeleting] = useState<UserListItemDTo | null>(null);

	const saveUser = useMutation({
		mutationFn: async (p: { id: number; body: UpdateUserDto }) =>
			(await api.put(`/api/users/${p.id}`, p.body)).data,
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["users-all"] });
			setOpenEdit(false);
			setEditing(null);
		},
	});

	const deletingId = deleting?.id;
	const { data: deletingUser } = useQuery<UserDetailsDTO>({
		queryKey: ["users", deletingId],
		queryFn: () => api.get(`/api/users/${deletingId}`).then((r) => r.data),
		enabled: !!deletingId,
	});

	const delUser = useMutation({
		mutationFn: async (id: number) =>
			(await api.delete(`/api/users/${id}`)).data,
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["users-all"] });
			setOpenConfirmDelete(false);
			setDeleting(null);
		},
	});

	return (
		<div className="space-y-4">
			<h1 className="text-2xl font-bold">Admin · Users</h1>

			<ul className="grid gap-3">
				{users.map((u) => (
					<li
						key={u.id}
						className="card flex flex-col items-center justify-between sm:flex-row"
					>
						<div className="space-y-0.5">
							<p className="font-semibold">{u.username}</p>
							<p className="text-sm text-gray-300">{u.email}</p>
							<p className="text-xs text-gray-400">
								Role: {roleLabel(u.role)}
							</p>
						</div>
						<div className="flex gap-2">
							<button
								className="btn-ghost"
								onClick={() => {
									setEditing(u);
									setOpenEdit(true);
								}}
							>
								Edit
							</button>
							<button
								className="btn-ghost text-red-500/60"
								onClick={() => {
									setDeleting(u);
									setOpenConfirmDelete(true);
								}}
								disabled={me?.id === u.id} // optional: prevent self-delete
								title={
									me?.id === u.id
										? "You cannot delete your own account"
										: ""
								}
							>
								Delete
							</button>
						</div>
					</li>
				))}
			</ul>

			{/* Edit modal */}
			<Modal
				open={openEdit}
				onClose={() => {
					setOpenEdit(false);
					setEditing(null);
				}}
				title="Edit user"
			>
				{editing && (
					<UserEditForm
						key={editing.id}
						value={editing}
						onSubmit={(body) =>
							saveUser.mutate({ id: editing.id, body })
						}
						isSubmitting={saveUser.isPending}
					/>
				)}
			</Modal>

			{/* Confirm delete modal */}
			<Modal
				open={openConfirmDelete}
				onClose={() => {
					setOpenConfirmDelete(false);
					setDeleting(null);
				}}
				title="Confirm User Deletion"
			>
				{deleting && (
					<ConfirmDelete
						user={deletingUser}
						onDelete={delUser.mutate}
					/>
				)}
			</Modal>
		</div>
	);
}

function UserEditForm({
	value,
	onSubmit,
	isSubmitting,
}: {
	value: UserListItemDTo;
	onSubmit: (body: UpdateUserDto) => void;
	isSubmitting?: boolean;
}) {
	const [username, setUsername] = useState<string>(value.username);
	const [email, setEmail] = useState<string>(value.email);

	return (
		<form
			className="space-y-4"
			onSubmit={() => onSubmit({ username, email })}
		>
			<div>
				<label className="label">Username</label>
				<input
					className="input"
					defaultValue={value.username}
					onChange={(e) => setUsername(e.target.value)}
					required
				/>
			</div>

			<div>
				<label className="label">Email</label>
				<input
					className="input"
					type="email"
					defaultValue={value.email}
					onChange={(e) => setEmail(e.target.value)}
					required
				/>
			</div>

			<div className="flex justify-end gap-2">
				<button className="btn-primary" disabled={isSubmitting}>
					{isSubmitting ? "Saving…" : "Save"}
				</button>
			</div>
		</form>
	);
}
