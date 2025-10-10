import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { Review, UpdateReviewDTO } from "@/lib/types";
import { useAuth } from "@/auth/useAuth";
import Modal from "@/components/Modal";
import { useState } from "react";
import { ReviewForm } from "../movies/ReviewForm";
import ConfirmDelete from "@/components/ConfirmDelete";

export default function MyReviews() {
	const { user } = useAuth();
	const q = useQueryClient();
	const { data: reviews } = useQuery<Review[]>({
		queryKey: ["my-reviews", user?.id],
		queryFn: async () => (await api.get(`/api/users/reviews`)).data,
		enabled: !!user,
	});

	const [open, setOpen] = useState(false);
	const [editing, setEditing] = useState<Review | null>(null);
	const [openDelete, setOpenDelete] = useState(false);
	const [deleting, setDeleting] = useState<Review | null>(null);

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
			q.invalidateQueries({ queryKey: ["my-reviews", user?.id] });
			setOpen(false);
			setEditing(null);
		},
	});

	const delReview = useMutation({
		mutationFn: async (rid: number) =>
			(await api.delete(`/api/reviews/${rid}`)).data,
		onSuccess: () =>
			q.invalidateQueries({ queryKey: ["my-reviews", user?.id] }),
	});

	return (
		<div>
			<h1 className="text-2xl font-bold mb-4">My reviews</h1>
			<ul className="space-y-3">
				{reviews?.map((r) => (
					<li
						key={r.id}
						className="p-3 rounded-xl bg-surface-100 border border-surface-200"
					>
						<div className="flex items-center justify-between">
							<div className="font-semibold">
								<p>{r.title}</p>
								<p>Score: {r.score}</p>
							</div>
							{user &&
								(user.role === "Admin" ||
									user.id === r.userId) && (
									<div className="flex gap-2">
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
											className="btn-ghost"
											onClick={() => {
												setOpenDelete(true);
												setDeleting(r);
											}}
										>
											Delete
										</button>
									</div>
								)}
						</div>
						{r.body && (
							<p className="text-gray-300 mt-1">{r.body}</p>
						)}
					</li>
				))}
			</ul>

			<Modal
				open={open}
				onClose={() => {
					setOpen(false);
					setEditing(null);
				}}
				title={editing?.id ? "Edit review" : "New review"}
			>
				{editing && (
					<ReviewForm
						value={editing}
						onSubmit={(v) => saveReview.mutate(v)}
					/>
				)}
			</Modal>

			<Modal
				open={openDelete}
				onClose={() => {
					setOpenDelete(false);
					setDeleting(null);
				}}
				title="Confirm Delete Review"
			>
				{deleting && (
					<ConfirmDelete
						review={deleting}
						onDelete={delReview.mutate}
					/>
				)}
			</Modal>
		</div>
	);
}
