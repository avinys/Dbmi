import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { Movie, Review, UpdateReviewDTO, UserListItemDTo } from "@/lib/types";
import { ReviewForm } from "../movies/ReviewForm";
import { useState } from "react";
import Modal from "@/components/Modal";
import ConfirmDelete from "@/components/ConfirmDelete";

export default function AdminReviews() {
	const q = useQueryClient();
	const { data: reviews } = useQuery<Review[]>({
		queryKey: ["reviews-all"],
		queryFn: async () => (await api.get("/api/reviews")).data,
	});
	const { data: movies } = useQuery<Movie[]>({
		queryKey: ["movies-all"],
		queryFn: async () => (await api.get("/api/movies")).data,
	});
	const { data: users } = useQuery<UserListItemDTo[]>({
		queryKey: ["users-all"],
		queryFn: async () => (await api.get("/api/users")).data,
	});
	const del = useMutation({
		mutationFn: async (id: number) =>
			(await api.delete(`/api/reviews/${id}`)).data,
		onSuccess: () => q.invalidateQueries({ queryKey: ["reviews-all"] }),
	});

	const [open, setOpen] = useState(false);
	const [editing, setEditing] = useState<Review | null>(null);
	const [deleting, setDeleting] = useState<Review | null>(null);
	const [openConfirmDelete, setOpenConfirmDelete] = useState<boolean>(false);

	const saveReview = useMutation({
		mutationFn: async (payload: Review) => {
			const body: UpdateReviewDTO = {
				score: payload.score,
				title: payload.title,
				body: payload.body,
			};
			return (await api.put(`/api/reviews/${payload.id}`, body)).data;
		},
		onSuccess: () => {
			q.invalidateQueries({ queryKey: ["reviews-all"] });
			setOpen(false);
			setEditing(null);
		},
	});

	return (
		<div className="space-y-4">
			<h1 className="text-2xl font-bold">Admin · Reviews</h1>
			<ul className="grid gap-3">
				{reviews?.map((r) => (
					<li
						key={r.id}
						className="card flex flex-col items-center justify-between sm:flex-row"
					>
						<div>
							<p>{r.title}</p>
							<p>
								Movie:{" "}
								{movies.find((m) => m.id === r.movieId).title}
							</p>
							<p>
								Author:{" "}
								{users?.find((u) => u.id === r.userId).username}
							</p>
							<p>Score: {r.score}</p>
						</div>
						<div>
							<button
								className="btn-ghost"
								onClick={() => {
									setEditing(r);
									setOpen(true);
								}}
							>
								Edit
							</button>
							<button
								className="btn-ghost text-red-500/50"
								onClick={() => {
									setDeleting(r);
									setOpenConfirmDelete(true);
								}}
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
				title="Edit review"
			>
				{editing && (
					<ReviewForm
						value={editing}
						onSubmit={(v) => saveReview.mutate(v)}
					/>
				)}
			</Modal>

			<Modal
				open={openConfirmDelete}
				onClose={() => {
					setOpenConfirmDelete(false);
					setDeleting(null);
				}}
				title="Confirm Review Deletion"
			>
				{deleting && (
					<ConfirmDelete review={deleting} onDelete={del.mutate} />
				)}
			</Modal>
		</div>
	);
}
